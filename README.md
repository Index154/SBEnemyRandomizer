# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade running on Windows x64.

If you use, adapt, or redistribute this project, please credit me (Index154) and link back to this repository!

Thank you to the developers of third-party software **[Retoc](https://github.com/trumank/retoc)**, **[UAssetAPI](https://github.com/atenfyr/UAssetAPI)** and **[Avalonia](https://github.com/AvaloniaUI/Avalonia)** which are included in this project! (See `LICENSE.md` for respective licenses)

## Current state and features
- Basic functionality:
  - Replaces most enemies with others of the same "rank" (small, normal and boss / elite)
  - Enemies excluded from randomization:
    - Providence is not randomized yet due to complex issues (WIP) (It can replace other bosses though)
    - Both Maelstrom encounters are neither randomized nor can they replace other bosses. This is due to unsolved balance issues (WIP)
    - Most enemies summoned by other enemies are not randomized yet (WIP)
    - Elder Naytiba phase 1 is excluded from replacing other bosses because he's boring
    - Stationary turrets and Hives are neither randomized nor can they replace other enemies. This is because of balance issues, broken AI, problematic hitboxes and spawn points located above gaps
  - Bosses are shuffled instead of fully randomized to avoid complex issues and unwanted duplicates
  - Balancing: All enemies have their HP, damage, loot drops, detection range and hardmode stat multipliers set to those of whatever enemy they replace. So most enemy encounters retain the same general difficulty as in vanilla
  - Randomization can be seeded to get predictable results (as long as the other settings are the same as well)
- Bonus features and options:
  - NPCs can be given random enemy appaerances. This is quite janky & funny but can occasionally prevent you from interacting with them. NPC apppearance randomization is not seeded so if you get stuck because of it you can just rerun the randomizer with the same seed to fix the problem
  - A fishing power multiplier
  - Enemy HP and damage multipliers
  - An additional HP multiplier for Hives because I think they're too tanky

## Usage instructions
**The mod is currently in an early testing stage so no release has been published yet. This **
1. Download the current release from the [releases page](https://github.com/Index154/SBEnemyRandomizer/releases)
2. Extract the downloaded zip to a folder of your choice
3. Run SBEnemyRandomizer.exe
4. Tweak the randomization options if you like. Hover over options with your mouse to see detailed tooltip explanations if available
5. Click the button that says "Randomize"
  1. The first time you do this you will be asked to provide the path to SB.exe in your game's installation directory. Usually this path will be something like this: `C:\Program Files (x86)\Steam\steamapps\common\StellarBlade\SB.exe`
  2. Wait for randomization to finish. It will automatically create a mod "SBEnemyRandomizer_P" and place it in your game directory under `.\SB\Content\Paks\~mods`
6. I **strongly** recommend making backups of your save files before using this mod. You can find your save files under `%LOCALAPPDATA%\SB\Saved\SaveGames`
7. Launch the game as you normally would. I highly recommend starting a new save file or NG+!
  - **IMPORTANT NOTICE:** Most bosses and some regular enemies in the game are spawned only *once*. The game remembers whether these enemies are still alive by recording their state in your save file. This means that if you randomize the enemies and load a previously played save file then these one-time-only spawns will **vanish** because the game isn't able to restore them. In other words: If you get softlocked due to missing enemies and resting at a camp does not fix it then do the following:
    1. Run the randomizer, enable the setting "Reset all spawns on zone reset" and randomize the enemies again (you can use the same seed as before). This will set all one-time-spawn enemies to instead respawn when resting or otherwise resetting the zone
    2. Launch the game and load the affected save file, then rest at a camp. This should now respawn all enemies in the current zone and update your save file to no longer be softlocked. Keep playing in this state until you're about to enter a new zone
    3. Before moving on to a new zone, close the game and run the randomizer again using the same seed but without the "Reset all spawns on zone reset" option. This is to prevent enemies in future zones from respawning unnecessarily

To remove the mod, simply delete the three files with "SBEnemyRandomizer_P" in their name from `StellarBlade\SB\Content\Paks\~mods`. Do note that due to the way enemy spawns work this might softlock all save files you've played on with the enemies randomized.

## Compatibility
The randomizer is incompatible with mods that make changes to any of the following game files:
- CharacterTable.uasset
- CharacterMoveTable.uasset
- ConditionTable.uasset
- EffectTable.uasset
- EventActorEffectTable.uasset
- EventNewGamePlusSpawnTable.uasset
- EventSpawnTable.uasset
- EventTheaterTable.uasset
- LevelTargetFilter.uasset
- M_Tachy_AI.uasset
- SkillActiveStepTable.uasset
- ZoneEventTable.uasset
- ZoneTriggerTable.uasset

## Known issues
- Major issues:
  - Exosuit has its AI disabled when replacing ElderPhase2, probably also when replacing phase 1
  - The ending cutscene is not triggering
    - Find a way to easily test cutscenes and figure out which is the correct one
    - Try triggering the ending cutscene at a different point in time "manually", for example by replacing the "adam's death" cutscene (if that is even a thing)
  - The boss replacing Providence has no combat animations. Fixing this would likely require removing the cutscene that plays before the fight but I suspect that to cause other problems. For the time being this boss is currently not being replaced so I suggest not taking Adam's hand
- Balancing issues:
  - Karakuri minions are not scaled at all
- Minor / rare issues:
  - Missing music triggers:
    - Abaddon 1, phase 2 (?)
    - Brute 2, phase 2 (?)
    - Stalker phase 2 (?)
    - Juggernaut phase 2 (??)
    - ! Tachy phase 2
    - Stalker 2, phase 2 (?)
    - ! Both RoyalGuard bosses
    - ! Belial 2 (WeaponMasterA)
    - ! Democrawler phase 2
    - ! Raven phase 2
    - ! ElderPhase2 phase 2
  - The enemy on the platform in front of the second power generator for the Hypertube clips into the ground when replaced by a tentacle, preventing you from hitting it with the sword
  - The Juggernaut replacement always has its meshScale set to 1.3, probably through the intro cutscene - For some bosses the camera can be troublesome when they're this much larger than normal
  - Mann, Scarlet and Elder Phase 2 will most likely have broken AI if they are placed in the game more than once because there are a lot of different triggers, conditions and effects tied to their spawn event that are required in order for the phase changes to work. All of these have to be cloned and modified for every copy of the boss
  - If the boss replacing Abaddon 2 is also present elsewhere in the Great Desert (or across the entire game?) then its AI remains disabled after the intro cutscene => Try removing the cutscene and modify bSpawnToActive + bHidden. But this might break the sidequest
  - Unidentified Naytiba sometimes freezes in place after certain aerial attacks. You can shoot her to get her to continue fighting
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry or blink. Placing Maelstrom in an arena where you can walk behind it can also make the fight feel rather unbalanced and clunky. For the time being these bosses are excluded from randomization and replacing others
  - Certain attacks of ElderPhase2 crash the game if there are multiple copies of him in the loaded area (I think?)
  - Bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless if there's multiple bosses in close proximity => Somehow disable this when replacing regular enemies with bosses?
- "Non-issues" which I don't plan on fixing (mostly because they're purely visual):
  - Some boss cutscenes and death animations are missing. This is usually on purpose to avoid gamebreaking issues
  - Most unremoved boss cutscenes are extremely janky looking. I consider this a positive
  - When Mann changes to phase 2 he can sometimes be seen holding two weapons at once for a short duration
  - The boss replacing Raven has her sword "equipped" for the entire battle
  - Small enemies sometimes spin in place at exponentially increasing speeds while idle (check out the funny guy on top of the boxes after solving the minigame in the Spire lobby)

## Planned features & changes (To-do list)
- High priority:
  - Test untested stuff:
    - Test Exosuit, Elder1 and Elder2 in the other spawns in Nest to see if their AI gets disabled
  - Fix major issues:
    - Find a temporary workaround for bosses clipping into the floor (either make them smaller or prevent them from being placed in problematic locations)
    - Try fixing Exosuit replacement animations by removing the intro cutscene
    - Try fixing shuffled final bosses having their AI disabled. Enable AI through eventOnBattle?
    - Figure out how the boss music triggers work and find a way to fix the broken ones
      - Notes: SoundEventTable has the music references. Look for ForceEventBattle and specific EventBattleState values! These settings determine what part of the zone's BGM to actually play. The names of the sound events seem to correspond with zone event names?
  - Fix balancing issues:
    - Fix summon randomization. This will make fixing the Karakuri minion balancing issue very easy
  - Publish an early release build
- Low priority:
  - Fix minor issues: See list above...
  - Try adding energy shield effect to Cocoon replacements to make them more obvious
  - More sophisticated enemy scaling to preserve an appropriate level of difficulty that matches the enemy type. Document the level of tankiness and damage output (=> "difficulty rank") of each enemy plus the average stats of each difficulty rank per zone and then use those values as references for the replacements
  - (GUI) A checklist of every enemy type so the user can prevent specific ones from appearing
  - (GUI) Things that go with the "shuffle bosses" setting:
    - "Try to place duplicate bosses in optional locations"
    - A list of all bosses that lets the user determine how many of them are in the shuffle pool. If the user reduces the number below the minimum then tell them that empty locations will be filled with random duplicates
- Rough ideas:
  - Option for randomizing Karakuri minions
  - Options for cross-randomizing of enemies between different ranks (also changing their mesh scale maybe)
  - Boss music behavior setting. With these options:
    - A: Music is based on the location (vanilla behavior)
    - B: Music is based on the boss
    - C: Random music

## For developers
Requirements: .NET 8 SDK

Testing with UI: `dotnet run`

Testing with console only (uses default settings): `dotnet run -- -cli`

Publishing: `dotnet publish -c Release -p:OutputType=WinExe`
