﻿/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 20:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;

namespace Mig.Controls.Schedule.Layout
{
	public interface IColumnLayouter
	{
		Schedule Owner{ get; set;}
		void Calculate(ScheduleColumn column, double change);
	}
}
