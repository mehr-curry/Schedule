using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

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
        
        public void Calculate(ScheduleRow row, double change){
			if(row == null)
                throw new ArgumentNullException("row");
			
			if(!Owner.Rows.Contains(row))
                throw new ArgumentOutOfRangeException("row");
            
            var width = row.Height + change / (Owner.Rows.IndexOf(row) + 1);
            
            foreach (ScheduleRow col in Owner.Rows)
                    col.SetCurrentValue(ScheduleRow.HeightProperty, width);

		    Owner.InvalidateMeasure();

			//Debug.WriteLine(string.Format("Calc {0} Real {1}", width, Row.Height));
		}

        public double GetOffset(TimeSpan value)
        {
            if (Owner.Rows.Any())
            {
                var interval = (TimeSpan)Owner.RowGenerator.Interval;
                var factor = Owner.Rows[0].Height / interval.TotalSeconds;
                return value.TotalSeconds * factor;
            }

            return double.NaN;
        }

        public double GetDesiredHeight()
        {
            if (Owner == null)
                throw new InvalidOperationException("Kein Besitzer");
            if (!Owner.Rows.Any())
                throw new InvalidOperationException("Keine Zeilen");

            return Owner.Rows.Count * Owner.Rows[0].Height;
        }

        public double GetOffset(ScheduleRow row)
	    {
	        double offset = 0;

	        foreach (ScheduleRow r in Owner.Rows)
	        {
	            if (r != row)
	                offset += r.Height;
                else 
                    break;
	        }

	        return offset;
	    }

        public IEnumerable<ScheduleRow> GetVisibleRows(Rect viewport)
        {
            return from row in Owner.Rows let xOffset = GetOffset(row) where xOffset < viewport.Bottom && xOffset + row.Height > viewport.Top select row;
        }

    }
}