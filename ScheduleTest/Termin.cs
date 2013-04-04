/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mig.Controls.Schedule.Interfaces;

namespace ScheduleTest
{
	/// <summary>
	/// Description of Termin.
	/// </summary>
	public class Termin : IEntry, IDataItem
	{
		public Termin()
		{

		}
		public TimeSpan Von { get; set; }
		public TimeSpan Bis { get; set; }
		public DateTime Datum { get; set; }
		
		object IDataItem.HorizontalValue{get{return Datum;}}
		object IDataItem.VerticalValue{get{return Von;}}
	}
}
