// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace Morphology
{
public class Morph : INotifyPropertyChanged
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _region;
        private readonly string _filepath;
        private readonly bool _bad;
        private readonly bool _auto;

        private Region _parent;
        private Brush _displayColor = Brushes.LightGray;
        private readonly List<string> _badMorphs = new List<string>(){
            "GXF_G2F_TransZUpJaw",
            "GXF_G2F_TransZLowJaw",
            "DollHead",
            "Lower head small",
            "Old",
            "Young",
            "Thin",
            "Face young",
            "MCMJulieFingersFistL",
            "MCMJulieFingersFistR",
            "MCMJulieThumbFistL",
            "MCMJulieThumbFistR",
            "AAdream",
            "Chest small X",
            "Chest small Y",
            "Chest small Z",
            "Hip line up",
            "Hip up",
            "Face thin"
        };
        public Morph()
        {

        }
        public Morph(string filepath)
        {
            string json = File.ReadAllText(filepath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            _id = jsonObj["id"];
            _name = jsonObj["displayName"];
            _region = jsonObj["region"];
            _filepath = filepath;

            _bad = _badMorphs.Contains(_name);
            _auto = _filepath.Contains("\\AUTO\\");

            ApplyColorScheme();
        }
        private void ApplyColorScheme()
        {
            // TODO: Include standard morphs marked as gray
            // if (_custom)
            // {
            //      // Mark custom morphs (not provided by VaM) as blue.
            DisplayColor = Brushes.LightBlue;
            // }

            if (_auto)
            {
                // Mark auto-imported (via VAC) morphs with as yellow.
                DisplayColor = Brushes.LightGoldenrodYellow;
            }

            if (_bad)
            {
                // Mark known bad morphs as red overriding all other swatches.
                DisplayColor = Brushes.Red;
            }
        }
        public string ID
        {
            get { return _id; }
        }
        public string Region
        {
            get { return _region; }
        }
        public string DisplayName
        {
            get { return _name; }
        }
        public string Filepath
        {
            get { return _filepath; }
        }
        public bool IsBad
        {
            get { return _bad; }
        }
        public bool IsAuto
        {
            get { return _auto; }
        }
        public Brush DisplayColor
        {
            get => _displayColor;
            set
            {
                _displayColor = value;
                OnPropertyChanged("DisplayColor");
            }
        }
        public Region Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString() => _id;
        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        internal void MoveToRegion(Region destination)
        {
            _parent.Morphs.Remove(this);
            destination.Morphs.Add(this);
            _parent = destination;
        }
        internal void Save()
        {
            if (!_region.Equals(_parent.Name))
            {
                // This morph was moved to a different region then what is stored 
                // in its metadata file (.vmi).
                // Load the complete dictionary from the file, change only the region, then save
                string json = File.ReadAllText(_filepath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["region"] = Parent.Name;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                Console.WriteLine("\n\n\n\nMorph " + _id + " was moved from region " + _region + " to region "+ _parent.Name);
                Console.WriteLine("Writing file: "+_filepath);

                File.WriteAllText(_filepath, output);
            }

        }
    }
}