/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 21:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
	public class ScheduleGridItem : ContentControl
	{
		private double _dragStartWidth = double.NaN;
		private Thumb _resizeTopGripper;
		
		public ScheduleGridItem()
		{
		}

		public override void OnApplyTemplate()
		{
			ResizeTopGripper = (Thumb)Template.FindName("PART_ResizeTop", this);
			
			base.OnApplyTemplate();
		}
		
		public Thumb ResizeTopGripper
		{
			get{ return _resizeTopGripper; }
			set{
				
				UnWireGripper(_resizeTopGripper);
				
				_resizeTopGripper = value;
				
				WireGripper(_resizeTopGripper);
			}
		}

		void WireGripper(Thumb value)
		{
			if (value != null) {
				value.DragStarted += new DragStartedEventHandler(gripper_DragStarted);
				value.DragDelta += new DragDeltaEventHandler(gripper_DragDelta);
				value.DragCompleted += new DragCompletedEventHandler(gripper_DragCompleted);
			}
		}

		void UnWireGripper(Thumb value)
		{
			if (value != null) {
				value.DragStarted -= new DragStartedEventHandler(gripper_DragStarted);
				value.DragDelta -= new DragDeltaEventHandler(gripper_DragDelta);
				value.DragCompleted -= new DragCompletedEventHandler(gripper_DragCompleted);
			}
		}
		
		private void gripper_DragStarted(object sender, DragStartedEventArgs e)
		{
			_dragStartWidth = Width;
		}

		private void gripper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			
			if(Column != null && e.Canceled && !double.IsNaN(_dragStartWidth))
				Column.SetCurrentValue(ScheduleColumn.WidthProperty, _dragStartWidth);
			
			_dragStartWidth = double.NaN;
		}

		private void gripper_DragDelta(object sender, DragDeltaEventArgs e)
		{
			
			Height = ActualHeight + e.HorizontalChange;

			e.Handled = true;
		}
		
		public ScheduleColumn Column { get; set; }
	}
}
