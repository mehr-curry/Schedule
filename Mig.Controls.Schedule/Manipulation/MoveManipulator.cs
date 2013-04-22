/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 22.04.2013
 * Time: 07:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Manipulation
{
	/// <summary>
	/// Description of MoveManipulationBehavior.
	/// </summary>
	public class MoveManipulator
		: IManipulatorBehavior
	{
		public bool Manipulate(ScheduleItem item, UIElement manipulatorHost)
		{
			return false;
		}
	}
}
