using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Morphology.Data;

namespace Morphology
{
    public class Regions : ObservableCollection<Region>
    {
        private readonly string _morph_folder;
        private readonly Settings _settings;
        public Regions(string _folder, Settings settings)
        {
            _morph_folder = _folder;
            _settings = settings;
            ApplyVaMStandardRegions();

            ScanFolder(_morph_folder);
        }
        public string Folder
        {
            get { return _morph_folder; }
        }
        public int TotalCustomRegions
        {
            get { return this.Where(region => !region.IsStandard).Count(); }
        }
        public int TotalCustomMorphs
        {
            get { return this.Sum(region => region.Morphs.Count); }
        }
        private void ApplyVaMStandardRegions() {    
            Add(new Region("Morph/Anus", true));
            Add(new Region("Morph/Arms", true));
            Add(new Region("Morph/Back", true));
            Add(new Region("Morph/Body", true));
            Add(new Region("Morph/Chest/Areola", true));
            Add(new Region("Morph/Chest/Breasts", true));
            Add(new Region("Morph/Chest/Nipples", true));
            Add(new Region("Morph/Feet", true));
            Add(new Region("Morph/Genitalia", true));
            Add(new Region("Morph/Hands", true));
            Add(new Region("Morph/Head", true));
            Add(new Region("Morph/Head/Brow", true));
            Add(new Region("Morph/Head/Cheeks", true));
            Add(new Region("Morph/Head/Chin", true));
            Add(new Region("Morph/Head/Ears", true));
            Add(new Region("Morph/Head/Eyes", true));
            Add(new Region("Morph/Head/Face", true));
            Add(new Region("Morph/Head/Jaw", true));
            Add(new Region("Morph/Head/Mouth", true));
            Add(new Region("Morph/Head/Mouth/Teeth", true));
            Add(new Region("Morph/Head/Mouth/Tongue", true));
            Add(new Region("Morph/Head/Nose", true));
            Add(new Region("Morph/Head/Shape", true));
            Add(new Region("Morph/Hip", true));
            Add(new Region("Morph/Legs", true));
            Add(new Region("Morph/Neck", true));
            Add(new Region("Morph/UpperBody", true));
            Add(new Region("Morph/Waist", true));
            Add(new Region("Pose/Arms", true));
            Add(new Region("Pose/Chest", true));
            Add(new Region("Pose/Feet/Left/Toes", true));
            Add(new Region("Pose/Feet/Right/Toes", true));
            Add(new Region("Pose/Hands/Left", true));
            Add(new Region("Pose/Hands/Left/Fingers", true));
            Add(new Region("Pose/Hands/Right", true));
            Add(new Region("Pose/Hands/Right/Fingers", true));
            Add(new Region("Pose/Head/Brow", true));
            Add(new Region("Pose/Head/Cheeks", true));
            Add(new Region("Pose/Head/Expressions", true));
            Add(new Region("Pose/Head/Eyes", true));
            Add(new Region("Pose/Head/Jaw", true));
            Add(new Region("Pose/Head/Mouth", true));
            Add(new Region("Pose/Head/Mouth/Lips", true));
            Add(new Region("Pose/Head/Mouth/Tongue", true));
            Add(new Region("Pose/Head/Nose", true));
            Add(new Region("Pose/Head/Visemes", true));
        }
            
        internal void ScanFolder(string dir)
        {
            try
            {
                foreach (string sub in Directory.GetDirectories(dir))
                {
                    foreach (string filepath in Directory.GetFiles(sub, "*.vmi"))
                    {
                        try {
                            Morph morph = new Morph(filepath);

                            if (MatchesFilter(morph)) { 
                                AddCustomMorph(morph);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not load morph metadata from:\n\n" + filepath + "\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    ScanFolder(sub);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not scan the selected folder.\n\n" + ex.Message, "Morphology", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool MatchesFilter(Morph morph)
        {
            return
                _settings.ShowAutoMorphs && morph.IsAuto ||
                _settings.ShowBadMorphs && morph.IsBad ||
                _settings.ShowCustomMorphs && !morph.IsStandard ||
                _settings.ShowPoseMorphs && morph.IsPose ||
                _settings.ShowShapeMorphs && !morph.IsPose ||
                _settings.ShowStandardMorphs && morph.IsStandard;
        }
        private void AddCustomMorph(Morph morph)
        {
            bool morphAddedToExistingRegion = false;
            foreach (Region region in this)
            {
                if (region.Name == morph.Region)
                {
                    morph.Parent = region;
                    region.Morphs.Add(morph);
                    morphAddedToExistingRegion = true;
                }
            }

            if (!morphAddedToExistingRegion)
            {
                // this morph has a custom region that's not yet listed
                // create anew region before adding this morph
                Region region = new Region(morph.Region);
                morph.Parent = region;
                region.Morphs.Add(morph);
                Add(region);
            }
        }
        internal List<Morph> GetAutoMorphs()
        {
            List<Morph> autoMorphs = new List<Morph>();

            foreach (Region region in this)
            {
                autoMorphs.AddRange(region.Morphs.Where(m => m.IsAuto));
            }
            return autoMorphs;
        }
    }
}