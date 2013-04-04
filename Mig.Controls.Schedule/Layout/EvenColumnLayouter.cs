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
		
		public void Calculate(ScheduleColumn column, double change){
			if(column == null)
				throw new ArgumentNullException("column");
			
			if(!Owner.Columns.Contains(column))
				throw new ArgumentOutOfRangeException("column");
			
			column.SetCurrentValue(ScheduleColumn.WidthProperty, column.Width + change / (Owner.Columns.IndexOf(column)+1));
			
			foreach(ScheduleColumn col in Owner.Columns)
				if(col != column)
					col.Width = column.Width;
		}

	    public double GetOffset(ScheduleColumn column)
	    {
	        double offset = 0;

	        foreach (ScheduleColumn col in Owner.Columns)
	        {
	            if (col != column)
	                offset += col.Width;
                else 
                    break;
	        }

	        return offset;
	    }

        public IEnumerable<ScheduleColumn> GetVisibleColumns(Rect viewport)
        {
            return from col in Owner.Columns let yOffset = GetOffset(col) where yOffset < viewport.Right && yOffset + col.Width > viewport.Left select col;
        }
	}
}
