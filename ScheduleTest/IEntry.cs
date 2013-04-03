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
	public interface IEntry
	{
		TimeSpan Von { get; set; }
		TimeSpan Bis { get; set; }
		DateTime Datum { get; set; }
	}
}
