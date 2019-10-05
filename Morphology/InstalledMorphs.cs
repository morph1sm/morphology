// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace Morphology
{
    public class InstalledMorphs : ObservableCollection<Morph>
    {
        public InstalledMorphs()
        {
            Add(new Morph("PHMBGMChin01", "Big Girl Morphs", "Chin 01" ));
            Add(new Morph("PHMBGMChin02", "Big Girl Morphs", "Chin 02"));
            Add(new Morph("AFChin01", "Asian Faces", "Chin 01"));
        }
    }
}