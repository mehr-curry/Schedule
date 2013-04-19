using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
    /// <summary>
    /// Description of EqualRowLayouter.
    /// </summary>
    public class EvenRowLayouter : IRowLayouter
    {
        public EvenRowLayouter()
        {
        }

        public Schedule Owner { get; set; }
        
        public void SetAll(double height){
			foreach (ScheduleRow row in Owner.Rows)
                row.SetCurrentValue(ScheduleRow.HeightProperty, height);

		    Owner.InvalidateMeasure();
		}
		
        public void Calculate(ScheduleRow row, double change){
			if(row == null)
                throw new ArgumentNullException("row");
			
			if(!Owner.Rows.Contains(row))
                throw new ArgumentOutOfRangeException("row");
            
            var height = row.Height + change / (Owner.Rows.IndexOf(row) + 1);
            
            foreach (ScheduleRow r in Owner.Rows)
                    r.SetCurrentValue(ScheduleRow.HeightProperty, height);

		    Owner.InvalidateMeasure();

			//Debug.WriteLine(string.Format("Calc {0} Real {1}", width, Row.Height));
		}

        public double TranslateFromSource(object value)
        {
            return SnappingBehavior.TranslateFromSource(value);
            //if (Owner.Rows.Any())
            //{
            //    var interval = (TimeSpan)Owner.RowGenerator.Interval;
            //    var factor = Owner.Rows[0].Height / interval.TotalSeconds;
            //    return ((TimeSpan)value).TotalSeconds * factor;
            //}

            //return double.NaN;
        }
        
        public object TranslateToSource(double offset)
        {
            return SnappingBehavior.TranslateToSource(offset);
            //if (Owner.Rows.Any())
            //{
            //    var interval = (TimeSpan)Owner.RowGenerator.Interval;
            //    var end = (TimeSpan)Owner.RowGenerator.End;
            //    var factor = interval.TotalSeconds / Owner.Rows[0].Height;
            //    var seconds = offset * factor;
            //    seconds = Math.Round(seconds / 1800,0) * 1800; //Taktung bzw Ausrichtung
            //    if(seconds < 0D) seconds = 0D;
            //    if(seconds > end.TotalSeconds) seconds = end.TotalSeconds;
            //    return TimeSpan.FromSeconds((int)Math.Round(seconds,0));
            //}

            //return TimeSpan.Zero;
        }

        public double GetDesiredHeight()
        {
            if (Owner == null)
                throw new InvalidOperationException("Kein Besitzer");
            if (!Owner.Rows.Any())
                throw new InvalidOperationException("Keine Zeilen");

            return Owner.Rows.Count * Owner.Rows[0].Height;
        }

        public IEnumerable<ScheduleRow> GetVisibleRows(Rect viewport)
        {
            return from row in Owner.Rows let xOffset = TranslateFromSource((TimeSpan)row.Value) where xOffset < viewport.Bottom && xOffset + row.Height > viewport.Top select row;
        }

        public ISnappingBehavior SnappingBehavior { get; set; }
    }
}