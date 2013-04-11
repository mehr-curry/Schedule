using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mig.Controls.Schedule.Layout
{
    public interface IRowLayouter
    {
        Schedule Owner { get; set; }
        void Calculate(ScheduleRow row, double change);
        void SetAll(double height);
	    double GetOffset(ScheduleRow row);
        IEnumerable<ScheduleRow> GetVisibleRows(Rect viewport);
        double GetOffset(TimeSpan value);
        double GetDesiredHeight();
    }
}