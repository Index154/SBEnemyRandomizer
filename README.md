# Stellar Blade Enemy Randomizer
A enemy randomizer mod for Stellar Blade. Currently in very early alpha. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

Supported OS: Windows x64

Thanks to the developers of third-party software **Retoc** and **UAssetAPI** which are included in this project! (See `LICENSE.md`)

## Current progress
- I'm pretty sure I've identified all the relevant enemy spawns to modify
- The script loads the game files, modifies all relevant enemy spawns, creates the mod and then moves it to the game installation's ~mods folder
- The behavior logic of replaced enemies is largely functional from my limited testing so far
- Enemies replace only those of the same "rank" (small, normal, boss)
- Bosses are shuffled instead of being fully randomized (which would add duplicates)
- Randomization can be seeded to reproduce enemy placements
- Enemy scaling is currently achieved by changing some of each enemy's stats to those of the one it is replacing (HP, shield, attack, detection radius, reward pool). A better solution might be possible but would take a lot of manual work
- Known functional bosses:
  - SD_M_HedgeBoarBrute_01
  - WLA_M_HedgeBoarBrute_01
  - DED_M_GorillaB_01
  - WLA_M_GorillaBBrokenHead_01
  - UME_M_Sawshark_01
  - WLB_M_SawsharkWasteland_01
  - SE_M_WeaponMasterA_01
  - SE_M_WeaponMasterB_01
  - SE_M_Marionette_01
  - ATL_M_Maelstrom_01
  - AYL_M_Maelstrom_01
  - DEDA_M_GrubShooterElite_01
  - DED_M_GrubShooterElite_01
  - WLA_M_GrubShooterEliteB_01
  - WLA_M_RoyalGuardFemale_01
  - WLB_M_RoyalGuardFemale_01
  - WLB_M_Behemoth_01
  - DED_M_Opener_01
  - WLB_M_OpenerWasteland_01
  - NST_M_ElderPhase1_01
  - NST_M_Raven_01
  - NST_M_ExoSuit_01
  - UME_M_SkullJuggernaut_01
  - UME_M_Tachy_01
- Current testing progress: Early Wasteland

## Known issues with my notes to self
-Some bosses are broken after randomizing:
  - XION_M_RavenBeast_01 teleports the player out of bounds with the phase 3 transition
  - SE_M_Crawler_01 teleports outside the playable area
  - CHAL_XION_M_Mann_01 becomes invincible and unresponsive at a certain HP threshold
  - CHAL_M_Scarlet_01 becomes invincible and unresponsive at a certain HP threshold
  - Defeating the boss that replaces Tachy does not trigger the cutscene, leaving the player stuck? => Easy fix? Just give M_Tachy_Finish stance and immortality effect to the boss?
  - NST_M_ElderPhase2_01 attacks mostly do not work
- Many regular enemy encounters in the game might be broken. I haven't tested much of the game yet. Known examples:
  - The "GrubDash" after the Abaddon arena does not spawn => Needs retesting!
  - The secret stash where you interact with the corpse and then a glass wall shatters did not have an enemy. Is this normal? => Test in vanilla
- Balancing issues:
  - NST_M_ElderPhase1_01 is not really a boss fight so he's boring to encounter. Making custom changes to his AI could be fun. Or adding a simple setting to remove him from the boss pool
  - Maelstrom is way too tanky without the gun to shoot his weak spots with. At the same time he's also kinda weird to fight when you can go behind him since he can't rotate to face you
  - NST_M_ExoSuit_01 is impossible to beat without the gun
  - The Corrupter + tanky elite enemy duo encounter in Eidos 9 is likely to be extremely unbalanced when randomized
  - The gun-only areas are most likely very unbalanced
  - The first Belial encounter is likely unbalanced since the game doesn't expect you to reduce the boss' HP below ~70% (and I'm using the original boss' stats for the replacement)
- Minor visual issues:
  - UME_M_Tachy_01 character model does not despawn on death => Just add the default dissolve animation to the character entry
  - SE_M_WeaponMasterB_01 has no death animation and simply disappears instantly => Just add the default dissolve animation to the character entry

## Planned features and changes
- Cross-randomizing of enemies between different ranks (I can also change the size of enemies pretty easily as a fun addition to this)
- A GUI and usage documentation so normal players can actually download and use the mod
- Randomization results and seed should be saved to some kind of log
- Nikke enemy variants are not included in randomization yet. Not sure if I want to add them
- Scarlet and Mann boss inclusion settings are unimplemented (currently included by default)
- A DLC check should be implemented to prevent Scarlet from appearing when it isn't owned (untested / unconfirmed)

## Installation
WIP

## Usage
WIP

## For developers
Requirements: .NET 8 SDK

Debugging: `dotnet run`

Publishing: `dotnet publish -c Release`