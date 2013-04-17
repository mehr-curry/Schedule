using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mig.Controls.Schedule.Converter
{
    public class DateTimeLayoutConverter : IValueConverter
    {
        public DateTimeLayoutConverter(bool endValueConverter)
        {
            IsEndValueConverter = endValueConverter;
        }

        public bool IsEndValueConverter { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is DateTime))
                throw new ArgumentOutOfRangeException("value");

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            var item = (ScheduleItem)parameter;
            var result = item.Owner.ColumnLayouter.TranslateFromSource((DateTime)value);

            if (IsEndValueConverter)
                result += item.Owner.Columns[0].Width;

            return result;
        }

        public object ConvertBack(object horizontalValue, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(horizontalValue is double))
                throw new ArgumentOutOfRangeException("horizontalValue");

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            var item = (ScheduleItem)parameter;
            var result = item.Owner.ColumnLayouter.TranslateToSource((double)horizontalValue);
            Debug.WriteLine("{0} -> {1}", horizontalValue, result);
            return result;
        }
    }
}
