/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 22.04.2013
 * Time: 07:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Manipulation
{
	/// <summary>
	/// Description of MoveManipulationBehavior.
	/// </summary>
	public class MoveManipulator : IManipulatorBehavior
	{
	    public Point? StartPoint { get; set; }

        public bool Manipulate(Point mp, ScheduleItem item)
        {
            var original = item.Tag as ScheduleItem;

            var r = new Rect(original.Left + mp.X, original.Top + mp.Y, item.ActualWidth, item.ActualHeight);

            if (r.Left < 0)
                item.SetCurrentValue(ScheduleItem.LeftProperty, 0D);
            else if (r.Right > item.Owner.ColumnLayouter.GetDesiredWidth())
                item.SetCurrentValue(ScheduleItem.LeftProperty, item.Owner.ColumnLayouter.GetDesiredWidth() - item.ActualWidth);
            else
            {
                item.SetCurrentValue(ScheduleItem.LeftProperty, r.Left);
                //if(Column.ItemAlignment != Alignment.Full)
                item.SetCurrentValue(ScheduleItem.RightProperty, r.Right);
            }

            if (r.Top < 0)
                item.SetCurrentValue(ScheduleItem.TopProperty, 0D);
            else if (r.Bottom > item.Owner.RowLayouter.GetDesiredHeight())
                item.SetCurrentValue(ScheduleItem.TopProperty, item.Owner.RowLayouter.GetDesiredHeight() - item.ActualHeight);
            else
            {
//            	if (mp.Y > 0)
//			    {
//			        var lastBottom = item.Bottom;
//			        item.SetCurrentValue(ScheduleItem.BottomProperty, r.Y);
//			        if (lastBottom != item.Bottom)
//			            item.SetCurrentValue(ScheduleItem.TopProperty, r.Y);
//			    }
//			    else if (mp.Y < 0)
//			    {
//			        var lastTop = item.Top;
//			        item.SetCurrentValue(ScheduleItem.TopProperty, r.Y);
//			        if (lastTop != item.Top)
//			            item.SetCurrentValue(ScheduleItem.BottomProperty, r.Y);
//			    }
			    
                item.SetCurrentValue(ScheduleItem.TopProperty, r.Top);
                item.SetCurrentValue(ScheduleItem.BottomProperty, r.Bottom);
            }

  			
		  

            Debug.WriteLine(r);
			return false;
		}
	}
}
