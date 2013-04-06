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
				throw new ArgumentNullException("Row");
			
			if(!Owner.Rows.Contains(row))
				throw new ArgumentOutOfRangeException("Row");
            
            var width = row.Height + change / (Owner.Rows.IndexOf(row) + 1);
            
            foreach (ScheduleRow col in Owner.Rows)
                    col.SetCurrentValue(ScheduleRow.HeightProperty, width);

		    Owner.InvalidateMeasure();

			//Debug.WriteLine(string.Format("Calc {0} Real {1}", width, Row.Height));
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