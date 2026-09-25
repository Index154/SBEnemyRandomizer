# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very early alpha. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

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
  - DED_M_GorillaB_01 (Gigas)
  - WLA_M_GorillaBBrokenHead_01
  - UME_M_Sawshark_01 (Stalker)
  - WLB_M_SawsharkWasteland_01
  - SE_M_WeaponMasterA_01 (Belial)
  - SE_M_WeaponMasterB_01
  - SE_M_Marionette_01 (Karakuri)
  - ATL_M_Maelstrom_01
  - AYL_M_Maelstrom_01
  - DEDA_M_GrubShooterElite_01 (Corrupter)
  - DED_M_GrubShooterElite_01
  - WLA_M_GrubShooterEliteB_01
  - WLA_M_RoyalGuardFemale_01
  - WLB_M_RoyalGuardFemale_01
  - WLB_M_Behemoth_01
  - DED_M_Opener_01 (Abaddon)
  - WLB_M_OpenerWasteland_01
  - NST_M_ElderPhase1_01
  - NST_M_Raven_01
  - XION_M_RavenBeast_01
  - NST_M_ExoSuit_01 (Providence)
  - UME_M_SkullJuggernaut_01
  - UME_M_Tachy_01
  - SE_M_Crawler_01 (Democrawler)
  - CHAL_XION_M_Mann_01
  - CHAL_M_Scarlet_01
- Areas and game progression confirmed to be functional: Up until Altess Levoire (haven't tested beyond that)

## Known issues
- NST_M_ElderPhase2_01 attacks do not function correctly
- Balancing issues:
  - Maelstrom is way too tanky without the gun to shoot his weak spots with. At the same time he's also kinda weird to fight when you can go behind him since he can't rotate to face you
  - NST_M_ExoSuit_01 and XION_M_RavenBeast_01 are impossible to beat without the gun
  - The Corrupter + tanky enemy duo encounter in Eidos 9 is likely to be extremely unbalanced => Needs a setting to prevent tough bosses from being placed there. Or an option to place 2 random normal enemies instead
  - The gun-only areas are most likely very unbalanced
  - The first Belial encounter is likely unbalanced since the game doesn't expect you to reduce the boss' HP below ~70% (and I'm using the original boss' stats for the replacement)
- Minor issues:
  - XION_M_RavenBeast_01 only casts its super move once per battle instead of twice
  - SE_M_Crawler_01, CHAL_XION_M_Mann_01, UME_M_Tachy_01 and SE_M_WeaponMasterB_01 have no death animation
  - The secret stash where you interact with the corpse and then a glass wall shatters did not have an enemy. Is this normal? => Test in vanilla
  - Mann holds two weapons at once after the phase 2 transition. A visual bug that disappears after his first axe attack
  - NST_M_ElderPhase1_01 is boring. Making custom changes to his AI could be fun

## Planned features and changes
- Cross-randomizing of enemies between different ranks (I can also change the size of enemies pretty easily as a fun addition to this)
- Extra enemy spawns in NG+ are not yet being randomized (I didn't notice that it was a separate file at first. Should be easy to include though)
- A GUI and usage documentation so normal players can actually download and use the mod
- Randomizing of enemies summoned by other enemies (EffectTable entries with EffectAction_SummonActor)
- Scarlet and Mann boss inclusion settings are unimplemented (currently included by default)
- A DLC check should be implemented to prevent Scarlet from appearing when it isn't owned (untested / unconfirmed)
- More sophisticated enemy scaling to preserve an appropriate level of tankiness that matches the the enemy type => Idea: Document the level of tankiness and damage output (difficulty) of each enemy plus the average stats of an enemy of each difficulty rating per zone and then use those values for the randomly placed enemies
- An option to prevent bosses that require the gun from appearing before the gun is unlocked
- A checklist of every enemy type so the user can prevent specific ones from appearing at all
- Some way for the user to make certain enemies more likely to appear than others => Weighted selection
- Things that go with the "shuffle bosses" setting:
  - A list of all bosses that lets the user determine how many of them are in the shuffle pool
  - "Don't place duplicate bosses in main story locations"
  - "Fill empty boss slots with random duplicates" (useful if the player )
- A setting for boss music. Options:
  - Music is based on the location
  - Music matches the randomly placed boss
  - Random music
    - Note to self: SoundEventTable has the music triggers. Look for ForceEventBattle and specific EventBattleState values! These settings determine what part of the zone's BGM to actually play (which includes battle music and boss themes)

## Installation
WIP

## Usage
WIP

## For developers
Requirements: .NET 8 SDK

Debugging: `dotnet run`

Publishing: `dotnet publish -c Release`
