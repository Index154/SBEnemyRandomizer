# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in very early alpha. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

Supported OS: Windows x64

Thanks to the developers of third-party software **Retoc** and **UAssetAPI** which are included in this project! (See `LICENSE.md`)

## Current state and features
- Basic functionality:
  - Most enemies and bosses are replaced by others of the same "rank"
  - The AI of replacements is 99% functional and the game can be completed! All boss encounters have been tested
  - Rudimentary level scaling: Enemies and bosses are given the stats and loot tables of whatever they're replacing
  - Excepted enemies:
    - Providence is not randomized yet
    - Both Maelstrom encounters are not randomized yet
    - Most enemies summoned by other enemies are not randomized yet
    - Elder Naytiba phase 1 is excluded from replacing other bosses
    - Stationary turrets and Hives are not randomized
  - By default bosses are shuffled in order to avoid duplicates
  - Randomization can be seeded to achieve consistent results
- Bonus features and options:
  - NPC appearances can be randomized as well (janky & funny but can occasionally make interacting with them impossible)
  - All spawns can be set to be replaced by the same enemy

## How to use
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
- ZoneTriggerTable.uasset

## Known issues
- Balancing issues:
  - Abaddon phase 2 may not be triggering correctly
  - Karakuri minions are not scaled in difficulty
  - Which properties of an enemy should be replaced and which should be retained needs another thorough check
- Minor / rare issues:
  - The secret room in Eidos 7 where you interact with the corpse and then a glass wall shatters did not have an enemy when I tested it => Needs another look
  - Gigas spawns halfway underground when replacing Belial 1 and does not attack the player until phase 2
  - The boss replacing Providence has no combat animations. Currently this boss is not being replaced
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. Placing Maelstrom in a different arena where you can walk behind it can also make the fight feel rather unbalanced => For now I am leaving the Maelstrom encounters untouched (aside from randomizing what minions they summon)
  - WLA_M_RoyalGuardFemale_01 encounter (WLA_30_E_CharS_025) no longer has battle music after being replaced. Other replaced bosses might also not have music or won't trigger a phase 2 music. I didn't pay much attention to this so far
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)
  - Bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless if there's multiple bosses in close proximity => Somehow disable this when replacing regular enemies with bosses?

## Planned features & changes
- High priority:
  - Fix balancing issues
  - Include NG+ enemy spawns in randomization
  - Go through all bosses again and check for important condition triggers and event actor effects dependent on the spawn event
  - Ensure that Scarlet can only spawn if the player has the DLC installed => Detect whether placing her is possible before doing so!
  - Randomization of enemies summoned by other enemies (current implementation is only working for Maelstrom)
  - A GUI and usage documentation, followed by a downloadable and playable release build! (https://github.com/AvaloniaUI/Avalonia.Samples, https://docs.avaloniaui.net/docs/get-started/starter-tutorial/)
- Low priority:
  - Fix minor issues
  - Try adding energy shield effect to Cocoon replacements
  - More sophisticated enemy scaling to preserve an appropriate level of difficulty that matches the enemy type. Idea: Document the level of tankiness and damage output (difficulty) of each enemy plus the average stats of each enemy difficulty per zone and then use those values for the randomly placed enemies
  - (GUI) A checklist of every enemy type so the user can prevent specific ones from appearing at all
  - (GUI) A list for the user to assign weights to every enemy type
  - (GUI) Things that go with the "shuffle bosses" setting:
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

## For developers
Requirements: .NET 8 SDK

Debugging: `dotnet run`

Publishing: `dotnet publish -c Release`