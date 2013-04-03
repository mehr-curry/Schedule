namespace Mig.Controls.Schedule.Layout
{
    public interface IRowLayouter
    {
        Schedule Owner { get; set; }
        void Calculate(ScheduleRow row, double change);
    }
}