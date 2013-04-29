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

namespace Mig.Controls.Customizing
{
	/// <summary>
	/// Description of MenuExtensions.
	/// </summary>
	public class CustomizingExtension
	{
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.RegisterAttached("Key", typeof(Guid?), typeof(CustomizingExtension),
			                            new FrameworkPropertyMetadata(null));

        public static Guid? GetKey(UIElement element)
		{
			return (Guid?)element.GetValue(KeyProperty);
		}

        public static void SetKey(UIElement element, Guid? value)
		{
			element.SetValue(KeyProperty, value);
		}
	
	}
}
