using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class PatrioticTimer : Timer
    {
        private readonly object m_Target; // Can be Item or Mobile
        // Standard high-quality UO hues: Red (1975), White (1153), Blue (1195 - Navy Blue)
        private static readonly int[] m_Hues = new int[] { 1975, 1153, 1195 };
        private int m_Index;

        public PatrioticTimer(object target) : base(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5))
        {
            m_Target = target;
            Priority = TimerPriority.OneSecond;

            // Set the first hue immediately on construction so there is no delay
            SetHue(m_Hues[0]);
        }

        private void SetHue(int hue)
        {
            if (m_Target is Item item && !item.Deleted)
            {
                item.Hue = hue;
                if (item is EtherealMount eth)
                {
                    eth.StatueHue = hue;
                    eth.TransparentMountedHue = hue;
                    eth.NonTransparentMountedHue = hue;
                }
            }
            else if (m_Target is Mobile mob && !mob.Deleted)
            {
                mob.Hue = hue;
            }
        }

        protected override void OnTick()
        {
            bool active = false;
            string name = null;

            if (m_Target is Item item && !item.Deleted)
            {
                active = true;
                name = item.Name;
            }
            else if (m_Target is Mobile mob && !mob.Deleted)
            {
                active = true;
                name = mob.Name;
            }

            // Stop the timer if the target is deleted, or if its name no longer contains "4th of July"
            if (!active || name == null || !name.Contains("4th of July"))
            {
                Stop();
                return;
            }

            m_Index = (m_Index + 1) % m_Hues.Length;
            SetHue(m_Hues[m_Index]);
        }
    }
}
