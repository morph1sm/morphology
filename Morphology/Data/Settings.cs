using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Morphology.Data
{
    /// <summary>
    /// Setting Class that gets Serialized to the XML-Setting File. Add any new Settings as a Property here
    /// </summary>
    public class Settings : INotifyPropertyChanged
    {
        public string Folder
        {
            get => _folder;
            set
            {
                var oldvalue = _folder;
                _folder = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowStandardMorphs
        {
            get => _showStandardMorphs;
            set
            {
                var oldvalue = _showStandardMorphs;
                _showStandardMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowCustomMorphs
        {
            get => _showCustomMorphs;
            set
            {
                var oldvalue = _showCustomMorphs;
                _showCustomMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowAutoMorphs
        {
            get => _showAutoMorphs;
            set
            {
                var oldvalue = _showAutoMorphs;
                _showAutoMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowBadMorphs
        {
            get => _showBadMorphs;
            set
            {
                var oldvalue = _showBadMorphs;
                _showBadMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _isDirty;
        [XmlIgnore]
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                if (_isDirty)
                {
                    SettingHandler<Settings>.CurrentInstance.Save();
                }
                OnPropertyChanged();
            }
        }
        private string _folder;
        private bool _showStandardMorphs = true;
        private bool _showCustomMorphs = true;
        private bool _showAutoMorphs = true;
        private bool _showBadMorphs = true;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
