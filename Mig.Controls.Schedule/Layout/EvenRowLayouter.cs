using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Mig.Controls.Schedule.Layout
{
    /// <summary>
    /// Description of EqualRowLayouter.
    /// </summary>
    public class EvenRowLayouter : IRowLayouter
    {
        public EvenRowLayouter()
        {
        }

        public Schedule Owner { get; set; }
        
        public void Calculate(ScheduleRow row, double change){
			if(row == null)
                throw new ArgumentNullException("row");
			
			if(!Owner.Rows.Contains(row))
                throw new ArgumentOutOfRangeException("row");
            
            var width = row.Height + change / (Owner.Rows.IndexOf(row) + 1);
            
            foreach (ScheduleRow col in Owner.Rows)
                    col.SetCurrentValue(ScheduleRow.HeightProperty, width);

		    Owner.InvalidateMeasure();

			//Debug.WriteLine(string.Format("Calc {0} Real {1}", width, Row.Height));
		}

        public double GetOffset(TimeSpan value)
        {
            if (Owner.Rows.Any())
            {
                var interval = (TimeSpan)Owner.RowGenerator.Interval;
                var factor = Owner.Rows[0].Height / interval.TotalSeconds;
                return value.TotalSeconds * factor;
            }

            return double.NaN;
        }

        public double GetOffset(ScheduleRow row)
	    {
	        double offset = 0;

	        foreach (ScheduleRow r in Owner.Rows)
	        {
	            if (r != row)
	                offset += r.Height;
                else 
                    break;
	        }

	        return offset;
	    }

        public IEnumerable<ScheduleRow> GetVisibleRows(Rect viewport)
        {
            return from row in Owner.Rows let xOffset = GetOffset(row) where xOffset < viewport.Bottom && xOffset + row.Height > viewport.Top select row;
        }

    }

    public interface IRowGenerator
    {
        ObservableCollection<ScheduleRow> Generate();
        object Interval { get; set; }
        object Start { get; set; }
        object End { get; set; }
    }

    public class RowGenerator<T> : DependencyObject, IRowGenerator, IEnumerable<T>, IEnumerator<T>
    {
        public ObservableCollection<ScheduleRow> Generate()
        {
            var result = new ObservableCollection<ScheduleRow>();

            foreach (var s in this)
                result.Add(new ScheduleRow() { Value = s, Header = s });

            return result;
        }

        public T Interval
        {
            get { return (T)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public T Start
        {
            get { return (T)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public T End
        {
            get { return (T)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }
        
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(object), typeof(Schedule), new UIPropertyMetadata(null));

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(object), typeof(Schedule), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(Schedule), new UIPropertyMetadata(null));
// ReSharper restore StaticFieldInGenericType

        ObservableCollection<ScheduleRow> IRowGenerator.Generate() { return Generate(); }

        object IRowGenerator.Interval
        {
            get { return Interval; }
            set { Interval = (T)value; }
        }

        object IRowGenerator.Start
        {
            get { return Start; }
            set { Start = (T)value; }
        }

        object IRowGenerator.End
        {
            get { return End; }
            set { End = (T)value; }
        }

        private T _current;
        T IEnumerator<T>.Current {
            get { return _current; }
        }

        void IDisposable.Dispose()
        {
            _current = default(T);
        }

        object IEnumerator.Current
        {
            get { return _current; }
        }

        bool IEnumerator.MoveNext()
        {
            object next = _current;
            bool result = Incrementer.Inc(ref next, Interval, End);

            if (result)
                _current = (T)next;

            return result;
        }

        void IEnumerator.Reset()
        {
            _current = default(T);
        }

        private static class Incrementer
        {
            public static bool Inc(ref object current, object step, object end)
            {
                if (current == null)
                    throw new ArgumentNullException("current");

                if (step == null)
                    throw new ArgumentNullException("step");

                if (end == null)
                    throw new ArgumentNullException("end");

                if (current.GetType() != step.GetType() ||
                    current.GetType() != end.GetType())
                    throw new InvalidOperationException();

                if (current is TimeSpan)
                {
                    var tsCur = (TimeSpan) current;
                    var tsStep = (TimeSpan) step;
                    var tsEnd = (TimeSpan) end;

                    if (tsCur + tsStep >= tsEnd)
                        return false;

                    current = tsCur + tsStep;
                    return true;
                }

                throw new NotSupportedException();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return this; }

        IEnumerator IEnumerable.GetEnumerator() { return this; }
    }
}