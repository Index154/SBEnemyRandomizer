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
  - The game has not been tested beyond Belial 2. Some softlocks probably still remain
  - The boss replacing Belial 2 has no combat animations
- Balancing issues:
  - Karakuri minions are not scaled in difficulty
  - Which properties of an enemy should be replaced and which should be retained needs another thorough check
- Minor / rare issues:
  - Gigas spawns halfway underground when replacing Belial 1 and does not attack the player until phase 2
  - The secret room in Eidos 7 where you interact with the corpse and then a glass wall shatters did not have an enemy when I tested it => Needs another look
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. Placing Maelstrom in a different arena where you can walk behind it can also make the fight feel rather unbalanced => For now I am leaving the Maelstrom encounters untouched (aside from randomizing what minions they summon)
  - WLA_M_RoyalGuardFemale_01 encounter (WLA_30_E_CharS_025) no longer has battle music after being replaced. Other replaced bosses might also not have music or at least no phase 2 music, probably. I didn't pay much attention to this yet
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)
  - All bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless => Somehow disable this when replacing regular enemies with bosses?

## Planned features and changes (from most to least important)
- High priority:
  - Fixing of the major issues
  - Double-check if Hedgeboar Brute phase 2 is triggering
  - Double-check if WLB Abaddon phase 2 is triggering correctly (from the start?)
  - Go through all bosses again and check for important condition triggers and event actor effects dependent on the spawn event tag
  - Fixing of the balancing issues
  - Include NG+ enemy spawns in randomization
  - Scarlet and Mann boss inclusion/exclusion settings (currently included by default)
  - Ensure that Scarlet can only spawn if the player has the DLC installed => Detect whether placing her is possible before doing so!
  - A GUI and usage documentation, followed by a downloadable and playable release build! (https://github.com/AvaloniaUI/Avalonia.Samples, https://docs.avaloniaui.net/docs/get-started/starter-tutorial/)
- Low priority:
  - Fixing of the minor issues
  - Try adding energy shield effect to Cocoon replacements
  - Randomization of enemies summoned by other enemies (current implementation is only working for Maelstrom)
  - More sophisticated enemy scaling to preserve an appropriate level of difficulty that matches the enemy type. Idea: Document the level of tankiness and damage output (difficulty) of each enemy plus the average stats of each enemy difficulty per zone and then use those values for the randomly placed enemies
  - (GUI) A checklist of every enemy type so the user can prevent specific ones from appearing at all
  - Fixing of the minor issues listed above
  - (GUI) Some way for the user to make certain enemies more likely to appear than others => Weighted selection
  - (GUI) Things that go with the "shuffle bosses" setting:
    - "Replace the tutorial Hedgeboar Brute with a duplicate boss"
    - "Try to place duplicate bosses in optional locations"
    - A list of all bosses that lets the user determine how many of them are in the shuffle pool. If the user reduces the number below the minimum then tell them that empty locations will be filled with random duplicates
- Rough ideas:
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
The randomizer is incompatible with mods that make changes to any of the following game files:
- CharacterTable.uasset
- CharacterMoveTable.uasset
- ConditionTable.uasset
- EffectTable.uasset
- EventActorEffectTable.uasset
- EventSpawnTable.uasset
- EventTheaterTable.uasset
- LevelTargetFilter.uasset
- M_Tachy_AI.uasset
- SkillActiveStepTable.uasset
- ZoneEventTable.uasset

## For developers
Requirements: .NET 8 SDK

Debugging: `dotnet run`

Publishing: `dotnet publish -c Release`