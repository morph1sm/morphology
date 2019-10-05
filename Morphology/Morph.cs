// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Morphology
{
    public class Morph : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private Region _region;

        public Morph()
        {
        }

        public Morph(string id, string name, Region region)
        {
            _id = id;
            _name = name;
            _region = region;
        }

        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public Region Region
        {
            get { return _region; }
            set
            {
                _region = value;
                OnPropertyChanged("Region");
            }
        }

        public string DisplayName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString() => _id;

        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public void MoveToRegion(Region destination)
        {
            _region.morphs.Remove(this);
            destination.morphs.Add(this);
            _region = destination;
        }
    }
}