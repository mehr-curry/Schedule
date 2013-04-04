using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
    public class ScheduleVirtualizingGrid : VirtualizingPanel
    {
        private Schedule _owner;

        public ScheduleVirtualizingGrid()
        {
            var dummy = InternalChildren;

            //DumpGeneratorContent();

            Loaded += new System.Windows.RoutedEventHandler(ScheduleVirtualizingGrid_Loaded);
        }

        void ScheduleVirtualizingGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //var dummy = InternalChildren;
            
            //IItemContainerGenerator generator = ItemContainerGenerator;

            //for (int i = 0; i < 2; i++)
            //{

            //    var position = generator.GeneratorPositionFromIndex(i);

            //    using (generator.StartAt(position, GeneratorDirection.Forward, true))
            //    {
            //        bool isNew = false;
            //        DependencyObject child = generator.GenerateNext(out isNew);
            //        generator.PrepareItemContainer(child);
            //    }
            //}
            //DumpGeneratorContent();
        }

        private void DumpGeneratorContent()
        {
            IItemContainerGenerator generator = ItemContainerGenerator;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            
            Console.WriteLine("Generator positions:");

            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                GeneratorPosition position = generator.GeneratorPositionFromIndex(i);
                
               
                Console.WriteLine("Item index=" + i + ", Generator position: index=" + position.Index + ", offset=" + position.Offset);

            }

            Console.WriteLine();
            
        }


        protected override Size MeasureOverride(Size constraint)
        {
            // Do work for IScrollInfo implementation

            // Figure out range that's visible based on layout algorithm

			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            if (itemsControl == null || itemsControl.ItemsSource == null) return base.MeasureOverride(constraint);

            var cols = Owner.ColumnLayouter.GetVisibleColumns(new Rect(constraint));
            var items = itemsControl.ItemsSource.Cast<IDataItem>()
//            var visibleItems = (from i in itemsControl.ItemsSource.Cast<IDataItem>()
//            					from c in cols 
//            					where c.Value != null && i.HorizontalValue != null && c.Value.Equals(i.HorizontalValue)
//            					select i).ToArray();

            	var visibleItems = new List<IDataItem>();
            
            for(int j = 0; j < cols.Count(); j++)
            {
            	if(cols[j].Value != null)
            	{
					for(int i = 0; i < items.Count; i++)
					{
		            	var item = items[i];
		            	if(item.HorizontalValue != null &&
		            	  	cols[j].Value.Equals(item.HorizontalValue))
		            	{
		            		visibleItems.Add(item);
		            	}
					}
            	}
            }
            	
			int firstVisibleItemIndex = 0, lastVisibleItemIndex = 0;
            //Debug.WriteLine(visibleItems.Count());
            
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(0);

            //// Get index where we'd insert the child for this position. If the item is realized
            //// (position.Offset == 0), it's just position.Index, otherwise we have to add one to
            //// insert after the corresponding child

            var childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;

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
                        Debug.Assert(child == InternalChildren[childIndex], "Wrong child was generated");
                    }


                    // Measurements will depend on layout algorithm
                    //child.Measure(new Size(itemWidth, itemHeight));
                }
            }

            //// Note: this could be deferred to idle time for efficiency
            //CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return constraint;
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
