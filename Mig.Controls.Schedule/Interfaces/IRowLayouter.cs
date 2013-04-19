using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
    public interface IRowLayouter
    {
        Schedule Owner { get; set; }
        void Calculate(ScheduleRow row, double change);
        void SetAll(double height);
        IEnumerable<ScheduleRow> GetVisibleRows(Rect viewport);
        double TranslateFromSource(object value);
        object TranslateToSource(double value);
        double GetDesiredHeight();
        ISnappingBehavior SnappingBehavior { get; set; }
    }
}