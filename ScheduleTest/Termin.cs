/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ScheduleTest
{
	/// <summary>
	/// Description of Termin.
	/// </summary>
	public class Termin : IEntry
	{
		public Termin()
		{

		}
		public TimeSpan Von { get; set; }
		public TimeSpan Bis { get; set; }
		public DateTime Datum { get; set; }
	}
}
