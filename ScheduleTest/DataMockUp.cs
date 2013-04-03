/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace ScheduleTest
{
	/// <summary>
	/// Description of DataMockUp.
	/// </summary>
	public class DataMockUp
	{
		public DataMockUp()
		{
		}
		
		public ObservableCollection<IEntry> EntriesTermin
		{
			get
			{
				return new ObservableCollection<IEntry>() {
														new Termin() {
															Von = new TimeSpan(6,0,0), 
															Bis = new TimeSpan(7,0,0)
																	}
																};
			} 
		}
		
		public IEnumerable HorizontalHeaderDatum
		{ 
			get
			{
				return new ObservableCollection<DateTime>() {DateTime.Today,
																DateTime.Today.AddDays(1),
																DateTime.Today.AddDays(2)};
			}
		}
		
		public IEnumerable VerticalHeaderZeit
		{ 
			get
			{
				return new ObservableCollection<TimeSpan>() {TimeSpan.Zero,
																new TimeSpan(1,0,0),
																new TimeSpan(2,0,0)};
			}
		}
	}
}
