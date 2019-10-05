// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Morphology
{
    public class Morph : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _region;

        public Morph()
        {
        }

        public Morph(string id, string region, string name)
        {
            _id = id;
            _region = region;
            _name = name;
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

        public string Region
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
    }
}