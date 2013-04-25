/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Mig.Controls.Schedule
{
	[TemplatePart(Name="PART_RightGripper", Type=typeof(Thumb))]
	public class ScheduleColumnHeader : ToggleButton
	{
		private Thumb _rightGripper;
		private ScheduleColumn _column;
		private double _dragStartWidth = double.NaN;
		private Schedule _owner;

	    public ScheduleColumnHeader()
		{
            //Width = 50D;
		}
		
		public ScheduleColumn Column
		{ 
			get
			{
				return _column;
			} 
			set
			{
				if(_column != null)
                    BindingOperations.ClearBinding(this, WidthProperty);
				
				_column = value;
				
				if(_column != null)
					BindingOperations.SetBinding(this, WidthProperty, new Binding("Width"){Source=_column});
			} 
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(e.Key == Key.Escape && RightGripper != null && RightGripper.IsDragging){
				RightGripper.CancelDrag();
				e.Handled = true;
			}
				
			base.OnKeyDown(e);
		} 
		
		public override void OnApplyTemplate()
		{
			RightGripper = Template.FindName("PART_RightGripper", this) as Thumb;
			
			base.OnApplyTemplate();
		}
		
		public Schedule Owner 
		{ 
			get { return _owner ?? (_owner = Helper.FindParent<Schedule>(this)); }
		}
		
		public Thumb RightGripper 
		{ 
			get{ return _rightGripper; }
			set{
				if(_rightGripper != null)
				{
					_rightGripper.DragStarted -= new DragStartedEventHandler(_rightGripper_DragStarted);
					_rightGripper.DragDelta -= new DragDeltaEventHandler(_rightGripper_DragDelta);
					_rightGripper.DragCompleted -= new DragCompletedEventHandler(_rightGripper_DragCompleted);
				}
				
				_rightGripper = value;
				
				if(_rightGripper != null)
				{
					_rightGripper.DragStarted += new DragStartedEventHandler(_rightGripper_DragStarted);
					_rightGripper.DragDelta += new DragDeltaEventHandler(_rightGripper_DragDelta);
					_rightGripper.DragCompleted += new DragCompletedEventHandler(_rightGripper_DragCompleted);
				}
			}
		}

		private void _rightGripper_DragStarted(object sender, DragStartedEventArgs e)
		{
			if(!IsKeyboardFocused)
				Focus();
					
			_dragStartWidth = Width;
		}

        private void _rightGripper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if(Owner != null && Owner.ColumnLayouter != null && e.Canceled && !double.IsNaN(_dragStartWidth))
				Owner.ColumnLayouter.SetAll(_dragStartWidth);
				
			_dragStartWidth = double.NaN;
		}

        private void _rightGripper_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if(Column != null && Owner != null && Owner.ColumnLayouter != null)
				Owner.ColumnLayouter.Calculate(Column, e.HorizontalChange);

			e.Handled = true;
		}
	}
}
