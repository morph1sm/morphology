using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphology.Data
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    

    public class SettingHandler<T> where T : new()
    {
        private T _loadedSettings;
        public static SettingHandler<T> CurrentInstance;

        public T LoadedSettings
        {
            get => _loadedSettings;
            set => _loadedSettings = value;
        }

        private readonly FileInfo _path;
        public readonly List<Exception> Errors;
        public SettingHandler(FileInfo path)
        {
            CurrentInstance = this;
            Errors = new List<Exception>();
            try
            {
                _path = path;
                
                if (!File.Exists(path.FullName) || path.Length == 0)
                {
                    new T().SerializeToFile(path.FullName);
                }

                _loadedSettings = new T().DeserializeFromFile(path.FullName);
                _loadedSettings.SerializeToFile(path.FullName);
            }
            catch (Exception e)
            {
                Errors.Add(e);
            }
        }

        public bool Save()
        {
            try
            {
                _loadedSettings.SerializeToFile(_path.FullName);
                return true;
            }
            catch (Exception e)
            {
                Errors.Add(e);
                return false;
            }
        }

        public bool Reload()
        {
            try
            {
                _loadedSettings = new T().DeserializeFromFile(_path.FullName);
                if (_loadedSettings != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Errors.Add(e);
                return false;
            }
        }
    }
}
