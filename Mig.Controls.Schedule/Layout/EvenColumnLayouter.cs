/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 20:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Mig.Controls.Schedule.Layout
{
	/// <summary>
	/// Description of EqualColumnLayouter.
	/// </summary>
	public class EvenColumnLayouter : IColumnLayouter
	{
		public EvenColumnLayouter()
		{
		}
		
		public Schedule Owner { get; set; }
		
		public void SetAll(double width){
			foreach (ScheduleColumn col in Owner.Columns)
                col.SetCurrentValue(ScheduleColumn.WidthProperty, width);

		    Owner.InvalidateMeasure();
		}
		
		public void Calculate(ScheduleColumn column, double change){
			if(column == null)
				throw new ArgumentNullException("column");
			
			if(!Owner.Columns.Contains(column))
				throw new ArgumentOutOfRangeException("column");
            
            var width = column.Width + change / (Owner.Columns.IndexOf(column) + 1);
            
            foreach (ScheduleColumn col in Owner.Columns)
                col.SetCurrentValue(ScheduleColumn.WidthProperty, width);

		    Owner.InvalidateMeasure();

			//Debug.WriteLine(string.Format("Calc {0} Real {1}", width, column.Width));
		}

        //public double TranslateFromSource(ScheduleColumn column)
        //{
        //    return TranslateFromSource((DateTime) column.Value);
        //}

        public double TranslateFromSource(object value)
        {
            double offset = 0;

            foreach (ScheduleColumn col in Owner.Columns)
            {
                if (!Equals(col.Value, value))
                    offset += col.Width;
                else
                    break;
            }

            return offset;
        }
        

        public IEnumerable<ScheduleColumn> GetVisibleColumns(Rect viewport)
        {
            return from col in Owner.Columns let yOffset = TranslateFromSource(col.Value) where yOffset < viewport.Right && yOffset + col.Width > viewport.Left select col;
        }

        public double GetDesiredWidth()
        {
            if (Owner == null)
                throw new InvalidOperationException("Kein Besitzer");
            if(!Owner.Columns.Any())
                throw new InvalidOperationException("Keine Spalten");

            return Owner.Columns.Count * Owner.Columns[0].Width;
        }

	    public object TranslateToSource(double horizontalValue)
	    {
            if (Owner.Columns.Any())
            {
                var colIdx = (int)Math.Round(horizontalValue / Owner.Columns[0].Width, 0);
                if(colIdx >= 0)
                    return Owner.Columns[colIdx].Value;

                //var interval = (TimeSpan)Owner.ColumnGenerator.Interval;
                //var end = (DateTime)Owner.ColumnGenerator.End;
                //var factor = interval.TotalSeconds / Owner.Columns[0].Width;
                //var seconds = horizontalValue * factor;
                ////seconds = Math.Round(seconds / 300, 0) * 300;
                //if (seconds < 0D) seconds = 0D;
                //if (seconds > interval.TotalSeconds) seconds = interval.TotalSeconds;
                //return TimeSpan.FromSeconds((int)Math.Round(seconds, 0));
            }

            return null;
	    }
	}
}
