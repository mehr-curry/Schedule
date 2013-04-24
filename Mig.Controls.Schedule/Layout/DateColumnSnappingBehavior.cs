/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 19.04.2013
 * Time: 07:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
	public class DateColumnSnappingBehavior
		: ISnappingBehavior
	{
		public Schedule Owner { get; set; }
		
		public double TranslateFromSource(object value)
        {
            double offset = 0;

            foreach (ScheduleColumn col in Owner.Columns)
            {
                if (!Equals(col.Value, value))
                    offset += col.Width;
                else
                    break;
            }

            return offset;
        }
		
		public object TranslateToSource(double horizontalValue)
	    {
            if (Owner.Columns.Any())
            {
                var offset = 0D;

                foreach (var c in Owner.Columns)
                {
                    offset += c.Width;

                    if (offset >= horizontalValue)
                        return c.Value;
                }
            }

            return null;
	    }

	    public double Align(double horizontalValue)
	    {
            if (Owner.Columns.Any())
            {
                var offset = 0D;

                foreach (var c in Owner.Columns)
                {
                    var next = offset + c.Width;

                    if (horizontalValue >= offset && horizontalValue <= next)
                        return offset;

                    offset = next;
                }
            }

            return 0D;
	    }
  
//	     public object TranslateToSource(double offset)
//        {
//            if (Owner.Rows.Any())
//            {
//                var interval = (TimeSpan)Owner.ColumnGenerator.Interval;
//                var end = (DateTime)Owner.ColumnGenerator.End;
//                var factor = interval.TotalMilliseconds / Owner.Columns[0].Width;
//                var ms = offset * factor;
//                ms = Math.Round(ms / 24*60*60*1000, 0) * 24*60*60*1000; //Taktung bzw Ausrichtung
//                if (ms < 0D) ms = 0D;
//                //if (ms > end..TotalMilliseconds) ms = end.TotalMilliseconds;
//                return new DateTime(0,0,0,0,0,0,(int)Math.Round(ms, 0));
//            }
//
//            return null;
//        }
//
//        public double TranslateFromSource(object value)
//        {
//            if (Owner.Rows.Any())
//            {
//                var interval = (TimeSpan)Owner.ColumnGenerator.Interval;
//                var factor = Owner.Columns[0].Width / interval.TotalSeconds;
//                return ((DateTime)value).Ticks * factor;
//            }
//
//            return double.NaN;
//        }
	}
}
