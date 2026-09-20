# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very very early Alpha. I'm a mostly self-taught hobby programmer so please excuse the poor code quality.

Thanks to the developers of **Retoc** and **UAssetAPI** which are included in this project.

## Current progress
- I'm pretty sure I've identified all the relevant enemy spawns to modify
- The script loads the game files, modifies all relevant enemy spawns, creates the mod and then moves it to the game installation's ~mods folder
- The behavior logic of replaced enemies is largely functional from my limited testing so far
- Enemies replace only those of the same "rank" (small, normal, boss)
- Bosses are shuffled instead of being fully randomized (which would add duplicates)
- Randomization can be seeded to reproduce enemy placements
- Enemy scaling is currently achieved by changing each enemy's stats to those of the one it is replacing. A better solution might be possible but would take a lot of manual work
- Known functional bosses:
  - WLA_M_HedgeBoarBrute_01

## Known issues and plans
- Most bosses or boss cutscenes are likely broken after randomizing. Known issues:
  - SE_M_Crawler_01 teleports outside the playable area
  - UME_M_Tachy_01 teleports outside the playable area
  - SD_M_HedgeBoarBrute_01 is unkillable
  - DED_M_GorillaB_01 kill cutscene teleports the player out of bounds
  - NST_M_ElderPhase2_01 attacks do not work properly
  - CHAL_XION_M_Mann_01 becomes invincible and unresponsive at a certain HP threshold
  - Maelstrom is way too tanky without the gun to shoot his weak spots with
- Many regular enemy encounters in the game are probably also broken. I haven't tested much of the game at all yet. Known examples:
  - DED10_E_CharS_037 / WindowBreakHydra spawn sometimes fails for unknown reasons. Can completely softlock the save file. I've probably had to play through the intro sequence like 10 times already because of this
- Cross-randomizing of enemies between different ranks (I can also change the size of enemies pretty easily as a fun addition to this)
- The duo boss encounter in Eidos 9 is currently likely to be extremely unbalanced
- Needs a GUI and usage documentation
- Randomization results and generated seed should be saved to some kind of log

## Untested bosses
- ATL_M_Maelstrom_01
- AYL_M_Maelstrom_01
- CHAL_M_Scarlet_01
- DEDA_M_GrubShooterElite_01
- DED_M_GrubShooterElite_01
- DED_M_Opener_01
- NST_M_ElderPhase1_01
- NST_M_ExoSuit_01
- NST_M_Raven_01
- SE_M_Marionette_01
- SE_M_WeaponMasterA_01
- SE_M_WeaponMasterB_01
- UME_M_Sawshark_01
- UME_M_SkullJuggernaut_01
- WLA_M_GorillaBBrokenHead_01
- WLA_M_GrubShooterEliteB_01
- WLA_M_RoyalGuardFemale_01
- WLB_M_Behemoth_01
- WLB_M_OpenerWasteland_01
- WLB_M_RoyalGuardFemale_01
- WLB_M_SawsharkWasteland_01
- XION_M_RavenBeast_01

## Installation
Not yet ready

## Usage
Not yet ready

## Building / Running
Needs .NET 8 SDK

dotnet run