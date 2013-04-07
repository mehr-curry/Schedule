/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 21:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of ScheduleColumn.
	/// </summary>
	public class ScheduleColumn : DependencyObject
	{
		public class ColumnEqualityComparer : IEqualityComparer<ScheduleColumn>
		{
			public bool Equals(ScheduleColumn x, ScheduleColumn y)
			{
				return x != null && y != null && x.Value != null && y.Value != null && x.Value.Equals(y.Value);
			}
			
			public int GetHashCode(ScheduleColumn obj)
			{
				if(obj == null)
					throw new ArgumentNullException("obj");
				
				return obj.GetHashCode();
			}
		}
		
		public static readonly IEqualityComparer<ScheduleColumn> DefaultComparer = new ColumnEqualityComparer();
		
		public ScheduleColumn()
		{
		}

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ScheduleColumn), new UIPropertyMetadata(null));

        
		
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(ScheduleColumn),
			                            new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, Width_CoerceValue));
		
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		private static object Width_CoerceValue(DependencyObject sender, object value){
			if(value is double)
			{
				double minValue = (double)sender.GetValue(MinimumWidthProperty);
				double maxValue = (double)sender.GetValue(MaximumWidthProperty);

//                Debug.WriteLine(String.Format("Coerce Width: {0} {1} ({3} {4}) {2}", minValue, value, maxValue, sender.ReadLocalValue(WidthProperty), sender.GetValue(WidthProperty)));

				if((double)value < minValue)
					return minValue;
				
				if((double)value > maxValue)
					return maxValue;
			}
			return value;
		}
		
		public static readonly DependencyProperty MinimumWidthProperty =
			DependencyProperty.Register("MinimumWidth", typeof(double), typeof(ScheduleColumn),
			                            new FrameworkPropertyMetadata(0D, MinimumWidth_PropertyChanged, MinimumWidth_CoerceValue));
		
		public double MinimumWidth {
			get { return (double)GetValue(MinimumWidthProperty); }
			set { SetValue(MinimumWidthProperty, value); }
		}
		
		private static object MinimumWidth_CoerceValue(DependencyObject sender, object value){
			if(value is double)
			{
				double maxWidth = (double)sender.GetValue(MaximumWidthProperty);

//                Debug.WriteLine(String.Format("Coerce Width: {0} {1}", value, maxWidth));

                if ((double)value > maxWidth)
                    return maxWidth;
			}
			return value;
		}
		private static void MinimumWidth_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			sender.CoerceValue(WidthProperty);
		}
		
		public static readonly DependencyProperty MaximumWidthProperty =
			DependencyProperty.Register("MaximumWidth", typeof(double), typeof(ScheduleColumn),
			                            new FrameworkPropertyMetadata(double.PositiveInfinity, MaximumWidth_PropertyChanged, MaximumWidth_CoerceValue));
		
		public double MaximumWidth {
			get { return (double)GetValue(MaximumWidthProperty); }
			set { SetValue(MaximumWidthProperty, value); }
		}
		
		private static object MaximumWidth_CoerceValue(DependencyObject sender, object value){
			if(value is double)
			{
				double minWidth = (double)sender.GetValue(MinimumWidthProperty);

//                Debug.WriteLine(String.Format("Coerce Width: {0} {1}", minWidth, value));

				if((double)value < minWidth)
					return minWidth;
			}
			return value;
		}
		private static void MaximumWidth_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			sender.CoerceValue(WidthProperty);
		}

        public Object Value
        {
            get { return (Object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(ScheduleColumn), new UIPropertyMetadata(null));

        
	}
}
