using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
    public class TimeRowSnappingBehavior
        : ISnappingBehavior
    {
        public Schedule Owner { get; set; }

        public double Align(double horizontalValue)
        {
            return TranslateFromSource(TranslateToSource(horizontalValue));
        }

        public object TranslateToSource(double offset)
        {
            if (Owner.Rows.Any())
            {
                var interval = (TimeSpan)Owner.RowGenerator.Interval;
                var end = (TimeSpan)Owner.RowGenerator.End;
                var factor = interval.TotalMilliseconds / Owner.Rows[0].Height;
                var ms = offset * factor;
                ms = Math.Round(ms / 1800000, 0) * 1800000; //Taktung bzw Ausrichtung
                if (ms < 0D) ms = 0D;
                if (ms > end.TotalMilliseconds) ms = end.TotalMilliseconds;
                return TimeSpan.FromMilliseconds((int)Math.Round(ms, 0));
            }

            return TimeSpan.Zero;
        }

        public double TranslateFromSource(object value)
        {
            if (Owner.Rows.Any())
            {
                var interval = (TimeSpan)Owner.RowGenerator.Interval;
                var factor = Owner.Rows[0].Height / interval.TotalSeconds;
                return ((TimeSpan)value).TotalSeconds * factor;
            }

            return double.NaN;
        }
    }
}
