using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        Point PP; // Mouse position for last PreviewMouseDown event
        private SettingHandler<Settings> _settings;
        private Window _dragdropWindow = null;
        private Dictionary<string, List<string>> _morph_references;

        public SettingHandler<Settings> Settings
        {
            get => _settings;
            set
            {
                _settings = value; 
                OnPropertyChanged();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            if (Settings.LoadedSettings.Folder != null)
            {
                // If a folder was selected in a previous session, 
                // reload the same folder when app starts up.
                LoadMorphFolder();
            }
            Settings.LoadedSettings.PropertyChanged += OnViewOptionChanged;
        }
        private void LoadSettings()
        {
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.xml");

            // Load settings store from auto-created XML settings file.
            Settings = new SettingHandler<Settings>(new FileInfo(settingsPath));
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
        private void StartDrag(ListBox listBox)
        {
            IList Selection = listBox.SelectedItems;

            //Are we Dragging anything?
            if (Selection.Count == 0) return;
            
            var items = listBox.SelectedItems.Cast<Morph>().ToList();

            List<FrameworkElement> itemvisuals = new List<FrameworkElement>();

            items.ForEach(x =>
            {
                //Find each of the selected Items in the visual Tree of the ListBox
                itemvisuals.Add(listBox.ItemContainerGenerator.ContainerFromItem(x) as FrameworkElement);
            });

            //Create a Visual Representation of the Items beeing dragged
            CreateDragDropWindow(itemvisuals);

            //Do the REAL Drag & Drop
            DragDrop.DoDragDrop(listBox, items,DragDropEffects.Copy | DragDropEffects.Move);

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
                Rectangle r = new Rectangle
                {
                    Width = x.ActualWidth,
                    Height = x.ActualHeight,
                    Fill = new VisualBrush(x)
                };
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
            ListBox listBox = (ListBox)sender;

            listBox.Tag = null;

            PP = e.GetPosition(listBox);

            ListBoxItem Item = (ListBoxItem)VisualTree.GetParent(e.OriginalSource, typeof(ListBoxItem));

            if (Item != null)
            {
                if (Item.IsSelected && listBox.CaptureMouse())
                {
                    //e.Handled = true;
                    listBox.Tag = Item;
                }
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

            ListBox listBox = (ListBox)sender;
            ListBoxItem item = (ListBoxItem)listBox.Tag;

            listBox.Tag = null;

            if (item != null && listBox.IsMouseCaptured)
            {
                //e.Handled = true;

                listBox.ReleaseMouseCapture(); // Release Mouse Capture

                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    item.IsSelected = !item.IsSelected;
                }
                else
                {
                    listBox.SelectedItems.Clear();
                    item.IsSelected = true;
                }

                if (!item.IsKeyboardFocused) item.Focus();
            }
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox listView = (ListBox)sender;

            if (listView.IsMouseCaptured)
            {
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
        }

        private void ListBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            //Cleanup Variables
            ListBox listView = (ListBox)sender;
            listView.Tag = null;
        }
        private void ListBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_dragdropWindow != null)
            {

                // update the position of the visual feedback item
                Win32Point w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);

                int mousecursorOffsetX = 20; //The Offset from the Mousecursor X Position
                int mousecursorOffsetY = 20; //The Offsetfrom the Mousecursor Y Position

                _dragdropWindow.Left = w32Mouse.X + mousecursorOffsetX;
                _dragdropWindow.Top = w32Mouse.Y + mousecursorOffsetY;
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListBox listView = (ListBox)sender;
            Region region = (Region)regionsListBox.SelectedItem;
            int total = listView.Items.Count;
            int selected = listView.SelectedItems.Count;

            string s = total == 1 ? "" : "s";
            string regionInfo = "";
            string selectedInfo = "";

            if (selected > 0)
            {
                groupMorphActions.Visibility = Visibility.Visible;
                selectedInfo = String.Format(", {0}/{1} selected", selected, total);
            }
            else
            {
                groupMorphActions.Visibility = Visibility.Collapsed;
            }

            if (!region.IsRoot)
            {
                regionInfo = " in category \"" + region.Name + "\"";
            }
            
            MorphInfo.Text = total + " custom morph" + s + regionInfo + selectedInfo;
        }
        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ListBox l = (ListBox)sender;
            Morph morph = (Morph)l.SelectedItem;
 
            MessageBox.Show(morph.Details, "Morp Detail");//, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RegionListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListBox list = (ListBox)sender;
            Region region = (Region)list.SelectedItem;
            
            if (region.IsStandard)
            {
                RenameRegionPanel.Visibility = Visibility.Collapsed;
                RegionName.Text = "";
            }
            else
            {
                RenameRegionPanel.Visibility = Visibility.Visible;
                RegionName.Text = region.Name;
            }

            MorphList.SelectAll();
            MorphList.SelectedItems.Clear();
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
        }
        internal void ScanSavesFolder(string dir)
        {
            try
            {
                foreach (string sub in Directory.GetDirectories(dir))
                {
                    foreach (string filepath in Directory.GetFiles(sub, "*.json"))
                    {
                        string morphListJSON="";
                        var match = Regex.Match("","");
                        try
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                this.Title = "Morphology - Scanning " + filepath;
                            });

                            // Example scene/look json
                            // {
                            //   ...
                            //   "atoms": [
                            //     ...
                            //     {
                            //       "id": "Girly Bob",
                            //       "type": "Person",
                            //       "storables": [
                            //         ... 
                            //         { 
                            //           "id" : "geometry",
                            //           "character": "Candy",
                            //           "clothing": [ ],      // naked!
                            //           "hair": [ ]           // bald!
                            //           "morphs": [           // interesting bits!
                            //             ...
                            //             { "name": "Jaw Round", "value": "0.31231625" },
                            //             { "name": "Jaw Width", "value": "-0.93873625" },
                            //             { "name": "Jaw Height", "value": "-0.36259387" },
                            //             ...
                            //           ]
                            //         }
                            //       ]
                            //     } 
                            //   ]
                            // }
                            //
                            // Instead of parsing the whole scene, cut out just the morphs list to speed up evaluation.
                            string json = File.ReadAllText(filepath);
                            int morphsPosition = json.IndexOf("\"morphs\"");
                            if (morphsPosition > 0)
                            {
                                int morphListStart = json.IndexOf('[', morphsPosition);
                                morphListJSON = json.Substring(morphListStart);

                                // Find the next closing bracket that had some sort of white-space in front of it.
                                // This avoids clipping the json array too early like in cases of brackets in names.
                                // Thanks, "[Alter3go]". :p
                                match = Regex.Match(morphListJSON, @"\s+]");
                                morphListJSON = morphListJSON.Substring(0, match.Index+match.Length);

                                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(morphListJSON);
                                foreach (dynamic morph in jsonObj)
                                {
                                    string name = morph["name"];
                                    if (!_morph_references.ContainsKey(name))
                                    {
                                        _morph_references[name] = new List<string>();
                                    }
                                    _morph_references[name].Add(filepath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not load JSON from:\n\n" + filepath + "\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    ScanSavesFolder(sub);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not scan the selected folder.\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadMorphFolder()
        {
            string folder = Settings.LoadedSettings.Folder;
            if (folder != null) {
                this.Title = "Morphology - " + folder;
                regions = new Regions(Settings.LoadedSettings, _morph_references);
                DataContext = regions;
            }
        }
        internal void TrimMorphsInSavesFolder(string dir)
        {
            try
            {
                foreach (string sub in Directory.GetDirectories(dir))
                {
                    foreach (string filepath in Directory.GetFiles(sub, "*.json"))
                    {
                        string morphListJSON = "";
                        var match = Regex.Match("", "");
                        try
                        {
                            Console.WriteLine("Scanning save {0}", filepath);

                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                this.Title = "Morphology - Scanning " + filepath;
                            });

                            // Example scene/look json
                            // {
                            //   ...
                            //   "atoms": [
                            //     ...
                            //     {
                            //       "id": "Girly Bob",
                            //       "type": "Person",
                            //       "storables": [
                            //         ... 
                            //         { 
                            //           "id" : "geometry",
                            //           "character": "Candy",
                            //           "clothing": [ ],      // naked!
                            //           "hair": [ ]           // bald!
                            //           "morphs": [           // interesting bits!
                            //             ...
                            //             { "name": "Jaw Round", "value": "0.31231625" },
                            //             { "name": "Jaw Width", "value": "0.0" },   <--- trim this one
                            //             { "name": "Jaw Height", "value": "-0.36259387" },
                            //             ...
                            //           ]
                            //         }
                            //       ]
                            //     } 
                            //   ]
                            // }
                            //
                            /*
                            if (jsonObj.ContainsKey("atoms"))
                            {
                                foreach (dynamic atom in jsonObj["atoms"])
                                {
                                    if (atom["type"] == "Person")
                                    {
                                        foreach (dynamic storable in atom["storables"])
                                        {
                                            if (storable["id"] == "geometry")
                                            {
                                                foreach (dynamic morph in storable["morphs"])
                                                {
                                                    string name = morph["name"];
                                                    if (!_morph_references.ContainsKey(name))
                                                    {
                                                        _morph_references[name] = new List<string>();
                                                    }
                                                    _morph_references[name].Add(filepath);
                                                }
                                            }
                                        }
                                    }
                                }
                            }*/
                            // Instead of parsing the whole scene, cut out just the morphs list to speed up evaluation.
                            string json = File.ReadAllText(filepath);
                            int morphsPosition = json.IndexOf("\"morphs\"");
                            if (morphsPosition > 0)
                            {
                                int morphListStart = json.IndexOf('[', morphsPosition);
                                morphListJSON = json.Substring(morphListStart);

                                // Find the next closing bracket that had some sort of white-space in front of it.
                                // This avoids clipping the json array too early like in cases of brackets in names.
                                // Thanks, "[Alter3go]". :p
                                match = Regex.Match(morphListJSON, @"\s+]");
                                morphListJSON = morphListJSON.Substring(0, match.Index + match.Length);

                                dynamic original = Newtonsoft.Json.JsonConvert.DeserializeObject(morphListJSON);
                                ListDictionary trimmed = new ListDictionary();  
                                foreach (dynamic morph in original)
                                {
                                    string name = morph["name"];
                                    string value = morph["value"];

                                    if (!_morph_references.ContainsKey(name))
                                    {
                                        _morph_references[name] = new List<string>();
                                    }
                                    _morph_references[name].Add(filepath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not load JSON from:\n\n" + filepath + "\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    ScanSavesFolder(sub);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not scan the selected folder.\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Settings.LoadedSettings.Folder ?? "";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Settings.LoadedSettings.Folder = dialog.SelectedPath;
                    LoadMorphFolder();
                }
            }
        }
        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadMorphFolder();
        }
        private void OnNewRegion(object sender, RoutedEventArgs e)
        {
            string regionName = NewRegionName.Text;
            foreach (Region region in regions) {
                if (region.Name == regionName)
                {
                    MessageBox.Show("A category with that name already exists. Please pick another name.", "Morphology - Region Name Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            regions.Add(new Region(regionName));
        }
        private async void OnRenameRegion(object sender, RoutedEventArgs e)
        {
            string regionName = RegionName.Text;
            Region selected = (Region)regionsListBox.SelectedItem;

            foreach (Region region in regions)
            {
                if (region.Name == regionName)
                {
                    MessageBox.Show("A category with that name already exists. Please pick another name.", "Morphology - Region Name Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            await Task.Run(() => RenameRegion(selected, regionName));
            LoadMorphFolder();
        }
        private void RenameRegion(Region region, string name) 
        {
            region.Name = name;
            // Save all morphs in region with the new region name
            foreach (Morph morph in region.Morphs)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.Title = "Morphology - Saving " + morph.Filepath;
                });
                morph.Save();
            }
        }
        private void OnDeleteDSF(object sender, RoutedEventArgs e)
        {
            List<string> dsfFiles = GetDSFFilePaths(regions.MorphFolder);

            if (dsfFiles.Count > 0)
            {
                if (MessageBox.Show("Delete " + dsfFiles.Count + " DSF files from morph folder?", "Morphology", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (string file in dsfFiles)
                    {
                        File.Delete(file);
                    }
                }
            } else
            {
                MessageBox.Show("No DSF files where found in the selected morph folder.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private List<string> GetDSFFilePaths(string dir)
        {
            List<string> dsfFiles = new List<string>();
            try
            {
                foreach (string sub in Directory.GetDirectories(dir))
                {
                    dsfFiles.AddRange(Directory.GetFiles(sub, "*.dsf"));
                    dsfFiles.AddRange(GetDSFFilePaths(sub));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dsfFiles;
        }
        private void OnDeleteAUTO(object sender, RoutedEventArgs e)
        {
            List<string> morphFiles = GetAutoMorphFilePaths(regions.MorphFolder);

            if (morphFiles.Count > 0)
            {
                if (MessageBox.Show("Delete " + morphFiles.Count + " AUTO morphs from morph folder?", "Morphology", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (string file in morphFiles)
                    {
                        // delete both the meta data and binary morph files
                        File.Delete(file);
                        File.Delete(file.Replace(".vmi", ".vmb"));
                    }

                    // refresh region list
                    LoadMorphFolder();
                }
            }
            else
            {
                MessageBox.Show("No AUTO morph files where found in the selected morph folder.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private List<string> GetAutoMorphFilePaths(string dir)
        {
            List<string> morphFiles = new List<string>();
            try
            {
                foreach (string sub in Directory.GetDirectories(dir))
                {
                    if (sub.EndsWith("\\AUTO"))
                    {
                        morphFiles.AddRange(Directory.GetFiles(sub, "*.vmi"));
                    }
                    morphFiles.AddRange(GetAutoMorphFilePaths(sub));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return morphFiles;
        }
        private void OnAutoRegion(object sender, RoutedEventArgs e)
        {
            List<Morph> autoMorphs = regions.GetAutoMorphs();

            if (autoMorphs.Count > 0)
            {
                if (MessageBox.Show("Move " + autoMorphs.Count + " AUTO morphs to the AUTO category?", "Morphology", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Region auto = regions.FirstOrDefault(r => r.Name == "AUTO");

                    if (auto == null)
                    {
                        auto = new Region("AUTO");
                        regions.Add(auto);
                    }

                    foreach (Morph morph in autoMorphs)
                    {
                        // delete both the meta data and binary morph files
                        morph.MoveToRegion(auto);
                        morph.Save();
                    }

                    // refresh region list
                    LoadMorphFolder();
                }
            }
            else
            {
                MessageBox.Show("No AUTO morphs where found in any category.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private async void OnFindMorphReferences(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Settings.LoadedSettings.SavesFolder ?? Path.Combine(Settings.LoadedSettings.Folder, "Saves");
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Settings.LoadedSettings.SavesFolder = dialog.SelectedPath;

                    _morph_references = new Dictionary<string, List<string>>();

                    await Task.Run(() => ScanSavesFolder(dialog.SelectedPath));

                    // reload morph folder to apply saves reference info to each morph
                    LoadMorphFolder();
                    GroupSaveActions.Visibility = Visibility.Visible;
                    GroupMorphReferences.Visibility = Visibility.Visible;
                }
            }

        }         
        private void OnRemoveInactiveMorphs(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry, this isn't implemented yet.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void OnRemoveMorphArtifacts(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry, this isn't implemented yet.", "Morphology", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void OnFilterMorphsByName(object sender, RoutedEventArgs e)
        {
            LoadMorphFolder();
        }
        private async void OnTrashMorph(object sender, RoutedEventArgs e)
        {
            List<string> paths = new List<string>();
            foreach (Morph morph in MorphList.SelectedItems)
            {
                paths.Add(morph.Filepath);
            }

            string vamFolder = Settings.LoadedSettings.Folder;
            string trashFolder = Path.Combine(vamFolder, "Custom\\Atom\\Person\\MorphsTrash");

            await Task.Run(() => MoveMorphsToFolder(paths, trashFolder));

            LoadMorphFolder();
        }
        private void MoveMorphsToFolder(List<string> selection, string folder) {
            string vamFolder = Settings.LoadedSettings.Folder;
            string morphFolder = Path.Combine(vamFolder, "Custom\\Atom\\Person\\Morphs");
            int subpathIndex = morphFolder.Length+1;            

            foreach (string source in selection)
            {
                if (source.Replace('/', '\\').StartsWith(morphFolder))
                {
                    string subpath = source.Substring(subpathIndex);
                    string destination = Path.Combine(folder, subpath);
                    string destinationFolder = Path.GetDirectoryName(destination);

                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }

                    try
                    {
                        // This will overwrite a possible duplicate file in the trash, e.g. when the same morph is deleted more than once.
                        File.Copy(source, destination, true);
                        File.Delete(source);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not move morph to trash.\n\n" + source + "\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }
        private void OnViewOptionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.StartsWith("Show"))
            {
                LoadMorphFolder();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}