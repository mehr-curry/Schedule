/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 22.04.2013
 * Time: 07:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using Mig.Controls.Schedule.Interfaces;

namespace Mig.Controls.Schedule.Manipulation
{
	/// <summary>
	/// Description of ManipulationProperty.
	/// </summary>
	public static class ManipulatorPropertyExt
	{
		public static readonly DependencyProperty ManipulatorProperty =
			DependencyProperty.RegisterAttached("Manipulator", typeof(IManipulatorBehavior), typeof(ManipulatorPropertyExt),
			                            new FrameworkPropertyMetadata());
		
		public static IManipulatorBehavior GetManipulator(UIElement element){
			return (IManipulatorBehavior)element.GetValue(ManipulatorProperty);
		}
		
		public static void SetManipulator(UIElement element, IManipulatorBehavior value){
			element.SetValue(ManipulatorProperty, value);
		}
		
	}
}
