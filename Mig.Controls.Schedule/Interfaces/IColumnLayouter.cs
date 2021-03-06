﻿/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04/01/2013
 * Time: 20:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Layout
{
	public interface IColumnLayouter
	{
		Schedule Owner{ get; set;}
		void Calculate(ScheduleColumn column, double change);
		void SetAll(double width);
        IEnumerable<ScheduleColumn> GetVisibleColumns(Rect viewport);
	    double GetDesiredWidth();
	    object TranslateToSource(double horizontalValue);
	    double TranslateFromSource(object value);
	    ISnappingBehavior SnappingBehavior { get; set; }
        double DefaultWidth { get; }
	}
}
