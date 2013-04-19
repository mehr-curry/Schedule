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
                double offset = 0D;

                foreach (var c in Owner.Columns)
                {
                    offset += c.Width;

                    if (offset >= horizontalValue)
                        return c.Value;
                }

                //var colIdx = (int)Math.Floor(horizontalValue / Owner.Columns[0].Width);
                
                //if(colIdx < 0)
                //    colIdx = 0;
                //else
                //    if(colIdx >= Owner.Columns.Count)
                //        colIdx = Owner.Columns.Count-1;
                	
                //return Owner.Columns[colIdx].Value;
            }

            return null;
	    }
	}
}
