/*
 * Created by SharpDevelop.
 * User: mercury
 * Date: 19.04.2013
 * Time: 07:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mig.Controls.Schedule.Interfaces
{
	/// <summary>
	/// Description of ISnappingBehavior.
	/// </summary>
	public interface ISnappingBehavior
	{
		Schedule Owner { get; set; }
		object TranslateToSource(double horizontalValue);
	    double TranslateFromSource(object value);
	}
}
