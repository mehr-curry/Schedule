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
using System.Diagnostics;
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

        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(ScheduleItem));
        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(ScheduleItem));
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(ScheduleItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, IsSelected_PropertyChanged));
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register("Bottom", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register("Right", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsSelected_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var instance = (ScheduleItem)sender;
            
            if ((bool)args.NewValue)
                instance.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, instance));
            else
                instance.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, instance));
        }

        protected virtual void OnSelected(RoutedEventArgs routedEventArgs)
        {
            RaiseEvent(routedEventArgs);
        }

        protected virtual void OnUnselected(RoutedEventArgs routedEventArgs)
	    {
	        RaiseEvent(routedEventArgs);
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
				value.DragStarted += gripper_DragStarted;
				value.DragDelta += gripper_DragDelta;
				value.DragCompleted += gripper_DragCompleted;
			}
		}

		private void UnWireGripper(Thumb value)
		{
			if (value != null) {
				value.DragStarted -= gripper_DragStarted;
				value.DragDelta -= gripper_DragDelta;
				value.DragCompleted -= gripper_DragCompleted;
			}
		}
		
		private void gripper_DragStarted(object sender, DragStartedEventArgs e)
		{
			_dragStartTop = Top;
			_activeGripper = sender as Thumb;
            Owner.Select(this, MouseButton.Left);
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
			if(Equals(_activeGripper, _resizeBottomGripper))
				SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
			else if(Equals(_activeGripper, _resizeTopGripper))
				SetCurrentValue(TopProperty, Top + e.VerticalChange);
			else if(Equals(_activeGripper, _moveGripper)){
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

				var mp = Mouse.GetPosition(this);
                Debug.WriteLine(Left + mp.X);
                
               	if (e.HorizontalChange > 0)
                {
//                    var lastRight = Right;
//                    SetCurrentValue(RightProperty, Right + e.HorizontalChange);
//                    if (lastRight != Right)
//                        SetCurrentValue(LeftProperty, Left + e.HorizontalChange);
                }
                else if (e.HorizontalChange < 0)
                {
//                    var lastLeft = Left;
//                    SetCurrentValue(LeftProperty, Left + e.HorizontalChange);
//                    if (lastLeft != Left)
//                        SetCurrentValue(RightProperty, Right + e.HorizontalChange);
                }

			}

			Owner.InvalidateArrange();
			e.Handled = true;
		}
		
		public Schedule Owner { get; set; }
		
		public double Left
        {
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}
		
        private static object Top_CoerceValue(DependencyObject sender, object value)
        {
            //if (value is double)
            //{
            //    var bottom = (double)sender.GetValue(BottomProperty);
            //    var top = (double)value;
                
            //    if (top >= bottom)
            //        return bottom;
            //}

            return value;
        }
		public double Top {
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}
		
        private static object Bottom_CoerceValue(DependencyObject sender, object value)
	    {
            //if (value is double)
            //{
            //    var top = (double)sender.GetValue(TopProperty);
            //    var bottom = (double)value;

            //    if (top >= bottom)
            //        return top;
            //}

            return value;
	    }

	    public double Bottom {
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            //Selector.SetIsSelected(this, !Selector.GetIsSelected(this));
            Owner.Select(this, MouseButton.Left);
            base.OnMouseUp(e);
        }
	}
}
