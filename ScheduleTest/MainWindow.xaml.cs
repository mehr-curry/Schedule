using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mig.Controls.Schedule;

namespace ScheduleTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            object t = "123";

            txt = "123";
            MockUp = new DataMockUp();
            ViewModel = new UiViewModel();

            InitializeComponent();
            
            for(int i = 0; i < 10; i++)
            	Debug.WriteLine(Guid.NewGuid());

            _schedule.ItemsSource = MockUp.EntriesTermin;
            //lb.ItemsSource = mockUp.EntriesTermin;
            //HorizontalHeaderHost.ItemsSource = new ObservableCollection<ScheduleColumn>() {new ScheduleColumn(),new ScheduleColumn()};
            //_schedule.ItemsSource = mockUp.EntriesTermin;
//            _schedule.HorizontalHeaderSource = mockUp.HorizontalHeaderDatum;
//            _schedule.VerticalHeaderSource = mockUp.VerticalHeaderZeit;

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }




        public UiViewModel ViewModel
        {
            get { return (UiViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(UiViewModel), typeof(MainWindow), new UIPropertyMetadata(null));

        

        public DataMockUp MockUp
        {
            get { return (DataMockUp)GetValue(MockUpProperty); }
            set { SetValue(MockUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MockUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MockUpProperty =
            DependencyProperty.Register("MockUp", typeof(DataMockUp), typeof(MainWindow), new UIPropertyMetadata(null));

        
        
        public string txt { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = new Button() {Content = "lala", Width = 100, Height = 100};
            grd.Children.Add(btn);
            Grid.SetRow(btn, 3);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListBox;

            if (lv != null)
            {
                if (Equals(lv, lvMa))
                    ViewModel.SelectedMas = lv.SelectedItems.Cast<Ma>().ToArray();
                else if (Equals(lv, lvAp))
                    ViewModel.SelectedAps = lv.SelectedItems.Cast<Ap>().ToArray();
                else if (Equals(lv, lvPa))
                    ViewModel.SelectedPas = lv.SelectedItems.Cast<Pa>().ToArray();
                //else if (Equals(lv, lvLe))
                //    ViewModel.SelectedLes = lv.SelectedItems.Cast<Le>().ToArray();
            }
        }


        
//        private void setTrue(object sender, System.Windows.RoutedEventArgs e)
//{
//    Grid.SetIsSharedSizeScope(dp1, true);
//    txt1.Text = "IsSharedSizeScope Property is set to " + Grid.GetIsSharedSizeScope(dp1).ToString();
//}
//
//private void setFalse(object sender, System.Windows.RoutedEventArgs e)
//{
//    Grid.SetIsSharedSizeScope(dp1, false);
//    txt1.Text = "IsSharedSizeScope Property is set to " + Grid.GetIsSharedSizeScope(dp1).ToString();
//}


//        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            DragDrop.DoDragDrop((DependencyObject) sender, sender, DragDropEffects.Copy);
//            Border b = (Border) sender;
//            SetGridLocation(b, new Point(Grid.GetColumn(b), Grid.GetRow(b)));
//        }
//
//        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//
//        }
//
//        private void Grid_Drop(object sender, DragEventArgs e)
//        {
//            Border item = e.Data.GetData(typeof(Border)) as Border;
//        }
//
//        private void Grid_DragOver(object sender, DragEventArgs e)
//        {
//            Border item = e.Data.GetData(typeof (Border)) as Border;
//            Point pos = e.GetPosition(grd);
//            double left = 0D;
//            double top = 0D;
//            int colIdx = -1;
//            int rowIdx = -1;
//
//            foreach (ColumnDefinition def in grd.ColumnDefinitions)
//            {
//                colIdx++;
//                if (pos.X >= left && pos.X <= left + def.ActualWidth)
//                    break;
//                left += def.ActualWidth;
//            }
//            foreach (RowDefinition def in grd.RowDefinitions)
//            {
//                rowIdx++;
//                if (pos.Y >= top && pos.Y <= top + def.ActualHeight)
//                    break;
//                top += def.ActualHeight;
//            }
//            //int colIdx = grd.ColumnDefinitions.TakeWhile(def => !(pos.X > left) || !(pos.X < left + def.ActualWidth)).Count();
//            //int rowIdx = grd.RowDefinitions.TakeWhile(def => !(pos.Y > top) || !(pos.Y < top + def.ActualHeight)).Count();
//            Trace.WriteLine(string.Format("{0}:{1}",colIdx,rowIdx));
//            if (item != null)
//            {
//                Grid.SetColumn(item, colIdx);
//                Grid.SetRow(item, rowIdx);
//            }
//            
//            
//        }
//
//        private void grd_DragEnter(object sender, DragEventArgs e)
//        {
//            e.Effects = DragDropEffects.Copy;
//        }
//
//
//
//        public static Point GetGridLocation(DependencyObject obj)
//        {
//            return (Point)obj.GetValue(GridLocationProperty);
//        }
//
//        public static void SetGridLocation(DependencyObject obj, Point value)
//        {
//            obj.SetValue(GridLocationProperty, value);
//        }
//
//        public static readonly DependencyProperty GridLocationProperty =
//            DependencyProperty.RegisterAttached("GridLocation", typeof(Point), typeof(MainWindow), new UIPropertyMetadata(new Point(0,0)));
//
//        private void grd_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
//        {
//            if (e.EscapePressed)
//            {
//                Border b = (Border)e.OriginalSource;
//                e.Action = DragAction.Cancel;
//                Grid.SetColumn(b, Convert.ToInt32(GetGridLocation(b).X));
//                Grid.SetRow(b, Convert.ToInt32(GetGridLocation(b).Y));
//            }
//        }
		
//		private double _start = 0.0;
//		void thumb1_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
//		{
//			var thumb = sender as Thumb;
//			
//			if(thumb != null)
//				_start = Helper.FindParent<ContentPresenter>(thumb).ActualWidth;
//			else
//				_start = bd1.ActualWidth;
//		}
//		
//		void thumb1_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
//		{
//			var thumb = sender as Thumb;
//			Helper.FindParent<ContentPresenter>(thumb).Width += e.HorizontalChange;
//		}
//		
//		void thumb1_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
//		{
//			
//		}
//		
//		void thumb2_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
//		{
//			_start = bd2.ActualWidth;
//		}
//		
//		void thumb2_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
//		{
//			bd2.Width += e.HorizontalChange;
//		}
//		
//		void thumb2_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
//		{
//			
//		}
    }
}
