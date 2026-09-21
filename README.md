<div align="center">

# Azimuth Stonecutter Bronze

**Bring the Stonecutter to the Bronze Age without breaking Valheim's Iron progression.**

![Azimuth Stonecutter Bronze](https://raw.githubusercontent.com/jrhunt223/AzimuthStonecutterBronze/master/images/Valheim%20Stonecutter%20Bronze%201.0.1.png)

**Author:** Azimuth  
**Version:** 1.0.1  
**Valheim:** 1.0  
**Multiplayer:** Client & Server — required on the dedicated server and every connecting client

</div>

## What It Does

Azimuth Stonecutter Bronze changes the vanilla Stonecutter construction cost from:

**10 Wood + 4 Stone + 2 Iron**

to:

**10 Wood + 4 Stone + 2 Bronze**

This makes the Stonecutter available during Bronze Age progression while preserving the intended Iron progression that normally follows.

## Progression Protection

Simply replacing the Stonecutter's Iron requirement with Bronze causes Valheim to make the **Sharpening Stone** available too early. This mod prevents that progression skip.

- The Stonecutter becomes available after discovering the required Bronze Age materials.
- The Sharpening Stone remains locked until that character discovers **Iron** for the first time.
- Killing **The Elder is not used as the unlock condition**.
- After Iron is discovered, normal Sharpening Stone progression resumes.
- Grinding Wheel progression therefore remains gated behind Iron.
- Characters affected by the v1.0.0 early-unlock issue are corrected when they have not yet discovered Iron.

## Requirements

- BepInExPack for Valheim
- Jötunn 2.30.1 or newer

## Multiplayer

This mod is designed for consistent multiplayer progression.

**Install it on the dedicated server and on every connecting client.**

Azimuth Stonecutter Bronze uses Jötunn network compatibility with `EveryoneMustHaveMod`, making the mod a join requirement for servers running it.

## Installation

Install the mod through your mod manager, or place `AzimuthStonecutterBronze.dll` in:

`BepInEx/plugins/AzimuthStonecutterBronze/`

Jötunn and BepInEx must also be installed.

## Version 1.0.1

- Fixed Sharpening Stone unlocking when Bronze unlocks the Stonecutter.
- Sharpening Stone now remains locked until the character discovers Iron.
- Prevented repeated "New crafting recipe: Sharpening Stone" notifications.
- Added cleanup for characters that incorrectly learned the recipe while using v1.0.0.
- Stonecutter continues to require 10 Wood, 4 Stone, and 2 Bronze.

## Building

The repository includes a GitHub Actions workflow that builds against the current Valheim Dedicated Server assemblies and the required BepInEx/Jötunn dependencies.

Pull requests are automatically build-tested. Pushes to `master` build an artifact, and version tags beginning with `v` create a GitHub Release.

## License

MIT License.

---

<div align="center">

## Azimuth Gaming

**Gaming for Military Vets**

</div>
