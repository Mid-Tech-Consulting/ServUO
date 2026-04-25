# Orion Client Lockup Investigation

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

1. **Iterate from data, not intuition.** The first throttle attempts
   assumed "peak pkt/s" was the metric. Real data showed "sustained
   pkt/s over ~10 s" is closer to what locks Orion. Enhance telemetry
   before tuning again.
2. **Correctness > performance.** Multiple iterations fixed the lockup
   but broke loading. Players would rather take the known freeze than a
   randomly-empty house.
3. **Protocol changes require client cooperation.** Server-side
   workarounds for a client decoder limit are blunt. Getting Orion
   community input first would have saved days of tuning.
