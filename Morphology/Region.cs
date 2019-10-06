using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;

namespace Morphology
{
    public class Region : INotifyPropertyChanged
    {
        private string _name;
        // Flags VaM's built-in category (region) names
        private readonly bool _standard;
        private readonly Morphs _morphs = new Morphs();

        public Region(string name)
        {
            _name = name;
            _standard = false;
        }
        public Region(string name, bool standard)
        {
            _name = name;
            _standard = standard;
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
                    return "empty";
                }

                if (count == 1)
                {
                    return "1 morph";
                }

                return count + " morphs";
            }
        }
        internal void TransferMorphs(IList selectedItems)
        {
            List<Morph> dragged = selectedItems.Cast<Morph>().ToList();
            foreach (Morph morph in dragged)
            {
                morph.MoveToRegion(this);
            }
        }
    }
}