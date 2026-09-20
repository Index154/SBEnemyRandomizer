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
- Enemy scaling is currently achieved by changing some of each enemy's stats to those of the one it is replacing (HP, shield, attack, detection radius, reward pool). A better solution might be possible but would take a lot of manual work
- Known functional bosses:
  - SD_M_HedgeBoarBrute_01 (after disabling immortality)
  - WLA_M_HedgeBoarBrute_01
  - UME_M_Sawshark_01
  - SE_M_Marionette_01 (after disabling immortality)
- Current testing progress: Reached Xion

## Known issues
- Most bosses or boss cutscenes are likely broken after randomizing. Known gamebreaking issues:
  - SE_M_Crawler_01 teleports outside the playable area
  - UME_M_Tachy_01 teleports outside the playable area
  - DED_M_GorillaB_01 kill cutscene teleports the player out of bounds, leading to death
  - NST_M_ElderPhase2_01 attacks do not work properly
  - CHAL_XION_M_Mann_01 becomes invincible and unresponsive at a certain HP threshold
  - Maelstrom is way too tanky without the gun to shoot his weak spots with
- Many regular enemy encounters in the game are probably also broken. I haven't tested much of the game at all yet. Known examples:
  - DED10_E_CharS_037 / WindowBreakHydra spawn sometimes fails for unknown reasons. Can completely softlock the save file. I've probably had to play through the intro sequence like 10 times already because of this
  - The "GrubDash" after the Abaddon arena does not spawn
  - The secret stash where you interact with the corpse and then a glass wall shatters did not have an enemy. Is this normal?
- The duo boss encounter in Eidos 9 is currently likely to be extremely unbalanced
- The gun-only areas are most likely very unbalanced as of now

## Planned features and changes
- Cross-randomizing of enemies between different ranks (I can also change the size of enemies pretty easily as a fun addition to this)
- A GUI and usage documentation so normal players can actually download and use the mod
- Randomization results and seed should be saved to some kind of log
- Nikke enemy variants are not included in randomization yet. Not sure if I want to add them
- Scarlet and Mann boss inclusion settings are unimplemented (currently included by default)
- A DLC check should be implemented to prevent Scarlet from appearing when it isn't owned

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