# Morphology
A morph organizing tool for Virt-a-Mate

## Installation
- download the [lastest release](https://github.com/morph1sm/morphology/releases)
- unzip into a folder somewhere
- run `setup.exe` to install (will also create an Uninstall entry your Apps list)

## Usage
- open your morph folder, e.g. `/Vam/Custom/Atom/Person`
- select a region to display all morphs currently in that region
- drag one or more morphs to any region to move them

That's it.

Some quick fixes for common morph issues in VaM:

### Delete All DSF Files
They are only needed one time for VaM to import them. After that, they only slow down scene loading in general. If needed, you can always copy the DSF files from your DAZ library again.

### Move Auto Morphs to AUTO Region
The main issue with organizing your morph library are the random morphs installed by opening VAC files in VaM. Every user has different ideas about region names and organisation schemes, so you end up with lots of different region names containing just 2 or 3 morphs.

Fortunately, VaM copies morphs imported from scenes to a special subfolder called AUTO in your main morph folders. This quick fix, renames the region for any of these imported morphs to AUTO. All the funny region names go away and you can still find all these morphs in the AUTO region, should you want to move them into your regular library.

### Delete All Auto Morphs
If you are just exploring some random scenes, but are not interested in sorting out the various imported morphs, you can prune them from your library with this quick fix. If you want to restore a deleted auto morph, load the corresponding VAC file in VaM and they will be re-imported.


## Roadmap
### v0.3
- add and rename regions
- quick fix for invalid `CheekBone  Width Outer` morph for legacy scenes and VACs

### v0.4
- install DSF and pre-sort by region so the new morphs are imported into your region of choice
- importing some DUF to VAP

### v0.5
- add license info to VMI (VaM morph metadata)
- mark sharable vs copyrighted morphs
- remove copyrighted morph refrences from scenes

### v0.6
- integrate with DAZ product lists to find original metadata for installed morphs
- import and correct morph regions based on DAZ product lists
