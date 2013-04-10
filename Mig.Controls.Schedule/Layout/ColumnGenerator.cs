using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mig.Controls.Schedule.Layout
{
    public class ColumnGenerator<T> : DependencyObject, IColumnGenerator, IEnumerable<T>, IEnumerator<T>
    {
        public ObservableCollection<ScheduleColumn> Generate()
        {
            var result = new ObservableCollection<ScheduleColumn>();

            foreach (var s in this)
                result.Add(new ScheduleColumn() { Value = s, Header = s });

            return result;
        }

        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
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
            DependencyProperty.Register("Start", typeof(object), typeof(ColumnGenerator<>), new UIPropertyMetadata(null));

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(object), typeof(ColumnGenerator<>), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(ColumnGenerator<>), new UIPropertyMetadata(null));
        // ReSharper restore StaticFieldInGenericType

        ObservableCollection<ScheduleColumn> IColumnGenerator.Generate() { return Generate(); }

        object IColumnGenerator.Start
        {
            get { return Start; }
            set { Start = (T)value; }
        }

        object IColumnGenerator.End
        {
            get { return End; }
            set { End = (T)value; }
        }

        private T _current;
        T IEnumerator<T>.Current
        {
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
            bool result;

            if (_current == null || _current.Equals(default(T)))
            {
                _current = Start;
                result = true;
            }
            else
            {
                object next = _current;
                result = Incrementer.Inc(ref next, Interval, End);

                if (result)
                    _current = (T)next;
            }

            return result;
        }

        void IEnumerator.Reset()
        {
            _current = default(T);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return this; }

        IEnumerator IEnumerable.GetEnumerator() { return this; }

    }
}
