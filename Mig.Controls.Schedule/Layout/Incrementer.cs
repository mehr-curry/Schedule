using System;

namespace Mig.Controls.Schedule.Layout
{
    internal static class Incrementer
    {
        public static bool Inc(ref object current, object step, object end)
        {
            if (current == null)
                throw new ArgumentNullException("current");

            if (step == null)
                throw new ArgumentNullException("step");

            if (end == null)
                throw new ArgumentNullException("end");

            //current.GetType() != step.GetType() ||
            if (current.GetType() != end.GetType()) 
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
            else if (current is DateTime)
            {
                var dtCur = (DateTime)current;
                 var iStep = (TimeSpan)step;
                var dtEnd = (DateTime)end;

                if (dtCur.Add(iStep) >= dtEnd)
                    return false;

                current = dtCur.Add(iStep);
                return true;
            }

            throw new NotSupportedException();
        }
    }
}