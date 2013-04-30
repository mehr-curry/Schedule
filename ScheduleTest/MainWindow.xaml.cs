using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Mig.Controls.Customizing;
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

            Loaded += new RoutedEventHandler(MainWindow_Loaded);

            //var p = new MenuProxy();
            //p.Key =
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dictionary<Guid, UIElement> commandingItems = new Dictionary<Guid, UIElement>();
            CollectCommandingItems(tray.ToolBars, commandingItems);

        }


        private void CollectCommandingItems(IEnumerable ic, Dictionary<Guid, UIElement> commandingItems)
        {
            foreach (var child in ic.OfType<UIElement>())
            {
                var key = CustomizingExtension.GetKey(child);
                var childIc = child as ItemsControl;

                if (key != null)
                    commandingItems.Add(key.Value, child);
                
                if (childIc != null && childIc.Items.Count > 0)
                    CollectCommandingItems(childIc.Items, commandingItems);
            }
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

        private void miDatei_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miSpeichern_Click(object sender, RoutedEventArgs e)
        {

            using (var sw = new XmlTextWriter(File.Open(@"c:\temp\toolbar_test2.xml", FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {
                sw.Formatting = Formatting.Indented;
                sw.Indentation = 1;
                sw.IndentChar = '\t';
                XamlWriter.Save(tray, sw);
                sw.Flush();
            }

            return;

            //var ms = new MemoryStream();
            using (
                var xmlwriter =
                    new XmlTextWriter(
                        File.Open(@"c:\temp\toolbar_test.xml", FileMode.Create,
                                  FileAccess.ReadWrite), Encoding.UTF8)
                        {
                            Formatting = Formatting.Indented,
                            Indentation = 1,
                            IndentChar = '\t'
                        })
            {
                xmlwriter.WriteStartDocument(true);
                xmlwriter.WriteStartElement("toolbars_and_menus");
                WriteCommandingItemsRecursive(tray.ToolBars, xmlwriter);
                xmlwriter.WriteEndElement(); // "toolbars_and_menus"
                xmlwriter.WriteEndDocument();

                xmlwriter.Flush();
            }
            //ms.Seek(0, SeekOrigin.Begin);

            //var sr = new StreamReader(ms);

            //Debug.WriteLine(sr.ReadToEnd());
        }

        private void WriteCommandingItemsRecursive(IEnumerable ic, XmlWriter xw)
        {
            foreach (var child in ic.OfType<FrameworkElement>())
            {
                xw.WriteStartElement(child.GetType().Name);

                var key = CustomizingExtension.GetKey(child);
                var childIc = child as ItemsControl;

                if (key != null)
                {
                    WriteAttribute(xw, "Key", key);
    
                    if(!double.IsNaN(child.Width))
                        WriteAttribute(xw, "Width", child.Width);

                    WriteAttribute(xw, "Visibility", child.Visibility);

                    if (child is ButtonBase)
                        WriteAttribute(xw, "Content", ((ButtonBase) child).Content);
                    else if(child is MenuItem)
                        WriteAttribute(xw, "Header", ((MenuItem)child).Header);
                    //Debug.WriteLine(child);
                }
                
                if (childIc != null && childIc.Items.Count > 0)
                    WriteCommandingItemsRecursive(childIc.Items, xw);

                xw.WriteEndElement();
            }
        }

        private static void WriteAttribute(XmlWriter xw, string localName, object value)
        {
            xw.WriteStartAttribute(localName);
            xw.WriteValue(value.ToString());
            xw.WriteEndAttribute();
        }

        private void miOeffnen_Click(object sender, RoutedEventArgs e)
        {
            using (var sr = new StreamReader(File.Open(@"c:\temp\toolbar_bhpl.xaml", FileMode.Open, FileAccess.Read)))
            {
                Dictionary<Guid, UIElement> originals = new Dictionary<Guid, UIElement>();
                CollectCommandingItems(tray.ToolBars, originals);
                
                var toolbar = (ToolBarTray)XamlReader.Parse(sr.ReadToEnd());
                var idx = grd.Children.IndexOf(tray);
                grd.Children.Remove(tray);

                Dictionary<Guid, UIElement> customs = new Dictionary<Guid, UIElement>();
                CollectCommandingItems(toolbar.ToolBars, customs);

                bool t = originals.Keys.SequenceEqual(customs.Keys);

                if (!t)
                {
                    AddToCustom(originals, customs);
                    RemoveFromCustom(originals, customs);
                }
                tray = toolbar;
                grd.Children.Insert(idx, tray);

            }
        }

        private static void AddToCustom(Dictionary<Guid, UIElement> originals, Dictionary<Guid, UIElement> customs)
        {
            foreach (var addedKey in originals.Keys.Except(customs.Keys))
            {
                var addedElement = (FrameworkElement) originals[addedKey];
                var parentKey = CustomizingExtension.GetKey((FrameworkElement) addedElement.Parent);

                if (parentKey != null)
                {
                    var originalParent = originals[parentKey.Value];
                    var customParent = customs[parentKey.Value];
                    var originalIdx = -1;

                    if (originalParent is ItemsControl)
                        originalIdx = ((ItemsControl) originalParent).Items.IndexOf(addedElement);

                    if (customParent is ItemsControl)
                    {
                        var clone = (UIElement)XamlReader.Parse(XamlWriter.Save(addedElement));
                    
                        if (originalIdx > -1)
                            ((ItemsControl) customParent).Items.Insert(originalIdx, clone);
                        else
                            ((ItemsControl) customParent).Items.Add(clone);

                        customs.Add(addedKey, clone);
                    }
                }
            }
        }

        private static void RemoveFromCustom(Dictionary<Guid, UIElement> originals, Dictionary<Guid, UIElement> customs)
        {
            var removedKeys = customs.Keys.Except(originals.Keys);

            for (int i = removedKeys.Count()-1; i>= 0; i--)
            {
                var removedKey = removedKeys.ElementAt(i);
                var removedElement = (FrameworkElement) customs[removedKey];
                var parentKey = CustomizingExtension.GetKey((FrameworkElement)removedElement.Parent);
                if (parentKey != null)
                {
                    var customParent = customs[parentKey.Value];

                    if (customParent is ItemsControl)
                        ((ItemsControl) customParent).Items.Remove(removedElement);

                    customs.Remove(removedKey);
                }
            }
        }

        private void miNeu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
