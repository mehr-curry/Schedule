/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 21:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mig.Controls.Schedule.Layout
{
    ///// <summary>
    ///// Description of PromotingColumnLayouter.
    ///// </summary>
    //public class PromotingColumnLayouter : IColumnLayouter
    //{
    //    public PromotingColumnLayouter()
    //    {
    //    }
		
    //    public Schedule Owner { get; set; }
		
    //    public void Calculate(ScheduleColumn column, double change)
    //    {
    //        if(column == null)
    //            throw new ArgumentNullException("column");
			
    //        if(Owner.Columns.Count == 0)
    //            throw new InvalidOperationException("Keine Spalten vorhanden");
			
    //        if(!Owner.Columns.Contains(column))
    //            throw new ArgumentOutOfRangeException("column");
			
    //        double perColumnChange = change / (Owner.Columns.Count-1);
			
    //        foreach(ScheduleColumn col in Owner.Columns)
    //            if(col == column)
    //                col.Width += change;
    //            else
    //                col.Width -= perColumnChange;
    //    }
    //}
}
