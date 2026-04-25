# Orion Client Lockup Investigation

## Status: RESOLVED (2026-04-25)

**Actual root cause:** synchronous file I/O on the game thread, not packet
flooding. Three log writers were blocking the game loop:

1. **`Scripts/Commands/Logging.cs`** — opened a new `StreamWriter` per command
   AND per property change AND per craft action. `CraftItem.cs:1910` logs
   every successful craft via `CommandLogging.WriteLine`. Two macro-crafting
   players at ~5 actions/sec each produced ~10 file open/write/close cycles
   per second, stalling the game thread for ALL connected clients (not just
   the crafters or the players nearby).
2. **`Server/Network/NetState.cs` SendQueueWarn telemetry** — the diagnostic
   logging we added to investigate the lockup was itself contributing. Each
   burst event called `File.AppendAllText` (open + write + close) on the
   game thread.
3. **`Scripts/Services/Chat/Logging.cs`** — relatively minor but every chat
   message hit disk; under load this added pressure.

**Why the symptoms looked like packet flooding:** when the game thread stalls
on disk I/O, *all* connected clients get starved of move-acks and state
updates. A player whose own packet load is tiny (`F3=210` over 10 s) still
appeared frozen because their commands weren't being processed promptly.
This explains why telemetry showed lockups happening to clients who weren't
receiving any meaningful packet volume — they were victims of game-thread
starvation, not packet floods.

**Real fixes shipped (commits `3d6da4a3`, `00e825e3`, `aeef5aeb`):**

1. Disabled `CommandLogging` and `ChatLogging` by default.
2. Removed the `WriteSendQueueWarn` helper, all call sites, and all rolling
   telemetry tracking. Detection logic for the receive-side liveness kick
   and the stuck-backlog `Dispose` still works in memory; just no log lines.
3. Capped champion-spawn `Respawn()` at 4 mobs per tick — separately fixed
   the Rikktor level-change burst that was a real packet flood (50+
   `MobileIncoming` packets in one frame).
4. Sent craft and harvest sounds only to the actor (`from.SendSound` instead
   of `from.PlaySound`) — defense in depth for macro-craft scenarios.

**Lessons reinforced:**
- "Players freeze when X happens" doesn't always mean X is sending too many
  packets to the player. The cause can be game-thread blocking from
  unrelated server-side activity.
- Diagnostic logging itself can be the bug. Add telemetry carefully and
  ensure it's not on the critical path.
- The user's hypothesis (logging was the cause) was correct from early in
  the investigation. We chased throttling/caching theories for too long
  before testing the simpler theory.

The investigation that follows captures the work done before the actual
cause was identified. Left in place because the data points and dead-end
attempts may be useful context if someone runs into similar symptoms later.

---

## Problem

Players using the Orion UO client freeze in dense-item areas:
- Running in a house with many locked-down items (1400+).
- Loading a Shadowguard Roof room (room materializes with many items + mobiles).
- Running past houses in Luna.

**Symptom:** client UI stops responding, no disconnect from server, player must
log out and back in to recover. Movement stops; hovering doesn't produce
tooltips; nothing reaches the screen until relog. Server-side the socket
stays technically alive — TCP ACKs slow down but don't stop.

## Telemetry currently in the tree

`Server/Network/NetState.cs` logs two kinds of events to
`Logs/SendQueueWarn.log`:

- **`[SendQueueWarn]`** — a client's outbound queue passed 256 KB.
  Throttled to once per 30 seconds per client. Line format:
  ```
  [SendQueueWarn]  Client: <ip>: backlog <N> KB (packet <ID>)  last10s: <top5>  peakPkt=<bytes>B
  ```
- **`[BurstDetected]`** — a client received >200 packets in one wall-clock
  second. Throttled to once per 2 seconds per client. Line format:
  ```
  [BurstDetected]  Client: <ip>: <N> packets/s  last10s: <top5>  peakPkt=<bytes>B
  ```

Packet IDs are the single-byte values from the UO protocol. `BF/XXXX` format
indicates a 0xBF general-info packet with subcommand `XXXX` (captured
pre-encryption). `last10s` is the top-5 packet types by count in the trailing
10-second window.

**This is non-invasive** — it only logs, doesn't modify the send path.

## Confirmed data points (from 2026-04-24 logs)

### Running-in-dense-house lockup
Peak: `872 packets/s` with `DC=1828 F3=1824 D6=1809` over 10 s.
Pattern: client re-requests full OPL (0xD6) for every hash (0xDC) it
receives, 3× amplifying per-item traffic.

### Running-past-Luna-houses lockup
Peak: `1789 packets/s` on entry, `F3=1004 DC=877` sustained.
Small per-step bursts (individually below throttle thresholds) cascade into
sustained ~170-200 pkt/s that Orion can't keep up with.

### Second-player lockup in same location
Peak: `201 packets/s` on a different IP — same pattern. Repeatable across
players, not client-specific within Orion users.

### Post-throttle lockup (with our first attempt in place)
`F3=1099 22=76 A3=11 A1=10 A2=10` in 10 s. Pure F3 (WorldItem) at 110/s
still locked the client even after we suppressed the 0xDC/0xD6 amplification.

### Approximate Orion lockup threshold
**~200-270 sustained packets/s** to a single client. Cumulative; even
moderate sustained rates over 10+ seconds cause the freeze.

## What we tried (all reverted in `55b2db76`)

Commits are functional code only — they were rolled back because each
introduced correctness bugs that degraded UX below the lockup baseline.

