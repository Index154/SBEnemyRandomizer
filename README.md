# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very very early Alpha. I'm a mostly self-taught hobby programmer so please excuse the poor code quality.

Thanks to the developers of **Retoc** and **UAssetAPI** which are included in this project.

## My progress so far
- I've identified all the relevant enemy spawns to replace I think. NPCs and other entities should remain untouched
- The script loads the game files, replaces all relevant enemy spawns, creates the mod files and then moves them to the ~mods folder
- The behavior logic of replacement enemies is generally functional from my limited testing so far
- Enemies replace only those of the same "rank" (small, normal, boss)
- Bosses can be replaced in a way that does not add any new duplicates
- Randomization can be seeded to reproduce the same results
- Enemy scaling is currently achieved by simply giving an enemy the stats of the one it is replacing. While vanilla does have some scaling of enemies across the game, different enemy types (and even different subvariants of the same enemy type) have their stats changed in wildly different ways so this is not a good reference
- Known functional bosses:
  - WLA_M_HedgeBoarBrute_01

## Known issues and plans
- Most bosses or boss arenas are broken after randomizing. Known issues:
  - Tachy and Democrawler teleport outside the playable area when replacing other bosses
  - The tutorial Hedgeboar Brute is unkillable when replacing other bosses
  - The Gigas kill cutscene teleports the player out of bounds when replacing other bosses
  - Elder Phase 2 attacks do not work properly when replacing other bosses
  - Mann becomes invincible and unresponsive at a certain HP threshold when replacing other bosses
- Many regular enemy encounters in the game are probably also broken. I haven't tested much of the game at all yet. Known examples:
  - DED10_E_CharS_037 / WindowBreakHydra spawn sometimes fails for unknown reasons. Can completely softlock the save file. I've probably had to play through the intro sequence like 10 times already because of this
- Balancing issues:
  - Maelstrom is way too tanky without the gun to shoot his weak spots with
- Cross-randomizing of enemies between different ranks (I can also change the size of enemies pretty easily as a fun addition to this)
- The duo boss encounter in Eidos 9 is currently likely to be extremely unbalanced
- Needs a GUI and usage documentation
- Randomization results and generated seed should be saved to some kind of log

## Installation
Not yet ready

## Usage
Not yet ready

## Building / Running
Needs .NET 8 SDK

dotnet run
