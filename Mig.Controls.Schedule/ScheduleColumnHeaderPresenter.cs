/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 10:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Mig.Controls.Schedule
{
	public class ScheduleColumnHeaderPresenter : ItemsControl
	{
		private Schedule _owner;
		
		public ScheduleColumnHeaderPresenter(){}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			if(Owner != null)
				ItemsSource = from c in Owner.Columns select c.Header;
		}
		
		public Schedule Owner 
		{ 
			get { return _owner ?? (_owner = Helper.FindParent<Schedule>(this)); }
		}
		
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ScheduleColumnHeader() { HorizontalAlignment = HorizontalAlignment.Left };
		} 
		
		private ScheduleColumn ColumnFromContainer(ScheduleColumnHeader container)
		{
			int index = base.ItemContainerGenerator.IndexFromContainer(container);
			return Owner.Columns[index];
		}
		
		protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item)
		{
			var header = element as ScheduleColumnHeader;
			
			if(header != null)
                header.Column = ColumnFromContainer(header);
				
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}
