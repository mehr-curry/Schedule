/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 20:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Mig.Controls.Schedule
{
    [TemplatePart(Name = "PART_BottomGripper", Type = typeof(Thumb))]
    public class ScheduleRowHeader : ButtonBase
	{
		private Thumb _bottomGripper;
		private ScheduleRow _row;
		private double _dragStartHeight = double.NaN;
		private Schedule _owner;
		
		public ScheduleRowHeader()
		{
            //Height = 50D;
		}
		
		public ScheduleRow Row
		{ 
			get
			{
				return _row;
			} 
			set
			{
				if(_row != null)
					BindingOperations.ClearBinding(this, HeightProperty);
				
				_row = value;
				
				if(_row != null)
					BindingOperations.SetBinding(this, HeightProperty, new Binding("Height"){Source=_row});
			} 
		}
		
		public override void OnApplyTemplate()
		{
			RightGripper = Template.FindName("PART_BottomGripper", this) as Thumb;
			
			base.OnApplyTemplate();
		}
		
		public Schedule Owner 
		{ 
			get { return _owner ?? (_owner = Helper.FindParent<Schedule>(this)); }
		}
		
		public Thumb RightGripper 
		{ 
			get{ return _bottomGripper; }
			set{
				if(_bottomGripper != null)
				{
					_bottomGripper.DragStarted -= new DragStartedEventHandler(_bottomGripper_DragStarted);
					_bottomGripper.DragDelta -= new DragDeltaEventHandler(_bottomGripper_DragDelta);
					_bottomGripper.DragCompleted -= new DragCompletedEventHandler(_bottomGripper_DragCompleted);
				}
				
				_bottomGripper = value;
				
				if(_bottomGripper != null)
				{
					_bottomGripper.DragStarted += new DragStartedEventHandler(_bottomGripper_DragStarted);
					_bottomGripper.DragDelta += new DragDeltaEventHandler(_bottomGripper_DragDelta);
					_bottomGripper.DragCompleted += new DragCompletedEventHandler(_bottomGripper_DragCompleted);
				}
			}
		}

		private void _bottomGripper_DragStarted(object sender, DragStartedEventArgs e)
		{
			_dragStartHeight = Height;
		}

        private void _bottomGripper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			
			if(Row != null && e.Canceled && !double.IsNaN(_dragStartHeight))
				Row.SetCurrentValue(ScheduleRow.HeightProperty, _dragStartHeight);
				
			_dragStartHeight = double.NaN;
		}

        private void _bottomGripper_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if(Row != null && Owner != null && Owner.RowLayouter != null)
				Owner.RowLayouter.Calculate(Row, e.VerticalChange);

			e.Handled = true;
		}
	}
}
