// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.IO;

namespace Morphology
{
    public class Morph : INotifyPropertyChanged
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _region;
        private readonly string _filepath;

        private Region _parent;

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