| Commit | Attempt | Why it failed |
|---|---|---|
| `c0232c5d` | Throttle `BatchQueryProperties` (chunk OPL replies at 25 per 50 ms) | Reduced D6 amplification but F3+DC view-enter still flooded. |
| `3ca5952b` | Chunk `SendEverything` over ticks (20 items per 100 ms) | Only covered teleport/login/map-change paths. Running didn't trigger `SendEverything`. |
| `cd249827` | Chunk the per-tile Location setter too | Per-call threshold (100 items) didn't help — running dense areas fires many small calls under the threshold that cumulatively flood. |
| `7948d672` | Centralize in NetState token bucket (60 items/s inline, 60/s drain) | Prevented lockups but houses took 23 seconds to fully draw — players said it looked broken. |
| `d5a2377a` | Loosen token bucket (150 inline, 100 drain) | Still slow; partial lockup cases returned. |
| `a2eb227c` | Skip OPL hash on view-enter + raise rates + skip-stale drain | Broke tooltips for everyone (client never learned items had properties). |
| `4141195a` | Restore OPL hash | Back to square one on packet rate. |
| `4fa70484` | Sticky F3/DC cache with 0x1D invalidation | Cache poisoning: items got marked cached at queue-time, skipped by CanSee at drain, never sent — but cache thought they were. Houses showed as empty grass. |

## Why throttling alone can't work

- Orion's lockup threshold (~200-270 pkt/s sustained) is **lower than what
  normal gameplay generates in dense areas**. A player running through
  Luna reveals 100+ items per second by view-frustum edge; each needs at
  minimum a 0xF3 packet. No server-side rate limit can stay below the
  lockup threshold *and* keep items loading responsively.

- Sticky caching would help the repeat-view case (running in/out of the
  same house) but cannot help the first-time load of a Roof room or any
  genuinely new dense area. Our cache implementation also required
  coordinating with the client's actual memory state, which we could not
  reliably track through all of ServUO's send paths.

## Probable real fixes (not yet attempted)

### 1. Orion client configuration / version
- Ask Orion community: is there a per-second packet decode cap, receive
  buffer size, or decoder-thread setting players can tune?
- Is this a known issue in specific Orion versions?
- Do they offer a debug/diag mode that would confirm a decoder stall?

### 2. Protocol-level packet batching
- UO protocol does not have a native multi-item WorldItem packet.
- ClassicUO / some ServUO forks support a custom compound packet that
  sends N items in one payload. If Orion supports a similar extension,
  packet count collapses even at same total bytes — Orion's per-packet
  decode overhead is likely the actual bottleneck.
- Requires cooperation with Orion maintainers.

### 3. Reduce item count in dense areas (content-side)
- Static decorative items that don't need server-side interaction could
  be moved to map tiles (0x40 static) instead of dynamic items.
- Shadowguard rooms stage their addon all at once during `Setup()`;
  staggering the spawn over 2-3 seconds would smooth the burst.

### 4. Spatial prioritization
- When a bulk view-enter fires, sort items by distance from player.
  Inner 50 send inline, outer items queue. Immediate surroundings appear
  instantly; far walls fill in over seconds.
- Does not reduce total packets but improves perceived responsiveness.
- Could be layered on top of a working throttle.

## Key code locations for next attempt

| File | What's there |
|---|---|
| `Server/Network/NetState.cs` | Current telemetry. Would host any rate limiter / cache. |
| `Server/Network/PacketHandlers.cs:BatchQueryProperties` | Serves 0xD6 replies. Natural chokepoint for OPL throttle. |
| `Server/Mobile.cs:SendEverything` | Bulk view-enter on teleport/login/map-change. |
| `Server/Mobile.cs` (Location setter) | Per-step view-enter while walking. |
| `Server/Item.cs:SendInfoTo` | Per-item F3+DC send. All call sites flow through here. |
| `Server/Item.cs:RemovePacket` / `Server/Network/Packets.cs:RemoveItem` | 0x1D delete. If implementing a sticky cache, must invalidate here. |

## Log locations

- `Logs/SendQueueWarn.log` — all telemetry events, appended.
- The log has an init line on each server startup — useful to separate sessions.

## Lessons learned

1. **Trust the user's hypothesis early.** From the beginning the user
   suggested logging might be the cause. We dismissed it because the
   data appeared to point at packet flooding. We were wrong; they were
   right. When someone close to the system has a hypothesis that
   contradicts your read of the data, take it seriously.
2. **Symptoms can mislead in concurrent systems.** A player freezing
   while receiving few packets looked like a client-decoder limit. It
   was actually game-thread starvation from unrelated disk I/O. The
   "victim" client had nothing to do with the cause.
3. **Diagnostic instrumentation can BE the bug.** Synchronous
   `File.AppendAllText` per event on the game thread defeats its own
   purpose. If you must log on the hot path, use a persistent stream
   with `AutoFlush`, or buffer to a background writer thread.
4. **Correctness > performance.** Several throttle/cache iterations
   "fixed" the lockup but broke item loading. Players would rather take
   the known freeze than a randomly-empty house. We had to fully
   rollback (`55b2db76`) before finding the actual cause.
5. **Sometimes the right fix is not in the system you suspect.** All of
   our packet-pipeline work was on a system that wasn't the bottleneck.
   The fix turned out to be one-line `Enabled = false` in two unrelated
   logger classes. When investigation goes long without progress,
   broaden the search.
