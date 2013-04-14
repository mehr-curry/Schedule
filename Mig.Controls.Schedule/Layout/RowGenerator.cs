using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mig.Controls.Schedule.Layout
{
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
            DependencyProperty.Register("Start", typeof(object), typeof(RowGenerator<>), new UIPropertyMetadata(null));

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(object), typeof(RowGenerator<>), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(RowGenerator<>), new UIPropertyMetadata(null));
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

        private object _current;
        T IEnumerator<T>.Current {
        	get { return (T)_current; }
        }

        void IDisposable.Dispose()
        {
            _current = null;
        }

        object IEnumerator.Current
        {
            get { return _current; }
        }

        bool IEnumerator.MoveNext()
        {
            bool result;

            if (_current == null)
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
            _current = null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return this; }

        IEnumerator IEnumerable.GetEnumerator() { return this; }
    }
}