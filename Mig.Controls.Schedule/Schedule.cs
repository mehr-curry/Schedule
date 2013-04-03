﻿/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Mig.Controls.Schedule.Layout;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of Schedule.
	/// </summary>
    [TemplatePart(Name = "PART_HorizontalHeaderHost", Type = typeof(ScheduleColumnHeaderPresenter))]
    [TemplatePart(Name = "PART_VerticalHeaderHost", Type = typeof(ScheduleRowHeaderPresenter))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ScheduleVirtualizingGrid))]
    [TemplatePart(Name = "PART_TopLeft", Type = typeof(FrameworkElement))]
    public class Schedule : MultiSelector, IScrollInfo
	{
        private Panel _itemsHost;
        private ItemsControl _horizontalHeaderHost;
        private ItemsControl _verticalHeaderHost;
        private FrameworkElement _topLeft;
	    private TranslateTransform _translate = new TranslateTransform();

        public Schedule()
		{
			Columns = new ObservableCollection<ScheduleColumn>();
            Rows = new ObservableCollection<ScheduleRow>();
            ColumnLayouter = new EvenColumnLayouter() { Owner = this };
		    RowLayouter = new EvenRowLayouter() {Owner = this};
            RenderTransform = _translate;
            //ColumnLayouter = new PromotingColumnLayouter() {Owner = this};
		}

        public override void OnApplyTemplate()
        {
            _itemsHost = (Panel)Template.FindName("PART_ItemsHost", this);
            _topLeft = (FrameworkElement)Template.FindName("PART_TopLeft", this);
            _horizontalHeaderHost = (ItemsControl)Template.FindName("PART_HorizontalHeaderHost", this);
            _verticalHeaderHost = (ItemsControl)Template.FindName("PART_VerticalHeaderHost", this);
            var topLeft = (Border)Template.FindName("PART_TopLeft", this);

            OnSelectiveScrollingOrientationChanged(_horizontalHeaderHost, SelectiveScrollingOrientation.Horizontal);
            OnSelectiveScrollingOrientationChanged(_verticalHeaderHost, SelectiveScrollingOrientation.Vertical);
            OnSelectiveScrollingOrientationChanged(topLeft, SelectiveScrollingOrientation.None);
        
            base.OnApplyTemplate();
        }
		
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ScheduleGridItem();
		} 
		
		public IColumnLayouter ColumnLayouter { get; set; }
        public IRowLayouter RowLayouter { get; set; }
        public ObservableCollection<ScheduleColumn> Columns { get; set; }
		public ObservableCollection<ScheduleRow> Rows { get; set; }


        /// <summary>In Anlehnung an das SelectiveScrollingGrid, welches irgendwie nur funktioniert, wenn es in einem Template verwendet wird.</summary>
        private static void OnSelectiveScrollingOrientationChanged(FrameworkElement uIElement, SelectiveScrollingOrientation selectiveScrollingOrientation)
        {
            var scrollViewer = Helper.FindParent<ScrollViewer>(uIElement);
            if (scrollViewer != null && uIElement != null)
            {
                var renderTransform = uIElement.RenderTransform;

                if (renderTransform != null)
                {
                    BindingOperations.ClearBinding(renderTransform, TranslateTransform.XProperty);
                    BindingOperations.ClearBinding(renderTransform, TranslateTransform.YProperty);
                }

                if (selectiveScrollingOrientation == SelectiveScrollingOrientation.Both)
                {
                    uIElement.RenderTransform = null;
                    return;
                }

                var tt = new TranslateTransform();
                if (selectiveScrollingOrientation != SelectiveScrollingOrientation.Horizontal)
                    BindingOperations.SetBinding(tt, TranslateTransform.XProperty, new Binding("ContentHorizontalOffset") {Source = scrollViewer});
                
                if (selectiveScrollingOrientation != SelectiveScrollingOrientation.Vertical)
                    BindingOperations.SetBinding(tt, TranslateTransform.YProperty, new Binding("ContentVerticalOffset") {Source = scrollViewer});

                uIElement.RenderTransform = tt;
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {

            var extent = new Size(Columns.Count * Columns[0].Width + _topLeft.ActualWidth, Rows.Count * Rows[0].Height + _topLeft.ActualHeight);

            if (_extent != extent)
            {
                _extent = extent;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            if (_viewport != arrangeBounds)
            {
                _viewport = arrangeBounds;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            _itemsHost.Arrange(new Rect(_extent));
            _horizontalHeaderHost.Arrange(new Rect(_extent));
            _verticalHeaderHost.Arrange(new Rect(_extent));
            _topLeft.Arrange(new Rect(_extent));

            return base.ArrangeOverride(_extent);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var extent = new Size(Columns.Count * Columns[0].Width + _topLeft.ActualWidth, Rows.Count * Rows[0].Height + _topLeft.ActualHeight);

            if (_extent != extent)
            {
                _extent = extent;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            if (_viewport != constraint)
            {
                _viewport = constraint;
                if (ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }

            _horizontalHeaderHost.Measure(constraint);
            _verticalHeaderHost.Measure(constraint);
            _itemsHost.Measure(constraint);
            _topLeft.Measure(constraint);

            return base.MeasureOverride(constraint);
        }

        public bool CanHorizontallyScroll { get; set; }

        public bool CanVerticallyScroll { get; set; }

        public double ExtentHeight
        {
            get { return _extent.Height; }
        }

        public double ExtentWidth
        {
            get { return _extent.Width; }
        }

        public double HorizontalOffset
        {
            get { return _offset.X; }
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + _viewport.Height * 0.1); // RowHeight / 2 ?
        }

        public void LineLeft()
        {
            SetVerticalOffset(HorizontalOffset - _viewport.Width * 0.1); // ColumnWidth / 2 ?
        }

        public void LineRight()
        {
            SetVerticalOffset(HorizontalOffset + _viewport.Width * 0.1); // ColumnWidth / 2 ?
        }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - _viewport.Height * 0.1); // RowHeight / 2 ?
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
            //throw new NotImplementedException();
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + _viewport.Height * 0.2); // RowHeight?
        }

        public void MouseWheelLeft()
        {
            SetVerticalOffset(HorizontalOffset - _viewport.Width * 0.2); // ColumnWidth ?
        }

        public void MouseWheelRight()
        {
            SetVerticalOffset(HorizontalOffset + _viewport.Width * 0.2); // ColumnWidth ?
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - _viewport.Height * 0.2); // RowHeight?
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + _viewport.Height);
        }

        public void PageLeft()
        {
            SetVerticalOffset(HorizontalOffset - _viewport.Width); // ColumnWidth / 2 ?
        }

        public void PageRight()
        {
            SetVerticalOffset(HorizontalOffset + _viewport.Width); // ColumnWidth / 2 ?
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - _viewport.Height);
        }

	    public ScrollViewer ScrollOwner { get; set; }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || _viewport.Width >= _extent.Width)
                offset = 0;
            else
                if (offset + _viewport.Width >= _extent.Width)
                    offset = _extent.Width - _viewport.Width;

            _offset.X = offset;

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            _translate.X = -offset;
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
                offset = 0;
            else
                if (offset + _viewport.Height >= _extent.Height)
                    offset = _extent.Height - _viewport.Height;

            _offset.Y = offset;

            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();

            _translate.Y = -offset;
        }

        public double VerticalOffset
        {
            get { return _offset.Y; }
        }

        public double ViewportHeight
        {
            get { return _viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return _viewport.Width; }
        }

        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;
    }
}