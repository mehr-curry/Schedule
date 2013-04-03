using System;

namespace Mig.Controls.Schedule.Layout
{
    /// <summary>
    /// Description of EqualColumnLayouter.
    /// </summary>
    public class EvenRowLayouter : IRowLayouter
    {
        public EvenRowLayouter()
        {
        }

        public Schedule Owner { get; set; }

        public void Calculate(ScheduleRow row, double change)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (!Owner.Rows.Contains(row))
                throw new ArgumentOutOfRangeException("row");

            row.SetCurrentValue(ScheduleRow.HeightProperty, row.Height + change / (Owner.Rows.IndexOf(row) + 1));

            foreach (ScheduleRow r in Owner.Rows)
                if (r != row)
                    r.Height = row.Height;
        }
    }
}