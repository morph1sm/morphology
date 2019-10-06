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
    public class Settings : INotifyPropertyChanged
    {
        public string CurrentlySelectedFolder
        {
            get => _currentlySelectedFolder;
            set
            {
                var oldvalue = _currentlySelectedFolder;
                _currentlySelectedFolder = value;
                if (value != oldvalue)
                {
                    IsDirty = true;
                }
                OnPropertyChanged();
            }
        }

        public bool AutoApplyChanges
        {
            get => _autoApplyChanges;
            set
            {
                var oldvalue = _autoApplyChanges;
                _autoApplyChanges = value;
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

        private string _currentlySelectedFolder;
        private bool _autoApplyChanges;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
