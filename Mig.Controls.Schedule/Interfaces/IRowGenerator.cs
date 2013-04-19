using System.Collections.ObjectModel;

namespace Mig.Controls.Schedule.Layout
{
    public interface IRowGenerator
    {
        ObservableCollection<ScheduleRow> Generate();
        object Interval { get; set; }
        object Start { get; set; }
        object End { get; set; }
    }
}