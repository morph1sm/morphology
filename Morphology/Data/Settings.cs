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
        public bool ShowUnusedMorphs
        {
            get => _showUnusedMorphs;
            set
            {
                var oldvalue = _showUnusedMorphs;
                _showUnusedMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowSingleUseMorphs
        {
            get => _showSingleUseMorphs;
            set
            {
                var oldvalue = _showSingleUseMorphs;
                _showSingleUseMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowShapeMorphs
        {
            get => _showShapeMorphs;
            set
            {
                var oldvalue = _showShapeMorphs;
                _showShapeMorphs = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        public bool ShowPoseMorphs
        {
            get => _showPoseMorphs;
            set
            {
                var oldvalue = _showPoseMorphs;
                _showPoseMorphs = value;
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
        private bool _showUnusedMorphs = true;
        private bool _showSingleUseMorphs = true;
        private bool _showShapeMorphs = true;
        private bool _showPoseMorphs = true;
        private bool _showAutoMorphs = true;
        private bool _showBadMorphs = true;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
