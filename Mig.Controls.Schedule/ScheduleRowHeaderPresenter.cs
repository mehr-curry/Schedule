/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of ScheduleRowHeaderPresenter.
	/// </summary>
	public class ScheduleRowHeaderPresenter : ItemsControl
	{
        private Schedule _owner;

        public ScheduleRowHeaderPresenter() { }
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			if(Owner != null)
			{
				ItemsSource = from r in Owner.Rows select r.Header;
			}
		}
		
		public Schedule Owner 
		{ 
			get { return _owner ?? (_owner = Helper.FindParent<Schedule>(this)); }
		}
		
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ScheduleRowHeader() { VerticalAlignment = VerticalAlignment.Top };
		} 
		
		private ScheduleRow RowFromContainer(ScheduleRowHeader container)
		{
			int index = base.ItemContainerGenerator.IndexFromContainer(container);
			return Owner.Rows[index];
		}
		
		protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item)
		{
            var header = element as ScheduleRowHeader;
			
			if(header != null)
			    header.Row = RowFromContainer(header);
				
			base.PrepareContainerForItemOverride(element, item);
		}

        //public ScheduleRowHeaderPresenter()
        //{
        //}
		
        //protected override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    return item is ScheduleRowHeader;
        //}
		
        //protected override System.Windows.DependencyObject GetContainerForItemOverride()
        //{
        //    return new ScheduleRowHeader();
        //}
	}
}
