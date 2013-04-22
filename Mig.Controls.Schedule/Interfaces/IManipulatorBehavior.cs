/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 22.04.2013
 * Time: 07:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace Mig.Controls.Schedule.Interfaces
{
	/// <summary>
	/// Description of IManipulationBehavior.
	/// </summary>
	public interface IManipulatorBehavior
	{
		bool Manipulate(ScheduleItem item, UIElement manipulatorHost);
	}
}
