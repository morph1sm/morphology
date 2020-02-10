// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Morphology
{
public class Morph : INotifyPropertyChanged
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _region;
        private readonly string _filepath;
        private readonly bool _is_pose;
        private readonly bool _is_standard;
        private readonly List<string> _references;
        private readonly bool _known_as_bad;
        private readonly bool _was_auto_imported;

        public event PropertyChangedEventHandler PropertyChanged;
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
        public Morph(string filepath, Dictionary<string, List<string>> references)
        {
            string json = File.ReadAllText(filepath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            _id = jsonObj["id"];
            _name = jsonObj["displayName"];
            _region = jsonObj["region"];
            _is_pose = jsonObj["isPoseControl"] ?? true;
            _filepath = filepath;
            
            // distinction between standard and custom morphs not yet implemented
            // will add list of standard morphs later
            _is_standard = false;

            // bad morphs have side effects outside of their intended region
            _known_as_bad = _badMorphs.Contains(_name);

            // imported via VAC and stored in AUTO subfolder
            _was_auto_imported = _filepath.Contains("\\AUTO\\");

            _references = new List<string>();
            if (references.ContainsKey(_name))
            {
                _references = references[_name];
            }
            
            ApplyColorScheme();
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void ApplyColorScheme()
        {
            // TODO: Include standard morphs marked as gray
            // if (_custom)
            // {
            //      // Mark custom morphs (not provided by VaM) as blue.
            DisplayColor = Brushes.LightBlue;
            // }

            if (_was_auto_imported)
            {
                // Mark auto-imported (via VAC) morphs with as yellow.
                DisplayColor = Brushes.LightGoldenrodYellow;
            }

            if (_known_as_bad)
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
        public int ReferenceCount
        {
            get { return _references.Count; }
        }
        public string ReferenceInfo
        {
            get {
                if (_references.Count == 0)
                {
                    return "";
                }
                else
                {
                    return String.Format("References: {0}", _references.Count);
                }
            }
        }
        public List<string> ReferenceList
        {
            get { return _references; }
        }
        public bool IsAuto
        {
            get { return _was_auto_imported; }
        }
        public bool IsBad
        {
            get { return _known_as_bad; }
        }
        public bool IsPose
        {
            get { return _is_pose; }
        }
        public bool IsStandard
        {
            get { return _is_standard; }
        }
        public Brush DisplayColor
        {
            get => _displayColor;
            set
            {
                _displayColor = value;
                OnPropertyChanged();
            }
        }
        public Region Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged();
            }
        }
        public override string ToString() => _id;
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