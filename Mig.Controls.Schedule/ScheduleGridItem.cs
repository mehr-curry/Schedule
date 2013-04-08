/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 21:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of ScheduleGridItem.
	/// </summary>
	public class ScheduleGridItem : Control
	{
		public ScheduleGridItem()
		{
		}

        public ScheduleColumn Column { get; set; }
	}
}
