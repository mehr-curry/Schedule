using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
    public class ScheduleVirtualizingPanel : VirtualizingPanel
    {
        private Schedule _owner;

        public ScheduleVirtualizingPanel()
        {
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }
		        
        protected override Size MeasureOverride(Size constraint)
        {
//        	var minMax = new MinMax(this);
            //return base.MeasureOverride(constraint);

//            Debug.WriteLine(constraint);
            var dummy = base.InternalChildren; // initialisiert die Property ItemContainerGenerator. Ist ein Bug

            // Do work for IScrollInfo implementation
            //Owner.InvalidateScrollInfo(constraint);

            // Figure out range that's visible based on layout algorithm

			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            if (itemsControl == null || itemsControl.ItemsSource == null) return base.MeasureOverride(constraint);

            //            var visibleItems = (from i in itemsControl.ItemsSource.Cast<IDataItem>()
            //            					from c in cols 
            //            					where c.Value != null && i.HorizontalValue != null && c.Value.Equals(i.HorizontalValue)
            //            					select i).ToArray();


            var cols = Owner.ColumnLayouter.GetVisibleColumns(new Rect(constraint))
            								.ToDictionary<ScheduleColumn, ScheduleColumn, List<IDataItem>>(col => col, col => new List<IDataItem>(), ScheduleColumn.DefaultComparer);
            //var cols = Owner.ColumnLayouter.GetVisibleColumns(new Rect(constraint)).ToDictionary<List<IDataItem>, ScheduleColumn>((c,k)=>c, ScheduleColumn.DefaultComparer);
            var items = itemsControl.ItemsSource.OfType<IDataItem>().ToList();
            //var visibleItems = new List<IDataItem>();
            int colIdx = 0, itemIdx = 0;
            int firstVisibleItemIndex = -1, lastVisibleItemIndex = -1;
            
            foreach (var c in cols)
            {
            	//c.Value = new List<IDataItem>();
            	
                if (c.Key.Value != null)
                {
                	for(int k = items.Count() - 1; k > -1; k--)
                	{
                		var i = items[k];
                		
                		if (i.HorizontalStartValue != null &&
                            c.Key.Value.Equals(i.HorizontalStartValue))
                        {
                			c.Value.Add(i);
                            //visibleItems.Add(i);
                            items.RemoveAt(k);

                            lastVisibleItemIndex++;
                            
                            if (firstVisibleItemIndex < 0)
                                firstVisibleItemIndex = lastVisibleItemIndex;
                        }
                	}
                    colIdx++;
                }
            }

            if (firstVisibleItemIndex >= 0)
            {
                IItemContainerGenerator generator = this.ItemContainerGenerator;

                // Get the generator position of the first visible data item
                GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

                var childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    for (int itemIndex = firstVisibleItemIndex;
                         itemIndex <= lastVisibleItemIndex;
                         ++itemIndex, ++childIndex)
                    {
                        bool newlyRealized;

                        // Get or create the child
                        var child = generator.GenerateNext(out newlyRealized) as ScheduleItem;

                        var dataItem = child.DataContext as IDataItem;
                        var col = (from c in Owner.Columns where c.Value.Equals(dataItem.HorizontalStartValue) select c).FirstOrDefault();

                        if (col != null)
                        {
                            if (child != null)
                            {
                                if (newlyRealized)
                                {
                                    // Figure out if we need to insert the child at the end or somewhere in the middle
                                    if (childIndex >= InternalChildren.Count)
                                        base.AddInternalChild(child);
                                    else
                                        base.InsertInternalChild(childIndex, child);

                                    generator.PrepareItemContainer(child);
                                }
                                else
                                {
                                    // The child has already been created, let's be sure it's in the right spot
                                    //Debug.Assert(child == InternalChildren[childIndex], "Wrong child was generated");
                                }
                            }

                            // Measurements will depend on layout algorithm
                            child.Measure(new Size(col.Width, 20));
                        }
                    }
                }
                //// Note: this could be deferred to idle time for efficiency
                CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);
            }
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in InternalChildren)
            {
                var scheduleItem = child as ScheduleItem;
                if (scheduleItem != null)
                {
                    //scheduleItem.ClearValue(ScheduleItem.TopProperty);
                    //scheduleItem.ClearValue(ScheduleItem.BottomProperty);
                    //scheduleItem.ClearValue(ScheduleItem.LeftProperty);
                    //scheduleItem.ClearValue(ScheduleItem.RightProperty);
                    //scheduleItem.InvalidateProperty(ScheduleItem.TopProperty);
                    //scheduleItem.InvalidateProperty(ScheduleItem.BottomProperty);
                    //scheduleItem.InvalidateProperty(ScheduleItem.LeftProperty);
                    //scheduleItem.InvalidateProperty(ScheduleItem.RightProperty);
                    var dataItem = scheduleItem.DataContext as IDataItem;
                    
                    if (dataItem != null)
                        dataItem.Invalidate();

                    var childRect = new Rect(new Point(scheduleItem.Left, scheduleItem.Top),
                                             new Point(scheduleItem.Right, scheduleItem.Bottom));

                    scheduleItem.Arrange(childRect);
                }
                //var dataItem = scheduleItem.DataContext as IDataItem;

                //var col = (from c in Owner.Columns where c.Value.Equals(dataItem.HorizontalStartValue) select c).FirstOrDefault();

                //if (col != null)
                //{
                //    //if ((DateTime) dataItem.HorizontalStartValue == new DateTime(2013, 04, 15) &&
                //    //    (TimeSpan) dataItem.VerticalStartValue != new TimeSpan(6, 0, 0))
                //    //{
                //    //    int stop = 0;
                //    //}

                //    var x = Owner.ColumnLayouter.TranslateFromSource(col.Value);
                //    //var y = scheduleItem.Top;
                //    //var height = scheduleItem.Bottom - scheduleItem.Top;
                //    var y1 = Owner.RowLayouter.TranslateFromSource((TimeSpan)dataItem.VerticalStartValue);
                //    var y2 = Owner.RowLayouter.TranslateFromSource((TimeSpan)dataItem.VerticalEndValue);
                //    var height = y2 - y1;

                //    //scheduleItem.SetCurrentValue(ScheduleItem.TopProperty, y1);
                //    //scheduleItem.SetCurrentValue(ScheduleItem.BottomProperty, y2);

                //    dataItem.EvaluateLocation();

                //    if (height > 0)
                //        scheduleItem.Arrange(new Rect(x, y1, col.Width, height));
                //}
            }

            return finalSize;
        }


        private void CleanUpItems(int firstVisibleItemIndex, int lastVisibleItemIndex)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                // Map a child index to an item index by going through a generator position
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                if (itemIndex < firstVisibleItemIndex || itemIndex > lastVisibleItemIndex)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        public ObservableCollection<ScheduleColumn> Columns { get; set; }
        public ObservableCollection<ScheduleRow> Rows { get; set; }
        public Schedule Owner { get { return _owner ?? (_owner = Helper.FindParent<Schedule>(this)); } }
    }
}
