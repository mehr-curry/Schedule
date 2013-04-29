/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.04.2013
 * Time: 07:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace Mig.Controls.Menu
{
	/// <summary>
	/// Description of MenuExtensions.
	/// </summary>
	public class MenuExtensions
	{
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.RegisterAttached("Key", typeof(Guid?), typeof(MenuExtensions),
			                            new FrameworkPropertyMetadata(null));
		
		public static Guid? GetKey(FrameworkElement element)
		{
			return (Guid?)element.GetValue(KeyProperty);
		}
		
		public static void SetKey(FrameworkElement element, Guid? value)
		{
			element.SetValue(KeyProperty, value);
		}
	
	}
}
