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
                //var rnd = new Random();
                //var result = new ObservableCollection<IEntry>();

                //for (int i = 0; i < 10; i++)
                //{
                //    var von = new TimeSpan(rnd.Next(0, 23), rnd.Next(0, 59), 0);

                //    result.Add(new Termin() {Datum = new DateTime(2013, 4, rnd.Next(DateTime.Today.Day, 12)),
                //                             Von = von,
                //                             Bis = new TimeSpan(rnd.Next(von.Hours, 23), rnd.Next(von.Minutes, 59), 0)});
                //}

                //return result;

                return new ObservableCollection<IEntry>() {
			                                                        new Termin() {
			                                                Datum = DateTime.Today,
			                                                Von = new TimeSpan(6,0,0), 
			                                                Bis = new TimeSpan(7,0,0)
			                                                        },
			                                                        new Termin() {
			                                                Datum = DateTime.Today.AddDays(1),
			                                                Von = new TimeSpan(7,0,0), 
			                                                Bis = new TimeSpan(8,0,0)
			                                                        },
			                                                        new Termin() {
			                                                Datum = DateTime.Today.AddDays(2),
			                                                Von = new TimeSpan(8,0,0), 
			                                                Bis = new TimeSpan(10,0,0)
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
