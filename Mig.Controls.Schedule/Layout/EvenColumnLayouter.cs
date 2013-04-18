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

		}

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
                ScheduleColumn col = null;
                double offset = 0D;

                foreach (var c in Owner.Columns)
                {
                    offset += c.Width;

                    if (offset >= horizontalValue)
                        return c.Value;
                }

                //var colIdx = (int)Math.Floor(horizontalValue / Owner.Columns[0].Width);
                
                //if(colIdx < 0)
                //    colIdx = 0;
                //else
                //    if(colIdx >= Owner.Columns.Count)
                //        colIdx = Owner.Columns.Count-1;
                	
                //return Owner.Columns[colIdx].Value;
            }

            return null;
	    }
	}
}
