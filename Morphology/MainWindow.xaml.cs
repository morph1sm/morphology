using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Xsl;
using Morphology.Data;
using Newtonsoft.Json.Linq;
using Path = System.IO.Path;

namespace Morphology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public Regions regions = null;
        ListBox dragSource = null;
        private ListBoxItem hitListBoxItem = null;
        Point PP; // Mouse position for last PreviewMouseDown event
        private SettingHandler<Settings> _currentSettingHandler;
        private Window _dragdropWindow = null;


        public SettingHandler<Settings> CurrentSettingHandler
        {
            get => _currentSettingHandler;
            set
            {
                _currentSettingHandler = value; 
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var settingfilename = "appsettings.xml";
            var fullsettingpath = Path.Combine(baseDirectory, settingfilename);
            CurrentSettingHandler = new SettingHandler<Settings>(new FileInfo(fullsettingpath));
            if (!string.IsNullOrEmpty(CurrentSettingHandler.LoadedSettings.CurrentlySelectedFolder) )
            {
                regions = new Regions(CurrentSettingHandler.LoadedSettings.CurrentlySelectedFolder);
                DataContext = regions;
            }
        }

        //DLL Imports for External Mouse Point Tracking
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        private void StartDrag(ListBox listView)
        {
            IList Selection = listView.SelectedItems;

            if (Selection.Count == 0)

                return;
            
            var items = listView.SelectedItems.Cast<Morph>().ToList();

            List<FrameworkElement> itemvisuals = new List<FrameworkElement>();

            items.ForEach(x =>
            {
                itemvisuals.Add(listView.ItemContainerGenerator.ContainerFromItem(x) as FrameworkElement);
            });

            CreateDragDropWindow(itemvisuals);


            DragDrop.DoDragDrop(listView, items,DragDropEffects.Copy | DragDropEffects.Move);

            if (_dragdropWindow == null) return;
            //Handle the Drag-Window
            _dragdropWindow.Close();
            _dragdropWindow = null;
        }

        /// <summary>
        /// Creates Drag and Drop Windows. In this case from a List of FrameworkElements
        /// </summary>
        /// <param name="dragElements"></param>
        private void CreateDragDropWindow(List<FrameworkElement> dragElements)
        {
            //Create a new Drag&Drop Window
            _dragdropWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                AllowDrop = false,
                Background = null,
                IsHitTestVisible = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Topmost = true,
                ShowInTaskbar = false
            };

            //Wrap the Elements in a Stackpanel.
            StackPanel panel = new StackPanel();

            //Create a Rectangle for each Visual Representation of the Control, that is being dragged.
            dragElements.ForEach(x =>
            {
                Rectangle r = new Rectangle();
                r.Width = x.ActualWidth;
                r.Height = x.ActualHeight;
                r.Fill = new VisualBrush(x);
                panel.Children.Add(r);
            });
            
            _dragdropWindow.Content = panel;

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            _dragdropWindow.Left = w32Mouse.X;
            _dragdropWindow.Top = w32Mouse.Y;
            _dragdropWindow.Show();
        }

        private void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBox listView = (ListBox)sender;

            listView.Tag = null;

            PP = e.GetPosition(listView);

            ListBoxItem Item = (ListBoxItem)VisualTree.GetParent(

                e.OriginalSource, typeof(ListBoxItem));


            if (Item == null) return;
            if (Item.IsSelected && listView.CaptureMouse())
            {
                e.Handled = true;
                listView.Tag = Item;

            }
        }

        private void ListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            // remove the visual feedback drag and drop item
            if (_dragdropWindow != null)
            {
                _dragdropWindow.Close();
                _dragdropWindow = null;
            }

            ListBox listView = (ListBox)sender;

            ListBoxItem Item = (ListBoxItem)listView.Tag;

            listView.Tag = null;

            if (Item == null)

                return;

            if (!listView.IsMouseCaptured)

                return;

            e.Handled = true;

            listView.ReleaseMouseCapture();

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Item.IsSelected = !Item.IsSelected; 
            }
            else
            {
                listView.SelectedItems.Clear();

                Item.IsSelected = true;
            }

            if (!Item.IsKeyboardFocused) Item.Focus();
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox listView = (ListBox)sender;

            if (!listView.IsMouseCaptured)

                return;
            e.Handled = true;
            Point P = e.GetPosition(listView);

            int Limit = 4;

            if (P.X - Limit > PP.X ||
                P.X + Limit < PP.X ||
                P.Y - Limit > PP.Y ||
                P.Y + Limit < PP.Y)
            {
                listView.ReleaseMouseCapture();
                // create the visual feedback drag and drop item
                StartDrag(listView);
            }
        }

        private void ListBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ListBox listView = (ListBox)sender;

            listView.Tag = null;
        }

        private void ListBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // update the position of the visual feedback item
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            if (_dragdropWindow != null)
            {
                _dragdropWindow.Left = w32Mouse.X + 20;
                _dragdropWindow.Top = w32Mouse.Y + 20;
            }
        }


        private void Region_DragEnter(object sender, DragEventArgs args)
        {
        }
        private void Region_DragLeave(object sender, DragEventArgs args)
        {
        }
        private void Region_Drop(object sender, DragEventArgs args)
        {
            // Drag event is captured by the whole list item (Grid element)
            Grid parent = (Grid)sender;
            // Find the Region instance from the DataContext of a bound element
            TextBlock textblock = (TextBlock)parent.Children[1];
            // convert DataContext to Type Region, so we can call methods on it
            Region region = (Region)textblock.DataContext;

            Console.WriteLine("Region received drop " + region);

            // transfer the selected Morphs from whatever region their located in 
            // to the region they were dropped on

            var dragdata = (List<Morph>)(args.Data.GetData(typeof(List<Morph>)));

            region.TransferMorphs(dragdata);

            if (SettingHandler<Settings>.CurrentInstance.LoadedSettings.AutoApplyChanges)
            {
                OnSave(null, null);
            }
            else
            {
                // change button caption to indicate that there are changes to be saved
                SaveButton.Content = "Apply Changes";
            }
        }
        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = CurrentSettingHandler.LoadedSettings.CurrentlySelectedFolder ?? "";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // e.g."E:/VAM/Custom/Atom/Person/Morphs"
                    regions = new Regions(dialog.SelectedPath); 
                    DataContext = regions;
                    CurrentSettingHandler.LoadedSettings.CurrentlySelectedFolder = dialog.SelectedPath;
                }
            }
        }
        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (regions == null)
            {
                MessageBox.Show("Please select your morph folder location first.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                regions.ApplyAllChanges();
                SaveButton.Content = "Refresh";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}