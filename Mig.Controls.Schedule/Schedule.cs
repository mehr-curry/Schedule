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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Mig.Controls.Schedule.Converter;
using Mig.Controls.Schedule.Layout;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
    /// <summary>
    /// Description of Schedule.
    /// </summary>
    [TemplatePart(Name = "PART_HorizontalHeaderHost", Type = typeof(ScheduleColumnHeaderPresenter))]
    [TemplatePart(Name = "PART_VerticalHeaderHost", Type = typeof(ScheduleRowHeaderPresenter))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ScheduleVirtualizingPanel))]
    [TemplatePart(Name = "PART_TopLeft", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_SelectionFrame", Type = typeof(Border))]
    public class Schedule : MultiSelector, IScrollInfo
    {
        private ScheduleVirtualizingPanel _itemsHost;
        private ItemsControl _horizontalHeaderHost;
        private ItemsControl _verticalHeaderHost;
        private FrameworkElement _topLeft;
        private Panel _overlay;

		private readonly List<ScheduleItem> _workingCopies = new List<ScheduleItem>();
        private IManipulatorBehavior _activeManipulator;
        private Border _selectionFrame;
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;
        private readonly TranslateTransform _translate = new TranslateTransform();
        
        internal IValueConverter _hsConverter;
        internal IValueConverter _heConverter;
        internal IValueConverter _vsConverter;
        internal IValueConverter _veConverter;

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(Schedule), new UIPropertyMetadata(SelectionMode.Single));

        public Schedule()
        {
            Columns = new ObservableCollection<ScheduleColumn>();
            ColumnLayouter = new EvenColumnLayouter() { Owner = this, SnappingBehavior = new DateColumnSnappingBehavior() { Owner = this } };
            ColumnGenerator = new ColumnGenerator<DateTime>() { Start = DateTime.Today, Interval = new TimeSpan(1, 0, 0, 0), End = DateTime.Today.AddDays(7) };

            Rows = new ObservableCollection<ScheduleRow>();
            RowLayouter = new EvenRowLayouter() { Owner = this, SnappingBehavior = new TimeRowSnappingBehavior() { Owner = this } };
            RowGenerator = new RowGenerator<TimeSpan>() { Start = new TimeSpan(0, 0, 0), Interval = new TimeSpan(1, 0, 0), End = new TimeSpan(24, 0, 0) };

            RenderTransform = _translate;
        }

        public override void OnApplyTemplate()
        {
            _itemsHost = (ScheduleVirtualizingPanel)Template.FindName("PART_ItemsHost", this);
            _topLeft = (FrameworkElement)Template.FindName("PART_TopLeft", this);
            _horizontalHeaderHost = (ItemsControl)Template.FindName("PART_HorizontalHeaderHost", this);
            _verticalHeaderHost = (ItemsControl)Template.FindName("PART_VerticalHeaderHost", this);
            _selectionFrame = (Border)Template.FindName("PART_SelectionFrame", this);
            _overlay = (Panel)Template.FindName("PART_Overlay", this);

            _hsConverter = (IValueConverter)TryFindResource("HorizontalStartConverter") ?? new DateTimeLayoutConverter(false);
            _heConverter = (IValueConverter)TryFindResource("HorizontalEndConverter") ?? new DateTimeLayoutConverter(true);
            _vsConverter = (IValueConverter)TryFindResource("VerticalStartConverter") ?? new TimeSpanLayoutConverter();
            _veConverter = (IValueConverter)TryFindResource("VerticalEndConverter") ?? new TimeSpanLayoutConverter();

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
            var item = new ScheduleItem() { Owner = this };
            //item.Bottom = 100;
            //item.Right = 100;

            BindingOperations.SetBinding(item, ScheduleItem.LeftProperty, new Binding("HorizontalStartValue") { Converter = _hsConverter, ConverterParameter = item });
            BindingOperations.SetBinding(item, ScheduleItem.RightProperty, new Binding("HorizontalEndValue") { Converter = _heConverter, ConverterParameter = item });
            BindingOperations.SetBinding(item, ScheduleItem.TopProperty, new Binding("VerticalStartValue") { Converter = _vsConverter, ConverterParameter = item });
            BindingOperations.SetBinding(item, ScheduleItem.BottomProperty, new Binding("VerticalEndValue") { Converter = _veConverter, ConverterParameter = item });
            return item;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            InvalidateScrollInfo(arrangeBounds);

            _topLeft.Arrange(new Rect(_topLeft.DesiredSize));
            _horizontalHeaderHost.Arrange(new Rect(new Point(50, 0), new Size(ColumnLayouter.GetDesiredWidth(), 20)));
            _verticalHeaderHost.Arrange(new Rect(new Point(0, 20), new Size(50, RowLayouter.GetDesiredHeight())));
            _itemsHost.Arrange(new Rect(new Point(50, 20), new Size(ColumnLayouter.GetDesiredWidth(), RowLayouter.GetDesiredHeight())));
            _overlay.Arrange(new Rect(new Point(50, 20), new Size(ColumnLayouter.GetDesiredWidth(), RowLayouter.GetDesiredHeight())));
            _selectionFrame.Arrange(new Rect(new Point(50, 20),
                                             new Size(ColumnLayouter.GetDesiredWidth(), RowLayouter.GetDesiredHeight())));
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
            _overlay.Measure(new Size(_extent.Width > constraint.Width ? constraint.Width : _extent.Width,
                                      _extent.Height > constraint.Height ? constraint.Height : _extent.Height));
            _selectionFrame.Measure(constraint);
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

        public string HorizontalValueMember { get; set; }
        public string VerticalValueMember { get; set; }
        public IColumnLayouter ColumnLayouter { get; set; }
        public IRowLayouter RowLayouter { get; set; }
        public ObservableCollection<ScheduleColumn> Columns { get; set; }
        public ObservableCollection<ScheduleRow> Rows { get; set; }
        public IRowGenerator RowGenerator { get; set; }
        public IColumnGenerator ColumnGenerator { get; set; }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        private ScheduleItem _lastSelected = null;

        public void Select(ScheduleItem item)
        {
            //if (button == MouseButton.Left && !Equals(Mouse.Captured, this))
            //{
            //    Mouse.Capture(this, CaptureMode.SubTree);
            //    //base.SetInitialMousePosition();
            //}

            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    if (!item.IsSelected)
                        UnselectAll();

                    item.IsSelected = !item.IsSelected;

                    break;
                case SelectionMode.Multiple:
                    if (!item.IsSelected && (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                    {
                        _lastSelected = null;
                        UnselectAll();
                    }

                    if (!SelectMultiple(item))
                        item.IsSelected = !item.IsSelected;
                    
                    _lastSelected = item.IsSelected ? item : null;

                    break;
                case SelectionMode.Extended:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == 0 &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                    {
                        _lastSelected = null;
                        UnselectAll();
                    }

                    if (!SelectMultiple(item))
                        item.IsSelected = (Keyboard.Modifiers & ModifierKeys.Control) == 0 || !item.IsSelected;
                    
                    _lastSelected = item.IsSelected ? item : null;

                    break;
            }
        }

        private bool SelectMultiple(ScheduleItem item)
        {
            var result = false;
            if (_lastSelected != null && !Equals(_lastSelected, item) && (Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                var rect = new Rect(new Point(_lastSelected.Left, _lastSelected.Top),
                                    new Point(item.Right, item.Bottom));
                Debug.WriteLine(rect);
                SelectByRect(rect);
                result = true;
            }
            return result;
        }

        private MouseInfos? _mouseInfos = null;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!_mouseInfos.HasValue)
            {
                var p = e.GetPosition(_itemsHost);

                if (p.X > 0 && p.Y > 0)
                    _mouseInfos = new MouseInfos() { MouseButton = e.ChangedButton, StartLocation = p };
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_mouseInfos.HasValue)
            {
                var p = e.GetPosition(_itemsHost);
                var v = p - _mouseInfos.Value.StartLocation;

                if (SystemParameters.MinimumVerticalDragDistance <= Math.Abs(v.Y) ||
                    SystemParameters.MinimumHorizontalDragDistance <= Math.Abs(v.X))
                {
                    Mouse.Capture(_itemsHost);

                    var width = Math.Max(p.X, 0) - _mouseInfos.Value.StartLocation.X;
                    var height = Math.Max(p.Y, 0) - _mouseInfos.Value.StartLocation.Y;
                    var x = width < 0 ? p.X : _mouseInfos.Value.StartLocation.X;
                    var y = height < 0 ? p.Y : _mouseInfos.Value.StartLocation.Y;

                    _selectionFrame.Margin = new Thickness(Math.Max(x, 0), Math.Max(y, 0), 0, 0);
                    _selectionFrame.Width = Math.Min(Math.Abs(width), _itemsHost.ActualWidth - x);
                    _selectionFrame.Height = Math.Min(Math.Abs(height), _itemsHost.ActualHeight - y);
                    Debug.WriteLine("{0},{1},{2},{3}", _selectionFrame.Margin.Left, _selectionFrame.Margin.Top,
                                    _selectionFrame.Width, _selectionFrame.Height);
                    _selectionFrame.Visibility = Visibility.Visible;
                    InvalidateArrange();

                    var selectionRectangle = new Rect(_selectionFrame.Margin.Left, _selectionFrame.Margin.Top,
                                                  _selectionFrame.ActualWidth, _selectionFrame.ActualHeight);

                    SelectByRect(selectionRectangle);
                }
            }

            base.OnMouseMove(e);
        }

        private void SelectByRect(Rect selectionRectangle)
        {
            foreach (UIElement element in _itemsHost.Children)
            {
                var child = element as ScheduleItem;

                if (child != null)
                {
                    var childRect = new Rect(new Point(child.Left, child.Top),
                                             new Point(child.Right, child.Bottom));
                    child.IsSelected = selectionRectangle.IntersectsWith(childRect);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_mouseInfos.HasValue)
            {
                _mouseInfos = null;
                _selectionFrame.Visibility = Visibility.Collapsed;
                Mouse.Capture(null);
            }

            base.OnMouseUp(e);
        }

        private struct MouseInfos
        {
            public MouseButton MouseButton;
            public Point StartLocation;
        }

        public void StartBehavior(ScheduleItem source, IManipulatorBehavior behavior)
        {
            if (_activeManipulator == null)
            {
//                foreach (
//                    ScheduleItem item in
//                        (from UIElement c in _itemsHost.Children
//                         where c is ScheduleItem && Selector.GetIsSelected(c)
//                         select c))
//                {

//                    var workingCopy = item.Clone();
//                    workingCopy.Tag = item;

//                    BindingOperations.SetBinding(workingCopy, Canvas.LeftProperty, new Binding("Left") { Source = workingCopy });
//                    BindingOperations.SetBinding(workingCopy, Canvas.TopProperty, new Binding("Top") { Source = workingCopy });
//                    BindingOperations.SetBinding(workingCopy, Canvas.RightProperty, new Binding("Right") { Source = workingCopy });
//                    BindingOperations.SetBinding(workingCopy, Canvas.BottomProperty, new Binding("Bottom") { Source = workingCopy });
////                    Canvas.SetLeft(workingCopy, workingCopy.Left);
////                    Canvas.SetTop(workingCopy, workingCopy.Top);
////                    Canvas.SetRight(workingCopy, workingCopy.Right);
////                    Canvas.SetBottom(workingCopy, workingCopy.Bottom);
                    
//                    _workingCopies.Add(workingCopy);
//                    _overlay.Children.Add(workingCopy);
//                }

//                InvalidateArrange();
                _activeManipulator = behavior;
            }
        }

        public void ProcessBehavior(ScheduleItem source, Point change)
        {
            if (_activeManipulator != null)
            {
                EnsureWorkCopies();

                foreach (var c in _workingCopies)
                {
                    var original = (ScheduleItem) c.Tag;
                    var mp = Mouse.GetPosition(original);
                    Debug.WriteLine(new Point(original.Left + mp.X + change.X, original.Top + change.Y));
                	
                    var alignedPoint = new Point(ColumnLayouter.SnappingBehavior.Align(original.Left + mp.X + change.X), RowLayouter.SnappingBehavior.Align(original.Top + change.Y));
                    c.Left = alignedPoint.X;
                    //c.Right = alignedPoint.X;
                    c.Top = alignedPoint.Y;
                    c.Bottom = alignedPoint.Y + c.ActualHeight;
                    Debug.WriteLine(alignedPoint);
                	//var alignedPoint = new Point(ColumnLayouter.SnappingBehavior.Align(mp.X),RowLayouter.SnappingBehavior.Align(mp.Y));
                    
                    //_activeManipulator.Manipulate(mp, c);
                    c.BorderBrush = (Brush)TryFindResource("SelectionFrameBorderBrush");
                    c.Background = (Brush)TryFindResource("SelectionFrameBackgroundBrush");
                    
                    //c.Top = RowLayouter.SnappingBehavior.Align(c.Top);
//                    Canvas.SetLeft(c, c.Left);
                    //Canvas.SetTop(c, c.Top);
//                    Canvas.SetRight(c, c.Right);
                    //Canvas.SetBottom(c, c.Bottom);
                }
                InvalidateArrange();
            }
        }

        private void EnsureWorkCopies()
        {
            if (!_workingCopies.Any())
            {
                foreach (
                    ScheduleItem item in
                        (from UIElement c in _itemsHost.Children
                         where c is ScheduleItem && Selector.GetIsSelected(c)
                         select c))
                {
                    var workingCopy = item.Clone();
                    workingCopy.Tag = item;

                    BindingOperations.SetBinding(workingCopy, Canvas.LeftProperty,
                                                 new Binding("Left") {Source = workingCopy});
                    BindingOperations.SetBinding(workingCopy, Canvas.TopProperty,
                                                 new Binding("Top") {Source = workingCopy});
                    BindingOperations.SetBinding(workingCopy, Canvas.RightProperty,
                                                 new Binding("Right") {Source = workingCopy});
                    BindingOperations.SetBinding(workingCopy, Canvas.BottomProperty,
                                                 new Binding("Bottom") {Source = workingCopy});

                    _workingCopies.Add(workingCopy);
                    _overlay.Children.Add(workingCopy);
                }
            }
        }

        public void StopBehavior(ScheduleItem source, bool cancel)
        {
            if (_workingCopies.Count > 0)
            {
            	if(!cancel)
            	{
	                foreach (var wc in _workingCopies)
	                {
	                    var original = wc.Tag as ScheduleItem;
	                    original.Top = wc.Top;
	                    original.Bottom = wc.Bottom;
	                }
            	}
            	
                _overlay.Children.Clear();
                InvalidateArrange();
            }

            _activeManipulator = null;
            _workingCopies.Clear();
        }
    }
}
