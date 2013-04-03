/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 21:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of ScheduleRow.
	/// </summary>
	public class ScheduleRow : DependencyObject
	{
		public ScheduleRow()
		{
		}

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ScheduleRow), new UIPropertyMetadata(null));


        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ScheduleRow),
                                        new FrameworkPropertyMetadata(50D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, Height_CoerceValue));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        private static object Height_CoerceValue(DependencyObject sender, object value)
        {
            if (value is double)
            {
                double minValue = (double)sender.GetValue(MinimumHeightProperty);
                double maxValue = (double)sender.GetValue(MaximumHeightProperty);

                if ((double)value < minValue)
                    return minValue;

                if ((double)value > maxValue)
                    return maxValue;
            }
            return value;
        }

        public static readonly DependencyProperty MinimumHeightProperty =
            DependencyProperty.Register("MinimumHeight", typeof(double), typeof(ScheduleRow),
                                        new FrameworkPropertyMetadata(0D, MinimumHeight_PropertyChanged, MinimumHeight_CoerceValue));

        public double MinimumHeight
        {
            get { return (double)GetValue(MinimumHeightProperty); }
            set { SetValue(MinimumHeightProperty, value); }
        }

        private static object MinimumHeight_CoerceValue(DependencyObject sender, object value)
        {
            if (value is double)
            {
                double maxValue = (double)sender.GetValue(MaximumHeightProperty);
                if ((double)value > maxValue)
                    return maxValue;
            }
            return value;
        }
        private static void MinimumHeight_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.CoerceValue(HeightProperty);
        }

        public static readonly DependencyProperty MaximumHeightProperty =
            DependencyProperty.Register("MaximumHeight", typeof(double), typeof(ScheduleRow),
                                        new FrameworkPropertyMetadata(double.PositiveInfinity, MaximumHeight_PropertyChanged, MaximumHeight_CoerceValue));

        public double MaximumHeight
        {
            get { return (double)GetValue(MaximumHeightProperty); }
            set { SetValue(MaximumHeightProperty, value); }
        }

        private static object MaximumHeight_CoerceValue(DependencyObject sender, object value)
        {
            if (value is double)
            {
                double minHeight = (double)sender.GetValue(MinimumHeightProperty);
                if ((double)value < minHeight)
                    return minHeight;
            }
            return value;
        }
        private static void MaximumHeight_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.CoerceValue(HeightProperty);
        }

        
        
	}
}
