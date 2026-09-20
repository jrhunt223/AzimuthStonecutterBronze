# Azimuth Stonecutter Bronze

A lightweight Valheim progression mod by **Azimuth Gaming**.

## What it does

Azimuth Stonecutter Bronze changes the vanilla Stonecutter construction cost from:

- 10 Wood
- 4 Stone
- 2 Iron

to:

- 10 Wood
- 4 Stone
- 2 Bronze

This makes the Stonecutter available during the Bronze Age while preserving the normal Iron progression gate for the Sharpening Stone and Grinding Wheel progression.

## Requirements

- BepInExPack for Valheim
- Jotunn
- All players in a multiplayer session must have the mod installed.

## Installation

Install the mod through your mod manager, or place `AzimuthStonecutterBronze.dll` in:

`BepInEx/plugins/AzimuthStonecutterBronze/`

Jotunn and BepInEx must also be installed.

## Multiplayer

The mod uses Jotunn network compatibility with `EveryoneMustHaveMod`, so clients and the server should all run the mod.

## Building

The repository includes a GitHub Actions workflow that builds against the current Valheim Dedicated Server assemblies and the required BepInEx/Jotunn dependencies.

Pull requests are automatically build-tested. Pushes to `master` build an artifact, and version tags beginning with `v` create a GitHub Release.

## License

MIT License.
