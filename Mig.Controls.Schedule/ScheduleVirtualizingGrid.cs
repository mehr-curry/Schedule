using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Mig.Controls.Schedule
{
    public class ScheduleVirtualizingGrid : VirtualizingPanel
    {
        private Schedule _owner;

        public ObservableCollection<ScheduleColumn> Columns { get; set; }
        public ObservableCollection<ScheduleRow> Rows { get; set; }

        public Schedule Owner
        {
            get { if(_owner == null) 
                    _owner = Helper.FindParent<Schedule>(this);

                //if (_owner != null)
                //    ItemContainerGenerator = _owner;

            return _owner;
            }
        }
    }
}
