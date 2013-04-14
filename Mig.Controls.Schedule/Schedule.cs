/*
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
using Mig.Controls.Schedule.Converter;
using Mig.Controls.Schedule.Layout;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of Schedule.
	/// </summary>
    [TemplatePart(Name = "PART_HorizontalHeaderHost", Type = typeof(ScheduleColumnHeaderPresenter))]
    [TemplatePart(Name = "PART_VerticalHeaderHost", Type = typeof(ScheduleRowHeaderPresenter))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ScheduleVirtualizingPanel))]
    [TemplatePart(Name = "PART_TopLeft", Type = typeof(FrameworkElement))]
    public class Schedule : MultiSelector, IScrollInfo
	{
        private Panel _itemsHost;
        private ItemsControl _horizontalHeaderHost;
        private ItemsControl _verticalHeaderHost;
        private FrameworkElement _topLeft;
	    private readonly TranslateTransform _translate = new TranslateTransform();

        public Schedule()
		{
            Columns = new ObservableCollection<ScheduleColumn>();
            ColumnLayouter = new EvenColumnLayouter() { Owner = this };
            ColumnGenerator = new ColumnGenerator<DateTime>() { Start = DateTime.Today, Interval = new TimeSpan(1, 0, 0, 0), End = DateTime.Today.AddDays(3) };

            Rows = new ObservableCollection<ScheduleRow>();
            RowLayouter = new EvenRowLayouter() { Owner = this };
            RowGenerator = new RowGenerator<TimeSpan>() { Start = new TimeSpan(0, 0, 0), Interval = new TimeSpan(1, 0, 0), End = new TimeSpan(24, 0, 0) };

            RenderTransform = _translate;
		}

        public override void OnApplyTemplate()
        {
            _itemsHost = (Panel)Template.FindName("PART_ItemsHost", this);
            _topLeft = (FrameworkElement)Template.FindName("PART_TopLeft", this);
            _horizontalHeaderHost = (ItemsControl)Template.FindName("PART_HorizontalHeaderHost", this);
            _verticalHeaderHost = (ItemsControl)Template.FindName("PART_VerticalHeaderHost", this);

            if (Rows.Count == 0)
                Rows = RowGenerator.Generate();

            if (Columns.Count == 0)
                Columns = ColumnGenerator.Generate();

//            OnSelectiveScrollingOrientationChanged(_horizontalHeaderHost, SelectiveScrollingOrientation.Horizontal);
//            OnSelectiveScrollingOrientationChanged(_verticalHeaderHost, SelectiveScrollingOrientation.Vertical);
//            OnSelectiveScrollingOrientationChanged(_topLeft, SelectiveScrollingOrientation.None);
        
            base.OnApplyTemplate();
        }

		protected override DependencyObject GetContainerForItemOverride()
		{
			var item = new ScheduleItem() {Owner=this};
			BindingOperations.SetBinding(item, ScheduleItem.TopProperty, new Binding("VerticalStartValue"){Converter=new TimeSpanLayoutConverter(), ConverterParameter=item});
			BindingOperations.SetBinding(item, ScheduleItem.BottomProperty, new Binding("VerticalEndValue"){Converter=new TimeSpanLayoutConverter(), ConverterParameter=item});
			return item;
		}

	    public string HorizontalValueMember { get; set; }
	    public string VerticalValueMember { get; set; }
	    public IColumnLayouter ColumnLayouter { get; set; }
        public IRowLayouter RowLayouter { get; set; }
        public ObservableCollection<ScheduleColumn> Columns { get; set; }
		public ObservableCollection<ScheduleRow> Rows { get; set; }
        public IRowGenerator RowGenerator { get; set; }
        public IColumnGenerator ColumnGenerator { get; set; }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            InvalidateScrollInfo(arrangeBounds);

            _topLeft.Arrange(new Rect(_topLeft.DesiredSize));
            _horizontalHeaderHost.Arrange(new Rect(new Point(50, 0), new Size(ColumnLayouter.GetDesiredWidth(), 20)));
            _verticalHeaderHost.Arrange(new Rect(new Point(0, 20), new Size(50, RowLayouter.GetDesiredHeight())));
            _itemsHost.Arrange(new Rect(new Point(50, 20), new Size(ColumnLayouter.GetDesiredWidth(), RowLayouter.GetDesiredHeight())));
            
//            Debug.WriteLine(string.Format("{0} {1} {2}", new Size(_topLeft.Width, _topLeft.Height), new Size(_topLeft.ActualWidth, _topLeft.ActualHeight), _topLeft.DesiredSize));
        	return arrangeBounds;
        }


        public void InvalidateScrollInfo()
        {
            InvalidateScrollInfo(_viewport);
        }
	    
	    private void InvalidateScrollInfo(Size constraint)
	    {
            var extent = new Size(ColumnLayouter.GetDesiredWidth() + _topLeft.ActualWidth,
                                  RowLayouter.GetDesiredHeight() + _topLeft.ActualHeight);

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
	    }

	    protected override Size MeasureOverride(Size constraint)
	    {
	        InvalidateScrollInfo(constraint);

            _topLeft.Measure(constraint);
            _horizontalHeaderHost.Measure(new Size(ColumnLayouter.GetDesiredWidth(), _topLeft.ActualHeight));
            _verticalHeaderHost.Measure(new Size(_topLeft.ActualWidth, RowLayouter.GetDesiredHeight()));
            _itemsHost.Measure(new Size(_extent.Width > constraint.Width ? constraint.Width : _extent.Width,
                                        _extent.Height > constraint.Height ? constraint.Height : _extent.Height));

//            Debug.WriteLine(String.Format("{0} {1}", constraint, _extent));

	        return constraint; // new Size(ColumnLayouter.GetDesiredWidth() + _topLeft.ActualWidth, RowLayouter.GetDesiredHeight() + _topLeft.ActualHeight);
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
            SetHorizontalOffset(HorizontalOffset - _viewport.Width * 0.1); // ColumnWidth / 2 ?
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + _viewport.Width * 0.1); // ColumnWidth / 2 ?
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
            SetHorizontalOffset(HorizontalOffset - _viewport.Width * 0.2); // ColumnWidth ?
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + _viewport.Width * 0.2); // ColumnWidth ?
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
            SetHorizontalOffset(HorizontalOffset - _viewport.Width); // ColumnWidth / 2 ?
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + _viewport.Width); // ColumnWidth / 2 ?
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
