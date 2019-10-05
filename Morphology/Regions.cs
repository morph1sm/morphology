using System.Collections.ObjectModel;

namespace Morphology
{
    public class Regions : ObservableCollection<Region>
    {
        public Regions()
        {
            // VaM Standard Regions
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


            // TODO: Remove these test custom regions
            Add(new Region("__Alter3go__"));

            Region bbw = new Region("Big Girl Morphs");
            bbw.morphs.Add(new Morph("PHMBGMChin01", "Chin 01", bbw));
            bbw.morphs.Add(new Morph("PHMBGMChin02", "Chin 02", bbw));
            Add(bbw);

            Region af = new Region("Asian Faces");
            af.morphs.Add(new Morph("AFNose01", "Nose 01", af));
            af.morphs.Add(new Morph("AFNose02", "Nose 02", af));
            Add(af);
        }
    }
}