# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very very early Alpha.

Thanks to the developers of **Retoc** and **UAssetAPI** which are included in this project.

**My progress so far:**
- Relevant enemy spawns have been identified. NPCs and other entities are untouched (I hope)
- The script loads the game files, replaces all relevant enemy spawns, creates the mod files and then moves them to the ~mods folder
- The behavior logic of replacement enemies is fully functional from my limited testing so far
- Enemies replace only those of the same "rank" (small, normal, boss)
- Bosses can be replaced in a way that does not add any new duplicates
- Enemy scaling is currently achieved by simply giving an enemy the stats of the one it is replacing. While vanilla does have some scaling of enemies across the game, different enemy types (and even different subvariants of the same enemy type) have their stats changed in wildly different ways so this is not a good reference

**Known issues and things to work on before this mod can be actively playtested:**
- Most bosses or boss arenas are broken after randomizing
  - Tachy and Democrawler teleport outside the arena
  - Tutorial boss is unkillable
  - The Gigas kill cutscene teleports the player out of bounds
  - Elder Phase 2 attacks do not work properly outside his arena (?)
  - Mann becomes invincible and unresponsive at a certain HP threshold when outside his arena
- Many regular enemy encounters in the game are probably also broken. One spawn in the first zone has already stumped me for several hours so I haven't tested much of the game at all
- Needs a GUI and usage documentation
- Cross-randomizing of enemies between different ranks (I could also change the size of enemies pretty easily as a fun addition to this)

## Installation
Not yet ready

## Usage
Not yet ready

## Building / Running
Needs .NET 8 SDK

dotnet run
