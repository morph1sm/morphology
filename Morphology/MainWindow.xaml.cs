using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Morphology.Data;
using Newtonsoft.Json.Linq;

namespace Morphology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public Regions regions = null;
        ListBox dragSource = null;
        private SettingHandler<Settings> _currentSettingHandler;

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

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;
            dragSource = source;
            DragDrop.DoDragDrop(source, source.SelectedItems, DragDropEffects.Move);
        }
        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ListBoxItem dragged = (ListBoxItem)sender;

            // ensure that dragged items are selected, so the target list box 
            // knows which items are being transferred

            // This seems iffy
            //dragged.IsSelected = true;
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
            region.TransferMorphs(dragSource.SelectedItems);

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