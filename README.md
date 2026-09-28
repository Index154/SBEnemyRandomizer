# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very early alpha. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

Supported OS: Windows x64

Thanks to the developers of third-party software **Retoc** and **UAssetAPI** which are included in this project! (See `LICENSE.md`)

## Current progress
- The script loads the game files, modifies all relevant enemy spawns, creates the mod and then moves it to the game installation's ~mods folder. The only thing the user has to do is provide the game path, make some configurations and then run the script
- The behavior logic of replaced enemies is largely functional from my limited testing so far
- Enemies replace only those of the same "rank" by default (small, normal, boss)
- Bosses are shuffled instead of being fully randomized (which would add duplicates)
- Randomization can be seeded to reproduce enemy placements
- All spawns can be set to be replaced by the same enemy
- Enemy scaling is currently achieved by changing some of each enemy's stats to those of the one it is replacing (HP, shield, attack, detection radius, reward pool). A better solution might be possible but would take a lot of manual work

## Known issues
- Major issues:
  - The game has not been tested beyond the second Royal Guard fight. Some softlocks probably still remain
  - NST_M_ElderPhase2_01 and XION_M_RavenBeast_01 certain-kill moves can spawn their destroyable projectiles out of bounds. In most arenas the chance of this happening is low. But it is likely to happen in some spawn locations: ME_03_E_CharS_018 (Stalker), ME_05_E_CharS_017 (Juggernaut) => Add a setting that prevents them from being placed there that is on by default
  - Double-check if Hedgeboar Brute phase 2 is triggering
  - Double-check if Abaddon phase 2 is triggering (for both variants)
  - Go through all bosses and check for important condition triggers and event actor effects dependent on the spawn event tag
- Balancing issues:
  - Karakuri minions are not scaled in difficulty
  - The Corrupter + tanky enemy duo encounter in Eidos 9 is likely to be extremely unbalanced => Needs a setting to prevent tough bosses from being placed there. Or an option to place 2 random normal enemies instead
  - The first Belial encounter is likely unbalanced since the game doesn't expect you to reduce the boss' HP below ~70% (and I'm using the original boss' stats for the replacement)
- Minor issues:
  - SE_M_Crawler_01, CHAL_XION_M_Mann_01, UME_M_Tachy_01 and SE_M_WeaponMasterB_01 have no death animation
  - The secret stash where you interact with the corpse and then a glass wall shatters did not have an enemy. Is this normal? => Test in vanilla
  - Mann holds two weapons at once after the phase 2 transition. A visual bug that disappears after his first axe attack
  - NST_M_ElderPhase1_01 is boring. Making custom changes to his AI could be fun
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. => For now I am leaving the Maelstrom encounters as they are (aside from randomizing what minions they summon)
  - WLA_M_RoyalGuardFemale_01 encounter (WLA_30_E_CharS_025) no longer has battle music
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)

## Planned features and changes (from most to least important)
- High priority:
  - Fixing of the major issues listed above
  - Randomization of the extra enemy spawns added in NG+
  - An option to prevent bosses that require the gun from appearing before the gun is unlocked (NST_M_ExoSuit_01, NST_M_ElderPhase2_01, XION_M_RavenBeast_01, Maelstrom)
  - Scarlet and Mann boss inclusion/exclusion settings (currently included by default)
  - Verification that Scarlet can only spawn if the player owns the DLC
  - A GUI and usage documentation, followed by a downloadable and playable release build!
- Low priority:
  - More sophisticated enemy scaling to preserve an appropriate level of tankiness that matches the enemy type => Idea: Document the level of tankiness and damage output (difficulty) of each enemy plus the average stats of an enemy of each difficulty rating per zone and then use those values for the randomly placed enemies
  - A checklist of every enemy type so the user can prevent specific ones from appearing at all
  - Some way for the user to make certain enemies more likely to appear than others => Weighted selection
  - Things that go with the "shuffle bosses" setting:
    - "Place a duplicate boss in the tutorial encounter"
    - "Don't place duplicate bosses in main story locations aside from the tutorial"
    - A list of all bosses that lets the user determine how many of them are in the shuffle pool. If the user reduces the number below the minimum then show them a warning and tell them that empty locations will be filled with random extra duplicates
  - Randomization of enemies summoned by Hives and upon death of bot enemies
- Rough ideas:
  - Cross-randomizing of enemies between different ranks (also change their size maybe)
  - Automatic save file backups? Maybe?
  - A setting for boss music. Options:
    - Music is based on the location
    - Music matches the randomly placed boss
    - Random music
      - SoundEventTable has the music triggers. Look for ForceEventBattle and specific EventBattleState values! These settings determine what part of the zone's BGM to actually play (which includes battle music and boss themes)
      - The names of the sound events seem to correspond with zone event names?

## Installation
WIP

## Usage
WIP

## Compatibility
This mod is incompatible with any mod that makes changes to the following game files:
- ... (TBA)

## For developers
Requirements: .NET 8 SDK

Debugging: `dotnet run`

Publishing: `dotnet publish -c Release`