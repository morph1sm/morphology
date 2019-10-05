using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Morphology
{
    public class Region : INotifyPropertyChanged
    {
        private string _name;
        // Signifies VaM's built-in region names
        private readonly bool _standard;

        public Morphs morphs = new Morphs();

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
        public string Info
        {
            get
            {
                long count = morphs.Count;

                if (count == 0) {
                    return "empty";
                }

                if (count == 1)
                {
                    return "1 morph";
                }

                return count + " morphs";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString() => _name;
        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}