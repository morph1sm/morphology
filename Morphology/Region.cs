using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Windows.Media;

namespace Morphology
{
    public class Region : INotifyPropertyChanged
    {
        private string _name;
        // Flags VaM's built-in category (region) names
        private readonly bool _standard;
        private readonly Morphs _morphs = new Morphs();

        //Default Color for Standard Region
        private Brush _displayColor = Brushes.LightGray;

        public Region(string name)
        {
            _name = name;
            _standard = false;
            ApplyColorScheme();   
        }
        //If the Current Morph is not a VAM-Standard-Region, the Color-Scheme applies.
        private void ApplyColorScheme()
        {
            if (!_standard)
            {
                _displayColor = Brushes.LightBlue;
            }
        }
        public Region(string name, Brush displaycolor)
        {
            _name = name;
            _standard = false;
            _displayColor = displaycolor;
        }

        public Region(string name, bool standard)
        {
            _name = name;
            _standard = standard;
            ApplyColorScheme();
        }

        public Region(string name, Brush displaycolor, bool standard)
        {
            _name = name;
            _standard = standard;
            _displayColor = displaycolor;
            ApplyColorScheme();
        }

        public bool IsStandard
        {
            get { return _standard; }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_standard)
                {

                }

                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public Morphs Morphs
        {
            get { return _morphs; }
        }
        public void AddMorph(Morph morph)
        {
            _morphs.Add(morph);
            OnPropertyChanged("Info");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString() => _name;
        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public string Info
        {
            get
            {
                long count = _morphs.Count;

                if (count == 0)
                {
                    return _name;
                }

                return _name + " (" + count + ")";

            }
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
        internal void TransferMorphs(List<Morph> selectedItems)
        {
            foreach (Morph morph in selectedItems)
            {
                morph.MoveToRegion(this);
                morph.Save();
            }
        }
    }
}