/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 14.04.2013
 * Time: 07:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Data;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Converter
{
	[ValueConversion(typeof(TimeSpan), typeof(double))]
	public class TimeSpanLayoutConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(!(value is TimeSpan))
				throw new ArgumentOutOfRangeException("value");
			
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			
			var item = (ScheduleItem)parameter;
			var result = item.Owner.RowLayouter.GetOffset((TimeSpan)value);
			return result;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(!(value is double))
				throw new ArgumentOutOfRangeException("value");
			
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			
			var item = (ScheduleItem)parameter;
			var result = item.Owner.RowLayouter.GetTimeSpan((double)value);
			Debug.WriteLine("{0} -> {1}", value, result);
			return result;
			
			//throw new NotImplementedException();
		}
	}
}
