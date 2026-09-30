# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very early alpha. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

Supported OS: Windows x64

Thanks to the developers of third-party software **Retoc** and **UAssetAPI** which are included in this project! (See `LICENSE.md`)

## Current progress
- The script loads the game files, modifies all relevant enemy spawns, creates the mod and then moves it to the game installation's ~mods folder. There is no GUI and no release yet
- The behavior logic of replaced enemies seems to be fully functional from my limited testing so far (after fixing some bosses)
- Enemies replace only those of the same "rank" by default (small, normal or boss). Alternatively all spawns can manually be set to be replaced by the same enemy
- Bosses are shuffled instead of being fully randomized (to not add extra duplicates)
- Randomization can be seeded to reproduce the enemy placements
- Enemy scaling is currently achieved by changing some of each enemy's stats to those of the one it is replacing (HP, shield, attack, detection radius, reward pool). A better solution might be possible but would take a lot of manual work

## Known issues (highest priority at the top)
- Major issues:
  - The game has not been tested beyond Maelstrom 2. Some softlocks and broken quests probably still remain
  - NST_M_ElderPhase2_01 and XION_M_RavenBeast_01 certain-kill moves can spawn their destroyable projectiles out of bounds. In most arenas the chance of this happening is low. But it is likely to happen in some locations: ME_03_E_CharS_018 (Stalker arena), ME_05_E_CharS_017 (Juggernaut arena) => Need a setting that prevents them from being placed there (on by default)
  - Bosses that require the gun can appear before the gun is unlocked (NST_M_ExoSuit_01, NST_M_ElderPhase2_01, XION_M_RavenBeast_01, Maelstrom) => Need a countermeasure setting for this too
- Balancing issues:
  - Karakuri minions are not scaled in difficulty
  - The Corrupter + tanky enemy duo encounter in Eidos 9 is likely to be extremely unbalanced => Needs a setting to prevent tough bosses from being placed there. Or an option to place 2 random normal enemies instead?
  - The first Belial encounter is likely unbalanced since the game doesn't expect you to reduce the boss' HP below ~70% (and I'm using the original boss' stats for the replacement)
  - Which properties of an enemy should be replaced and which should be retained needs another thorough check
  - The replacement for the Skull Juggernaut is scaled up in size. This can make the camera behave a bit annoyingly for some bosses
- Minor issues:
  - The secret room in Eidos 7 where you interact with the corpse and then a glass wall shatters did not have an enemy when I tested it => Needs another look
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. Placing Maelstrom in a different arena where you can walk behind it can also make the fight feel rather unbalanced => For now I am leaving the Maelstrom encounters untouched (aside from randomizing what minions they summon)
  - WLA_M_RoyalGuardFemale_01 encounter (WLA_30_E_CharS_025) no longer has battle music after being replaced
  - Other replaced bosses might also not have music or at least no phase 2 music, probably. I didn't pay much attention to this yet
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)
  - All bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless => Somehow disable this when replacing regular enemies with bosses?

## Planned features and changes (from most to least important)
- High priority:
  - Fixing of the major issues listed above
  - Double-check if Hedgeboar Brute phase 2 is triggering
  - Double-check if Abaddon phase 2 is triggering
  - Go through all bosses again and check for important condition triggers and event actor effects dependent on the spawn event tag
  - Fixing of the balancing issues listed above
  - Include NG+ enemy spawns in randomization
  - Scarlet and Mann boss inclusion/exclusion settings (currently included by default)
  - Ensure that Scarlet can only spawn if the player has the DLC installed => Detect whether placing her is possible before doing so!
  - A GUI and usage documentation, followed by a downloadable and playable release build! (https://github.com/AvaloniaUI/Avalonia.Samples, https://docs.avaloniaui.net/docs/get-started/starter-tutorial/)
- Low priority:
  - A setting that reduces the HP of Hives since they're annoying... not really related to the randomizer but I want this
  - Randomization of enemies summoned by other enemies (current implementation is only working for Maelstrom)
  - More sophisticated enemy scaling to preserve an appropriate level of difficulty that matches the enemy type. Idea: Document the level of tankiness and damage output (difficulty) of each enemy plus the average stats of each enemy difficulty per zone and then use those values for the randomly placed enemies
  - (GUI) A checklist of every enemy type so the user can prevent specific ones from appearing at all
  - Fixing of the minor issues listed above
  - (GUI) Some way for the user to make certain enemies more likely to appear than others => Weighted selection
  - (GUI) Things that go with the "shuffle bosses" setting:
    - "Place a duplicate boss in the tutorial encounter"
    - "Avoid placing duplicate bosses in main story locations aside from the tutorial"
    - A list of all bosses that lets the user determine how many of them are in the shuffle pool. If the user reduces the number below the minimum then tell them that empty locations will be filled with random duplicates
- Rough ideas:
  - Custom AI for NST_M_ElderPhase1_01 so he's less boring to encounter (randomize his AI? Maybe other human enemy moves can work with his model. Otherwise make him very squishy in exchange for him having broken animations? At least then the fight can be interesting)
  - Cross-randomizing of enemies between different ranks (also changing their mesh scale maybe)
  - Automatic save file backups? Maybe? Though it's easier to just warn the user to backup their saves before using the randomizer
  - A setting for boss music. Options:
    - A: Music is based on the location (default / vanilla behavior)
    - B: Music matches the randomly placed boss
    - C: Random music
    - Notes: SoundEventTable has the music triggers. Look for ForceEventBattle and specific EventBattleState values! These settings determine what part of the zone's BGM to actually play (which includes battle music and boss themes)
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