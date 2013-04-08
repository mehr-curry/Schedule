/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 04.04.2013
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mig.Controls.Schedule.Interfaces
{
	public interface IDataItem
	{
		object HorizontalValue { get; }
		object VerticalStartValue { get; }
        object VerticalEndValue { get; }
	}
}
