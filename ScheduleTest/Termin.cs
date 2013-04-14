/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 03/28/2013
 * Time: 23:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using Mig.Controls.Schedule.Interfaces;

namespace ScheduleTest
{
	/// <summary>
	/// Description of Termin.
	/// </summary>
	public class Termin : IEntry, IDataItem, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		public TimeSpan Von { get; set; }
		public TimeSpan Bis { get; set; }
		public DateTime Datum { get; set; }

		public object HorizontalValue { get { return Datum; } set {Datum = (DateTime)value; OnPropertyChanged("HorizontalValue"); }}
		public object VerticalStartValue { get { return Von; } set {Von = (TimeSpan)value; OnPropertyChanged("VerticalStartValue"); } }
        public object VerticalEndValue { get { return Bis; } set {Bis = (TimeSpan)value; OnPropertyChanged("VerticalEndValue"); } }
        
        protected virtual void OnPropertyChanged(string propName){
        	if(PropertyChanged != null)
        		PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
	}
}
