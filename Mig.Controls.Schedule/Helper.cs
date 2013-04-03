/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 29.03.2013
 * Time: 21:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Media;

namespace Mig.Controls.Schedule
{
	/// <summary>
	/// Description of Helper.
	/// </summary>
	public class Helper
	{
        //public static T FindParent<T>(FrameworkElement element) where T : FrameworkElement
        //{
        //    for (FrameworkElement frameworkElement = VisualTreeHelper.GetParent(element) as FrameworkElement; frameworkElement != null; frameworkElement = (VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement))
        //    {
        //        T t = frameworkElement as T;
        //        if (t != null)
        //        {
        //            return t;
        //        }
        //    }
        //    return default(T);
        //}

        public static T FindParent<T>(DependencyObject obj) where T : DependencyObject
        {


            while (obj != null)
            {
                T o = obj as T;

                if (o != null)
                {
                    return o;
                }

                obj = VisualTreeHelper.GetParent(obj);

            }

            return null;

        }
	}
	
	
}
