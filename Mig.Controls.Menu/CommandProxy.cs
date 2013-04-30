using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Mig.Controls.Customizing
{
    public class MenuProxy
    {
        public FrameworkElement Owner { get; set; }
        public Guid Key { get; set; }

        public static explicit operator Menu(MenuProxy source)
        {
            return source.Owner.FindResource(source.Key) as Menu;
        }
    }
}
