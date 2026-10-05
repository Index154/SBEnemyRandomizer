# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade. Currently in early beta. I'm a mostly self-taught hobby programmer so please excuse any inefficient or unclean code.

Supported OS: Windows x64

Thanks to the developers of third-party software **Retoc** and **UAssetAPI** which are included in this project! (See `LICENSE.md`)

## Current state and features
- Basic functionality:
  - The mod replaces all enemies by others of the same "rank" when randomized
  - The AI of replacements is 99% functional and the game can be completed (almost)! All boss encounters have been tested
  - Rudimentary level scaling: Enemies and bosses are given the stats and loot tables of whatever they're replacing
  - Excepted enemies:
    - Providence is not randomized yet due to unidentified issues (WIP)
    - Both Maelstrom encounters are not randomized yet for balance reasons (WIP)
    - Most enemies summoned by other enemies are not randomized (WIP)
    - Elder Naytiba phase 1 is excluded from replacing other bosses because he's boring
    - Stationary turrets and Hives are not randomized because of balance reasons, broken AI, problematic hitboxes and floating spawn points
  - Bosses are shuffled among each other as to avoid extra duplicates and complex issues
  - Randomization can be seeded to achieve reproducible results
- Bonus features and options:
  - NPC appearances can be randomized as well. This is quite janky & funny but can occasionally prevent you from interacting with them. NPC apppearance randomization is not seeded so if you get stuck because of it you can just rerun the randomizer with the same seed to fix the problem
  - All spawns can also be set to be replaced by the same enemy if desired. **Warning:** This is highly experimental since I haven't done a lot of testing with it. Some bosses like Mann and Scarlet will DEFINITELY not work when spawned multiple times across the game! Expect softlocks and crashes

## How to use
Work in progress...

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
  - Gigas spawns halfway underground when replacing Belial 1 and does not attack the player until phase 2 because of it
  - The boss replacing Providence has no combat animations. Currently this boss is not being replaced
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. Placing Maelstrom in a different arena where you can walk behind it can also make the fight feel rather unbalanced => For now I am leaving the Maelstrom encounters untouched (aside from randomizing what minions they summon)
  - WLA_M_RoyalGuardFemale_01 encounter (WLA_30_E_CharS_025) no longer has battle music after being replaced. Other replaced bosses might also not have music or won't trigger a phase 2 music. I didn't pay much attention to this so far
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)
  - Bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless if there's multiple bosses in close proximity => Somehow disable this when replacing regular enemies with bosses?
- "Non-issues" which I don't plan on fixing:
  - Some boss cutscenes and death animations are missing. This is usually on purpose to avoid softlocks
  - Most cutscenes that remain are extremely janky looking. I see this as a positive
  - When Mann changes to phase 2 he can be seen holding two weapons at once for a short duration
  - The boss replacing Raven has her sword "equipped" for the entire battle (purely visual)

## Planned features & changes
- High priority:
  - Fix the ending (final cutscene does not trigger)
    - The theater event seems to be targeting specific characters like M_ElderPhase1 and M_Exosuit but because they don't exist it's taking random enemies from Wasteland instead?
    - Find a way to easily test cutscenes and figure out which is the correct one
    - Try triggering the ending cutscene at a different point in time "manually", for example through EventOnDead. Could be an easy workaround!
  - Fix final boss replacement issues: Exosuit has its AI disabled if it replaces ElderPhase2x
  - Fix balancing issues
  - Include NG+ enemy spawns in randomization
  - Go through all bosses again and check for important condition triggers and event actor effects dependent on the spawn event
  - Ensure that Scarlet can only spawn if the player has the DLC installed
    - Uninstall the DLC and check if she's still in the charactertable
    - => Implement a check in the code that excludes her if the DLC content is missing
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