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
using System.Windows.Input;

using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
	[TemplatePart(Name = "PART_ResizeTop", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeLeft", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeRight", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeBottom", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_Copy", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_Move", Type=typeof(Thumb))]
	public class ScheduleItem : ContentControl
	{
		private double _dragStartTop = double.NaN;
		
		private Thumb _resizeTopGripper;
		private Thumb _resizeLeftGripper;
		private Thumb _resizeRightGripper;
		private Thumb _resizeBottomGripper;
		private Thumb _copyGripper;
		private Thumb _moveGripper;
		
		private Thumb _activeGripper;
		
		public ScheduleItem()
		{
		}

		public override void OnApplyTemplate()
		{
			UnWireGripper(_resizeTopGripper);
			UnWireGripper(_resizeLeftGripper);
			UnWireGripper(_resizeRightGripper);
			UnWireGripper(_resizeBottomGripper);
			UnWireGripper(_copyGripper);
			UnWireGripper(_moveGripper);
			
			_resizeTopGripper = (Thumb)Template.FindName("PART_ResizeTop", this);
			_resizeLeftGripper = (Thumb)Template.FindName("PART_ResizeLeft", this);
			_resizeRightGripper = (Thumb)Template.FindName("PART_ResizeRight", this);
			_resizeBottomGripper = (Thumb)Template.FindName("PART_ResizeBottom", this);
			_copyGripper = (Thumb)Template.FindName("PART_Copy", this);
			_moveGripper = (Thumb)Template.FindName("PART_Move", this);
			
			WireGripper(_resizeTopGripper);
			WireGripper(_resizeLeftGripper);
			WireGripper(_resizeRightGripper);
			WireGripper(_resizeBottomGripper);
			WireGripper(_copyGripper);
			WireGripper(_moveGripper);
			
			base.OnApplyTemplate();
		}
		
//		public Thumb ResizeTopGripper
//		{
//			get{ return _resizeTopGripper; }
//			set{
//				UnWireGripper(_resizeTopGripper);
//				_resizeTopGripper = value;
//				WireGripper(_resizeTopGripper);
//			}
//		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(e.Key == Key.Escape && _activeGripper != null && _activeGripper.IsDragging){
				_activeGripper.CancelDrag();
				e.Handled = true;
			}
				
			base.OnKeyDown(e);
		} 

		private void WireGripper(Thumb value)
		{
			if (value != null) {
				value.DragStarted += new DragStartedEventHandler(gripper_DragStarted);
				value.DragDelta += new DragDeltaEventHandler(gripper_DragDelta);
				value.DragCompleted += new DragCompletedEventHandler(gripper_DragCompleted);
			}
		}

		private void UnWireGripper(Thumb value)
		{
			if (value != null) {
				value.DragStarted -= new DragStartedEventHandler(gripper_DragStarted);
				value.DragDelta -= new DragDeltaEventHandler(gripper_DragDelta);
				value.DragCompleted -= new DragCompletedEventHandler(gripper_DragCompleted);
			}
		}
		
		private void gripper_DragStarted(object sender, DragStartedEventArgs e)
		{
			_dragStartTop = Top;
			_activeGripper = sender as Thumb;
		}

		private void gripper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if(e.Canceled && !double.IsNaN(_dragStartTop))
			{
				// TODO handling
			}
			
			_activeGripper = null;
			_dragStartTop = double.NaN;
		}

		private void gripper_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if(_activeGripper == _resizeBottomGripper)
				SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
			else if(_activeGripper == _resizeTopGripper)
				SetCurrentValue(TopProperty, Top + e.VerticalChange);
			else if(_activeGripper == _moveGripper){
				if(e.VerticalChange > 0){
					var lastBottom = Bottom;
					SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
					if(lastBottom != Bottom)
						SetCurrentValue(TopProperty, Top + e.VerticalChange);
				}
				else if (e.VerticalChange < 0 ){
					var lastTop = Top;
					SetCurrentValue(TopProperty, Top + e.VerticalChange);
					if(lastTop != Top)
						SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
				}
			}

			Owner.InvalidateArrange();
			e.Handled = true;
		}
		
		public Schedule Owner { get; set; }
		
		public static readonly DependencyProperty LeftProperty =
			DependencyProperty.Register("Left", typeof(double), typeof(ScheduleItem),
			                            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
		
		public double Left {
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}
		
		public static readonly DependencyProperty TopProperty =
			DependencyProperty.Register("Top", typeof(double), typeof(ScheduleItem),
			                            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
		
		public double Top {
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}
		
		public static readonly DependencyProperty BottomProperty =
			DependencyProperty.Register("Bottom", typeof(double), typeof(ScheduleItem),
			                            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
		
		public double Bottom {
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		
		public static readonly DependencyProperty RightProperty =
			DependencyProperty.Register("Right", typeof(double), typeof(ScheduleItem),
			                            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
		
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}
	}
}
