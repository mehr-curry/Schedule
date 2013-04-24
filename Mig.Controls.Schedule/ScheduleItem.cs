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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using Mig.Controls.Schedule.Interfaces;
using Mig.Controls.Schedule.Manipulation;

namespace Mig.Controls.Schedule
{
	[TemplatePart(Name = "PART_ResizeTop", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeLeft", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeRight", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeBottom", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_Copy", Type=typeof(Thumb))]
	[TemplatePart(Name = "PART_Move", Type=typeof(Thumb))]
    public class ScheduleItem : ContentControl, ICloneable
	{
		private Point? _dragStartPoint;
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
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange ));

	    public static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register("Bottom", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register("Right", typeof(double), typeof(ScheduleItem),
                                        new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentArrange));

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
			_activeGripper = (Thumb)sender;
            if(!IsSelected)
		        Owner.Select(this);
            Owner.StartBehavior(this, ManipulatorPropertyExt.GetManipulator((UIElement)sender));
		}

		private void gripper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Owner.StopBehavior(this, e.Canceled);		    
			_activeGripper = null;
		}

		private void gripper_DragDelta(object sender, DragDeltaEventArgs e)
		{
		    Owner.ProcessBehavior(this);
//			if(Equals(_activeGripper, _resizeBottomGripper))
//				SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
//			else if(Equals(_activeGripper, _resizeTopGripper))
//				SetCurrentValue(TopProperty, Top + e.VerticalChange);
//			else if(Equals(_activeGripper, _moveGripper))
//			{
//				var mp = Mouse.GetPosition(this);
//				var r = new Rect(Left + mp.X, Top + mp.Y, ActualWidth, ActualHeight);
//				
//			    if (r.Left < 0)
//                    SetCurrentValue(LeftProperty, 0D);
//                else if (r.Right > Owner.ColumnLayouter.GetDesiredWidth())
//                    SetCurrentValue(LeftProperty, Owner.ColumnLayouter.GetDesiredWidth() - ActualWidth);
//                else
//                {
//                    SetCurrentValue(LeftProperty, r.Left);
//                    //if(Column.ItemAlignment != Alignment.Full)
//                        SetCurrentValue(RightProperty, r.Right);
//                }
//
//			    if (r.Top < 0)
//                    SetCurrentValue(TopProperty, 0D);
//                else if (r.Bottom > Owner.RowLayouter.GetDesiredHeight())
//                    SetCurrentValue(TopProperty, Owner.RowLayouter.GetDesiredHeight() - ActualHeight);
//                else
//                {
//                    SetCurrentValue(TopProperty, r.Top);
//                    SetCurrentValue(BottomProperty, r.Bottom);
//                }
//
//			    Debug.WriteLine(r);

		    //if (e.VerticalChange > 0)
		    //{
		    //    var lastBottom = Bottom;
		    //    SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
		    //    if (lastBottom != Bottom)
		    //        SetCurrentValue(TopProperty, Top + e.VerticalChange);
		    //}
		    //else if (e.VerticalChange < 0)
		    //{
		    //    var lastTop = Top;
		    //    SetCurrentValue(TopProperty, Top + e.VerticalChange);
		    //    if (lastTop != Top)
		    //        SetCurrentValue(BottomProperty, Bottom + e.VerticalChange);
		    //}

		    //if (e.HorizontalChange > 0)
		    //{
		    //    var lastRight = Right;
		    //    SetCurrentValue(RightProperty, Right + e.HorizontalChange);
		    //    if (lastRight != Right)
		    //        SetCurrentValue(LeftProperty, Left + e.HorizontalChange);
		    //}
		    //else if (e.HorizontalChange < 0)
		    //{
		    //    var lastLeft = Left;
		    //    SetCurrentValue(LeftProperty, Left + e.HorizontalChange);
		    //    if (lastLeft != Left)
		    //        SetCurrentValue(RightProperty, Right + e.HorizontalChange);
		    //}

		    //var mp = Mouse.GetPosition(this);

		    //if (e.VerticalChange != 0)
		    //{
		    //    if (Owner.RowLayouter.GetDesiredHeight() > Top + mp.Y &&
		    //         Top + mp.Y > 0)
		    //    {
		    //        var lastTop = Top;
		    //        SetCurrentValue(TopProperty, Top + mp.Y);
		    //        if (lastTop != Top)
		    //            SetCurrentValue(BottomProperty, Bottom + mp.Y);
		    //    }
		    //}

		    //if (e.HorizontalChange != 0)
		    //{
		    //    if (Owner.ColumnLayouter.GetDesiredWidth() > Left + mp.X &&
		    //         Left + mp.X > 0)
		    //    {
		    //        var lastLeft = Left;
		    //        SetCurrentValue(LeftProperty, Left + mp.X);
		    //        if (lastLeft != Left)
		    //            SetCurrentValue(RightProperty, Left + mp.X + ActualWidth);
		    //    }
		    //}
//			}
//
//			Owner.InvalidateArrange();
//			e.Handled = true;
		}
		
		public Schedule Owner { get; set; }
		
        //private static object Top_CoerceValue(DependencyObject sender, object value)
        //{
        //    if (value is double)
        //    {
        //        var bottom = (double)sender.GetValue(BottomProperty);
        //        var top = (double)value;

        //        if (top < 0D)
        //            return 0D;

        //        if (top >= bottom)
        //            return bottom;
        //    }

        //    return value;
        //}
        //private static object Bottom_CoerceValue(DependencyObject sender, object value)
        //{
        //    if (value is double)
        //    {
        //        var instance = (ScheduleItem)sender;
        //        var top = (double)sender.GetValue(TopProperty);
        //        var bottom = (double)value;

        //        if (bottom > instance.Owner.RowLayouter.GetDesiredHeight())
        //            return instance.Owner.RowLayouter.GetDesiredHeight();
                
        //        if (top >= bottom)
        //            return top;
        //    }

        //    return value;
        //}

        //private static object Left_CoerceValue(DependencyObject sender, object value)
        //{
        //    if (value is double)
        //    {
        //        var right = (double)sender.GetValue(RightProperty);
        //        var left = (double)value;

        //        if (left < 0D)
        //            return 0D;

        //        if (left >= right)
        //            return right;
        //    }

        //    return value;
        //}

        //private static object Right_CoerceValue(DependencyObject sender, object value)
        //{
        //    if (value is double)
        //    {
        //        var instance = (ScheduleItem)sender;
        //        var left = (double)sender.GetValue(LeftProperty);
        //        var right = (double)value;

        //        if (right > instance.Owner.ColumnLayouter.GetDesiredWidth())
        //            return instance.Owner.ColumnLayouter.GetDesiredWidth();

        //        if (left >= right)
        //            return left;
        //    }

        //    return value;
        //}

        //private static void Top_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    d.CoerceValue(BottomProperty);
        //}

        //private static void Bottom_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    d.CoerceValue(TopProperty);
        //}

	    

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public double Bottom
        {
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}

        //public ScheduleColumn Column{ get; set; }

	    protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            //Selector.SetIsSelected(this, !Selector.GetIsSelected(this));
	        switch (e.ChangedButton)
	        {
                case MouseButton.Left:
	                Owner.Select(this);
                    break;
	        }

	        base.OnMouseUp(e);
        }

        object ICloneable.Clone()
        {
            var copy = new ScheduleItem
            {
                Left = this.Left,
                Top = this.Top,
                Right = this.Right,
                Bottom = this.Bottom,
                Height = this.ActualHeight,
                Width = this.ActualWidth,
                Owner = this.Owner,
                Content = this.Content
            };

            //BindingOperations.SetBinding(copy, ScheduleItem.LeftProperty, new Binding("HorizontalStartValue") { Converter = Owner._hsConverter, ConverterParameter = copy });
            //BindingOperations.SetBinding(copy, ScheduleItem.RightProperty, new Binding("HorizontalEndValue") { Converter = Owner._heConverter, ConverterParameter = copy });
            BindingOperations.SetBinding(copy, ScheduleItem.TopProperty, new Binding("VerticalStartValue") { Converter = Owner._vsConverter, ConverterParameter = copy });
            BindingOperations.SetBinding(copy, ScheduleItem.BottomProperty, new Binding("VerticalEndValue") { Converter = Owner._veConverter, ConverterParameter = copy });
            
            return copy;
        }

        public ScheduleItem Clone()
        {
            return (ScheduleItem)((ICloneable)this).Clone();
        }
    }
}
