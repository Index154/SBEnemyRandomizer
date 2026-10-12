# Stellar Blade Enemy Randomizer
An enemy randomizer mod for Stellar Blade running on Windows x64.

If you use, adapt, or redistribute this project, please credit me (Index154) and link back to this repository!

Thank you to the developers of third-party software **[Retoc](https://github.com/trumank/retoc)**, **[UAssetAPI](https://github.com/atenfyr/UAssetAPI)** and **[Avalonia](https://github.com/AvaloniaUI/Avalonia)** which are included in this project! (See `LICENSE.md` for respective licenses)

## Current state and features
- Basic functionality:
  - The mod replaces all enemies by others of the same "rank" when randomized
  - The AI of replacements is 99% functional and the game can be completed (almost)! All boss encounters have been tested
  - Rudimentary level scaling: Enemies and bosses are given HP, damage, loot drops, detection range and hardmode stat multipliers of whatever they're replacing
  - Excluded enemies:
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
  - A fishing power multiplier
  - Enemy HP and damage multipliers
  - An additional HP multiplier for Hives

## Usage instructions
**The mod is currently in beta so no release has been published yet**
- Download the current release from the [releases page](https://github.com/Index154/SBEnemyRandomizer/releases)
- Extract the downloaded zip to a folder of your choice
- Run SBEnemyRandomizer.exe
- Tweak the randomization options if you like. Hover over options with your mouse to see detailed tooltip explanations if available
- Click the button that says "Randomize"
  - The first time you do this you will be asked to provide the path to SB.exe in your game's installation directory. Usually this path will be something like this: `C:\Program Files (x86)\Steam\steamapps\common\StellarBlade\SB.exe`
  - Wait for randomization to finish. It will automatically create a mod "SBEnemyRandomizer_P" and place it in your game directory under `.\SB\Content\Paks\~mods`
- Launch the game as you normally would. I strongly recommend starting a new save file or NG+!
  - **IMPORTANT KNOWLEDGE:** Most bosses and some regular enemies in the game are only spawned *once* immediately upon entering the zone they're in. The game remembers whether these enemies are still there by recording them in your save file in some way. Loading a used save file after randomizing will cause these previously spawned enemies to **vanish** instead of being replaced. If you experience this issue and resting at a camp does not fix it then proceed as follows:
    - Run the randomizer, enable the setting "Reset all spawns on zone reset" and randomize the enemies again (you can use the same seed as before)
    - Launch the game and load the affected save file, then rest at a camp. This should now respawn all enemies in the current zone (possibly including bosses you've already defeated) and update your save file to no longer be softlocked
    - Close the game and run the randomizer again using the same seed but without the "Reset all spawns on zone reset" option. After this you should be able to continue playing normally

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
  - Exosuit has its AI disabled if it replaces ElderPhase2
    - Enable AI through eventOnBattle?
  - The ending cutscene is not triggering
    - The theater event seems to be targeting specific characters like M_ElderPhase1 and M_Exosuit
    - Find a way to easily test cutscenes and figure out which is the correct one
    - Try triggering the ending cutscene at a different point in time "manually", for example through EventOnDead. Could be an easy workaround!
- Balancing issues:
  - Abaddon phase 2 may not be triggering correctly
  - Karakuri minions are not scaled in difficulty
- Minor / rare issues:
  - Gigas spawns halfway underground when replacing Belial 1 and does not attack the player until phase 2 because of it
    - => Test Gigas in other arenas to see if it happens elsewhere. Reduce his meshscale for these locations as an easy workaround?
  - Unidentified Naytiba sometimes freezes in place after her aerial attacks. You can just shoot her to get her to continue fighting so it's not a big deal
  - The boss replacing Providence has no combat animations. To prevent this issue this boss is currently not being replaced
  - Replacing Maelstrom with a different boss can result in unwinnable situations due to ammo shortage + no way to parry etc. Placing Maelstrom in a different arena where you can walk behind it can also make the fight feel rather unbalanced => For now I am leaving the Maelstrom encounters untouched (aside from randomizing what minions they summon)
  - The RoyalGuardFemale encounters no longer have battle music after being replaced. Other replaced bosses might also not have music or won't trigger a phase 2 music. I didn't pay much attention to this so far
  - Certain attacks of NST_M_ElderPhase2_01 crash the game if there are multiple copies of him in the loaded area (I think?)
  - Bosses always put a large HP bar at the top of the screen but you can only ever see one of them at a time, making it totally useless if there's multiple bosses in close proximity => Somehow disable this when replacing regular enemies with bosses?
- "Non-issues" which I don't plan on fixing:
  - Some boss cutscenes and death animations are missing. This is usually on purpose to avoid gamebreaking issues
  - Most cutscenes are extremely janky looking. I consider this a desirable feature :D
  - When Mann changes to phase 2 he can sometimes be seen holding two weapons at once for a short duration
  - The boss replacing Raven has her sword "equipped" for the entire battle (purely visual)

## Planned features & changes
- High priority:
  - Fix major issues
  - Fix balancing issues
  - Go through all bosses again and check for important condition triggers and event actor effects dependent on the spawn event
  - Ensure that Scarlet is removed properly if the DLC is uninstalled
  - Randomization of enemies summoned by other enemies (current implementation is only working for Maelstrom)
  - GUI and usage documentation, followed by a downloadable and playable release build
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
  - Boss music setting:
    - A: Music is based on the location (default / vanilla behavior)
    - B: Music matches the randomly placed boss
    - C: Random music
    - Notes: SoundEventTable has the music triggers. Look for ForceEventBattle and specific EventBattleState values! These settings determine what part of the zone's BGM to actually play (which includes battle music and boss themes)
      - The names of the sound events seem to correspond with zone event names?

## For developers
Requirements: .NET 8 SDK

Testing with UI: `dotnet run`

Testing with console only (uses default settings): `dotnet run -- -cli`

Publishing: `dotnet publish -c Release -p:OutputType=WinExe`
