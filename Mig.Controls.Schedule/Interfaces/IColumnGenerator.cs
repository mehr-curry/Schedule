using System.Collections.ObjectModel;

namespace Mig.Controls.Schedule.Layout
{
    public interface IColumnGenerator
    {
        ObservableCollection<ScheduleColumn> Generate();
        object Interval { get; set; }
        object Start { get; set; }
        object End { get; set; }
    }
}
