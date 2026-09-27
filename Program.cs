using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using UAssetAPI.Unversioned;
using System.Diagnostics;
using static SBEnemyRandomizer.src.GlobalSettings;
using static SBEnemyRandomizer.src.GameData;
using static SBEnemyRandomizer.src.Logger;
using System.Text.RegularExpressions;
using System.Text;

namespace SBEnemyRandomizer;

public class Program{

    async static Task Main()
    {
        // Create folders
        Directory.CreateDirectory(unpackTablePath);
        Directory.CreateDirectory(repackTablePath);
        Directory.CreateDirectory(unpackAIPath);
        Directory.CreateDirectory(repackAIPath);

        // Extract assets from game files and convert to legacy format using retoc
        await RetocToLegacy(characterTable);
        await RetocToLegacy(eventSpawnTable);
        await RetocToLegacy(levelTargetFilterTable);
        await RetocToLegacy(eventActorEffectTable);
        await RetocToLegacy(conditionTable);
        await RetocToLegacy(effectTable);
        await RetocToLegacy(characterMoveTable);
        await RetocToLegacy(skillActiveStepTable);
        await RetocToLegacy(zoneEventTable);
        await RetocToLegacy(tachyAI);

        // Random seeds
        Random seedGenerator = (seed == -1) ? new() : new(seed);
        int randoSeed = seedGenerator.Next(int.MinValue, int.MaxValue);
        Random rndCategory = new(randoSeed);
        Random rndAlias = new(randoSeed);
        Log($"Seed = {seed}");
        
        // Load legacy uasset files and modify them
        UAsset charactersAsset = ReadUAsset($"{unpackTablePath}/{characterTable}", mapPath);
        UAsset spawnEventsAsset = ReadUAsset($"{unpackTablePath}/{eventSpawnTable}", mapPath);
        UAsset levelTargetFiltersAsset = ReadUAsset($"{unpackTablePath}/{levelTargetFilterTable}", mapPath);
        UAsset eventActorEffectsAsset = ReadUAsset($"{unpackTablePath}/{eventActorEffectTable}", mapPath);
        UAsset conditionsAsset = ReadUAsset($"{unpackTablePath}/{conditionTable}", mapPath);
        UAsset effectsAsset = ReadUAsset($"{unpackTablePath}/{effectTable}", mapPath);
        UAsset characterMovesAsset = ReadUAsset($"{unpackTablePath}/{characterMoveTable}", mapPath);
        UAsset skillActiveStepsAsset = ReadUAsset($"{unpackTablePath}/{skillActiveStepTable}", mapPath);
        UAsset zoneEventsAsset = ReadUAsset($"{unpackTablePath}/{zoneEventTable}", mapPath);
        UAsset tachyAIAsset = ReadUAsset($"{unpackAIPath}/{tachyAI}", mapPath);
        if(randomizeNPCAppearances) ShuffleNPCAppearances(charactersAsset);
        FindAndModifyEffects(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, rndCategory, rndAlias);
        RandomizeSpawns(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, rndCategory, rndAlias);
        ModifySkillActiveSteps(skillActiveStepsAsset);
        ModifyCharacterMoves(characterMovesAsset);
        ModifyZoneEvents(zoneEventsAsset);
        ModifyAI(tachyAIAsset);
        Check(effectsAsset);

        // Save modified uassets
        Log("Saving files...");
        charactersAsset.Write($"{repackTablePath}/{characterTable}");
        spawnEventsAsset.Write($"{repackTablePath}/{eventSpawnTable}");
        levelTargetFiltersAsset.Write($"{repackTablePath}/{levelTargetFilterTable}");
        eventActorEffectsAsset.Write($"{repackTablePath}/{eventActorEffectTable}");
        conditionsAsset.Write($"{repackTablePath}/{conditionTable}");
        effectsAsset.Write($"{repackTablePath}/{effectTable}");
        characterMovesAsset.Write($"{repackTablePath}/{characterMoveTable}");
        skillActiveStepsAsset.Write($"{repackTablePath}/{skillActiveStepTable}");
        zoneEventsAsset.Write($"{repackTablePath}/{zoneEventTable}");
        tachyAIAsset.Write($"{repackAIPath}/{tachyAI}");

        // Repack modified uassets, convert to game-ready zen format using retoc and move them to the mods folder
        await RetocToZen();

        // Save file reset for testing
        if(replaceSaveDataOnRun) File.Copy(Environment.ExpandEnvironmentVariables("%userprofile%/Downloads/StellarBladeSave03.sav"), Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/SB/Saved/SaveGames/76561198169967897/StellarBladeSave03.sav"), overwrite: true);
    }

    async static Task RunRetoc(string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = retocPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
    }

    async static Task RetocToLegacy(string uAssetName)
    {
        if(File.Exists($"{unpackTablePath}/{uAssetName}") || File.Exists($"{unpackAIPath}/{uAssetName}"))
        {
            Log($"[{uAssetName}] has already been unpacked");
            return;
        }
        await RunRetoc($"to-legacy \"{gamePakPath}\" \"{tempPath}/unpacked\" -f \"{uAssetName}\"");
        Log($"Unpacked [{uAssetName}]!");
    }

    async static Task RetocToZen()
    {
        string modName = "SBEnemyRandomizer_P";
        await RunRetoc($"to-zen \"{tempPath}/modified\" \"{tempPath}/{modName}.utoc\" --version UE4_26");

        Directory.CreateDirectory(gamePakPath + "/~mods");
        File.Move($"{tempPath}/{modName}.pak", gamePakPath + $"/~mods/{modName}.pak", overwrite: true);
        File.Move($"{tempPath}/{modName}.ucas", gamePakPath + $"/~mods/{modName}.ucas", overwrite: true);
        File.Move($"{tempPath}/{modName}.utoc", gamePakPath + $"/~mods/{modName}.utoc", overwrite: true);
        Log("Mod files moved to game directory!");
    }

    static UAsset ReadUAsset(string uAssetPath, string mapPath){
        return new UAsset(uAssetPath, EngineVersion.VER_UE4_26, new Usmap(mapPath));
    }

    static void RandomizeSpawns(UAsset charactersAsset, UAsset spawnEventsAsset, UAsset levelTargetFiltersAsset, UAsset eventActorEffectsAsset, UAsset conditionsAsset, UAsset effectsAsset, Random rndCategory, Random rndAlias)
    {
        Log("Randomizing enemies...");
        DataTableExport spawnEventsTable = (DataTableExport)spawnEventsAsset.Exports[0];
        List<StructPropertyData> spawnEvents = spawnEventsTable.Table.Data;
        DataTableExport charactersTable = (DataTableExport)charactersAsset.Exports[0];
        List<StructPropertyData> characters = charactersTable.Table.Data;

        Dictionary<string, string> consistentReplacements = [];
        // For challenge mode bosses - Currently unused because the zones are commented out in GameData.cs
        Dictionary<EnemyRank, Dictionary<string, string[]>> challengeBossesToPlaceOnce = new(){
            [EnemyRank.Boss] = EnemiesToPlaceOnce[EnemyRank.Boss].ToDictionary(pair => pair.Key, pair => pair.Value.ToArray())
        };

        // Go through spawn events and modify all that are relevant
        foreach(StructPropertyData row in spawnEvents)
        {
            ArrayPropertyData characterAliasArray = (ArrayPropertyData)row["CharacterAlias"];
            if(characterAliasArray.Value == null || characterAliasArray.Value.Length < 1) continue;
            NamePropertyData characterAlias = (NamePropertyData)characterAliasArray.Value[0];

            // Ignore spawn events in irrelevant zones
            NamePropertyData zone = (NamePropertyData)row["Zone"];
            if(zone.Value == null || RelevantZones.IndexOf(zone.Value.ToString()) == -1) continue;
            // Testing for specific zone
            if(customZoneNameRestriction != "" && !zone.Value.ToString().Contains(customZoneNameRestriction)) continue;

            // Determine enemy rank
            EnemyRank rank;
            if(EnemiesToReplace[EnemyRank.Animal].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Animal;
            else if(EnemiesToReplace[EnemyRank.Normal].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Normal;
            else if(EnemiesToReplace[EnemyRank.Boss].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Boss;
            else continue;
            if(customRankRestriction != null && rank != customRankRestriction) continue;

            // Determine replacement enemy
            string replacementAlias = "";
            if(consistentReplacements.ContainsKey(characterAlias.Value.ToString()))
            {
                replacementAlias = consistentReplacements[characterAlias.Value.ToString()];
            }
            else if(onlyShuffleExistingBoss && rank == EnemyRank.Boss)
            {
                Dictionary<string, string[]> enemyShuffleDict = EnemiesToPlaceOnce[rank];
                if(rank == EnemyRank.Boss && zone.Value.ToString().Contains("_Boss_")) enemyShuffleDict = challengeBossesToPlaceOnce[EnemyRank.Boss];
                
                // Pick random category and enemy
                string keyToRemoveFrom = "";
                while(replacementAlias == "")
                {
                    KeyValuePair<string, string[]> randomKVP = enemyShuffleDict.ElementAt(rndCategory.Next(enemyShuffleDict.Count));
                    string[] replacementCategory = randomKVP.Value;
                    if(!characterAlias.Value.ToString().Contains(randomKVP.Key))
                    {
                        keyToRemoveFrom = randomKVP.Key;
                        replacementAlias = replacementCategory[rndAlias.Next(replacementCategory.Length)];
                    }
                }

                // Remove selected enemy from the list and remove category if it's empty
                List<string> tempList = enemyShuffleDict[keyToRemoveFrom].ToList();
                tempList.Remove(replacementAlias);
                enemyShuffleDict[keyToRemoveFrom] = tempList.ToArray();
                if(enemyShuffleDict[keyToRemoveFrom].Length < 1) enemyShuffleDict.Remove(keyToRemoveFrom);

            }else{
                // Select random enemy from different category
                replacementAlias = PickDiffCategoryEnemy(characterAlias.Value.ToString(), rank, rndCategory, rndAlias);
            }

            // Force specific enemy for testing
            if(forceReplacementEnemyName != "" && (forceReplacementForEvent == "" || forceReplacementForEvent == row.Name.Value.ToString())) replacementAlias = forceReplacementEnemyName;

            // These bosses have two spawn events each so prevent these from being randomized separately
            if(!consistentReplacements.ContainsKey(characterAlias.Value.ToString()) && (characterAlias.Value.ToString() == "NST_M_Raven_01" || characterAlias.Value.ToString() == "NST_M_ElderPhase1_01"))
            {
                consistentReplacements.Add(characterAlias.Value.ToString(), replacementAlias);
            }

            // Clone and modify the replacement charactertable entry. This is necessary so we can have different scaling for each copy of an enemy. We could add some more checks to reduce some of the duplicate entries this produces but I don't think that's worth the effort
            string newEnemyName = ModifyEnemy(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, characterAlias.Value.ToString(), replacementAlias, characters, row, rndCategory, rndAlias, zone.Value.ToString());

            // Modify the spawn event to spawn the new custom character instead
            characterAlias.Value = FName.FromString(spawnEventsAsset, newEnemyName);
            //Log($"{incrementalID - 1} | {row.Name.Value}: {characterAlias.Value} => {replacementAlias}");
        }
    }

    static string ModifyEnemy(UAsset charactersAsset, UAsset spawnEventsAsset, UAsset levelTargetFiltersAsset, UAsset eventActorEffectsAsset, UAsset conditionsAsset, UAsset effectsAsset, string characterAlias, string replacementAlias, List<StructPropertyData> characters, StructPropertyData spawnEvent, Random rndCategory, Random rndAlias, string zoneName)
    {
        // Clone the enemy before modifying it
        StructPropertyData originalEnemy = new();
        StructPropertyData replacementEnemy = new();
        foreach(StructPropertyData ch in characters){
            if(ch.Name.Value.ToString() == characterAlias) originalEnemy = ch;
            if(ch.Name.Value.ToString() == replacementAlias) replacementEnemy = ch;
        }

        // Assign the new enemy a new name and a unique ID
        StructPropertyData newEnemy = (StructPropertyData)replacementEnemy.Clone();
        string vanillaEnemyName = newEnemy.Name.Value.ToString();
        string spawnEventName = spawnEvent.Name.Value.ToString();
        string newEnemyName = Regex.Replace(replacementAlias, @".*_M_", $"_M_{incrementalID}_");
        newEnemyName = zoneName.Replace("Zone_", "") + newEnemyName;
        ((UInt32PropertyData)newEnemy["ID"]).Value = incrementalID;

        // Keep some of the properties of the replaced enemy such as combat data and drop tables for balance reasons
        foreach(string property in EnemyPropertiesToRetain)
        {
            newEnemy[property].RawValue = originalEnemy[property].RawValue;
        }
        // Lower HP for testing
        if(enemyHPForTesting > 0) ((IntPropertyData)newEnemy["MaxHP"]).Value = enemyHPForTesting;
        
        // Add ranged damage multipliers to enemies in ATL and AYL. Also check for Projectile2_Summon and M_Maelstrom for the function call from FindAndModifyEffects()
        ArrayPropertyData defaultEffectArray = (ArrayPropertyData)newEnemy["DefaultEffectArray"];
        if(spawnEventName.Contains("AYL_") || spawnEventName.Contains("Projectile2_Summon")){
            defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, "RangeAttackDamageReductionRate_AYL")}).ToArray();
        }
        else if(spawnEventName.Contains("ATL_") || spawnEventName.Contains("M_Maelstrom")){
            defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, "RangeAttackDamageReductionRate_ATL")}).ToArray();
        }

        // Randomize enemies summoned by other enemies
        ArrayPropertyData effectWhenZeroHPArray = (ArrayPropertyData)newEnemy["EffectWhenZeroHPArray"];
        if(defaultEffectArray.Value.Any(val => ((NamePropertyData)val).Value.ToString().Contains("Summon")))
        {
            ReplaceSummonEffect(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, characters, spawnEvent, rndCategory, rndAlias, zoneName, defaultEffectArray);
        }
        else if(effectWhenZeroHPArray != null && effectWhenZeroHPArray.Value.Any(val => ((NamePropertyData)val).Value != null && ((NamePropertyData)val).Value.ToString().Contains("Summon")))
        {
            ReplaceSummonEffect(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, characters, spawnEvent, rndCategory, rndAlias, zoneName, effectWhenZeroHPArray);
        }

        // Reset spawn when loading save - For testing. Enemies with SaveType Save will have their name and last position (?) written into your save file. This prevents rerandomizing enemies mid-playthrough without softlocking the game (in many cases). Most enemies are spawned the moment you enter the zone so it's very unwieldy to test the game without changing the SaveType. (Exlude spawnEventname M_Maelstrom for the call from FindAndModifyEffects())
        if(resetAllEnemySpawnsOnLoad && !spawnEventName.Contains("M_Maelstrom"))
        {
            EnumPropertyData saveType = (EnumPropertyData)spawnEvent["SaveType"];
            if(saveType.Value.ToString() == "ESBZoneObjSaveType_Save") saveType.Value = FName.FromString(spawnEventsAsset, "ESBZoneObjSaveType_ResetZone");
        }

        // Conditional fixes
        // --------------------------------------------------------------------------------------

        // Add immortality if required for a location-based cutscene. Otherwise remove immortality from certain bosses. This effect would usually be removed through a cutscene but we avoid most boss-specific cutscenes because they teleport you to arbitrary coordinates
        if(SpawnsRequiringImmortality.Contains(spawnEventName)){
            if(!defaultEffectArray.Value.Any(val => ((NamePropertyData)val).Value.ToString() == "Passive_Immortal"))
            {
                defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, "Passive_Immortal")}).ToArray();
            }
        }
        else if(BossesWithImmortality.Contains(vanillaEnemyName))
        {
            defaultEffectArray.Value = defaultEffectArray.Value.Where(val => val.ToString() != "Passive_Immortal").ToArray();
        }

        // Remove special finisher stances from bosses (triggers a specific cutscene on defeat)
        ArrayPropertyData stanceAliasArray = (ArrayPropertyData)newEnemy["StanceAliasArray"];
        if(BossesWithFinishStance.Contains(vanillaEnemyName))
        {
            stanceAliasArray.Value = stanceAliasArray.Value.Where(val => !val.ToString().Contains("_Finish")).ToArray();
        }
        // Add special finisher stances to bosses in locations that normally have them
        if(SpawnsRequiringStances.ContainsKey(spawnEventName))
        {
            stanceAliasArray.Value = stanceAliasArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, SpawnsRequiringStances[spawnEventName])}).ToArray();
        }

        // Add other default effects to enemies in specific spawn locations
        if(SpawnsWithEffects.ContainsKey(spawnEventName))
        {
            foreach(string effect in SpawnsWithEffects[spawnEventName])
            {
                defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, effect)}).ToArray();
            }
        }

        // Fix Mann problems: Cutscenes teleport the player out of bounds and phase changes don't trigger
        if(vanillaEnemyName == "CHAL_XION_M_Mann_01")
        {
            // Change levelTargetFilter to target the new spawn event so the HP conditions for the phase changes are functional
            DataTableExport levelTargetFiltersTable = (DataTableExport)levelTargetFiltersAsset.Exports[0];
            List<StructPropertyData> levelTargetFilters = levelTargetFiltersTable.Table.Data;
            foreach(StructPropertyData row in levelTargetFilters)
            {
                if(row.Name.Value.ToString() == "Xion_Boss_Mann_LevelTargetFilter_001")
                {
                    ((NamePropertyData)row["SpawnPointName"]).Value = FName.FromString(levelTargetFiltersAsset, spawnEventName);
                }
            }

            // Change the target tag of the eventActorEffects to the tag of the spawnEvent - Required for the change to phase 2 to work
            // Modifying the tag of the spawnEvent would lead to issues with logic from the original arena (like intro cutscenes)
            DataTableExport eventActorEffectsTable = (DataTableExport)eventActorEffectsAsset.Exports[0];
            List<StructPropertyData> eventActorEffects = eventActorEffectsTable.Table.Data;
            foreach(StructPropertyData row in eventActorEffects)
            {
                if(row.Name.Value.ToString().Contains("Xion_Boss_Mann"))
                {
                    NamePropertyData tagName = (NamePropertyData)row["TargetTagName"];
                    if(tagName.Value != null && tagName.Value.ToString() == "M_Mann")
                    {
                        StructPropertyData eventActorClone = row;
                        // Clone them for each Mann spawn so we can have more than one - But there's more work to be done for that...
                        //StructPropertyData eventActorClone = (StructPropertyData)row.Clone();
                        //((UInt32PropertyData)eventActorClone["ID"]).Value = incrementalEventActorEffectID;
                        //eventActorClone.Name = FName.FromString(eventActorEffectsAsset, $"{incrementalEventActorEffectID}_{row.Name.Value}_{spawnEventName}");
                        NamePropertyData newActorTag = (NamePropertyData)row["TargetTagName"];
                        newActorTag.Value = FName.FromString(eventActorEffectsAsset, ((NamePropertyData)spawnEvent["TagName"]).Value.ToString());

                        //eventActorEffects.Add(eventActorClone);
                        incrementalEventActorEffectID++;
                    }
                }
            }

            // Add conditional trigger for phase 2 to the spawn event
            ArrayPropertyData ConditionsTrigger = (ArrayPropertyData)spawnEvent["ConditionsTrigger"];
                ConditionsTrigger.Value = ConditionsTrigger.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_Condition_002")}).ToArray();
            ArrayPropertyData ConditionTriggerEvent = (ArrayPropertyData)spawnEvent["ConditionTriggerEvent"];
                ConditionTriggerEvent.Value = ConditionTriggerEvent.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_E_ActorEff_006")}).ToArray();
            ArrayPropertyData ConditionTriggerRunType = (ArrayPropertyData)spawnEvent["ConditionTriggerRunType"];
                ConditionTriggerRunType.Value = ConditionTriggerRunType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerRunType_Once")}).ToArray();
            ArrayPropertyData ConditionTriggerExecType = (ArrayPropertyData)spawnEvent["ConditionTriggerExecType"];
                ConditionTriggerExecType.Value = ConditionTriggerExecType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerExecType_RunTime")}).ToArray();
        }

        // Fix Scarlet problems: Cutscenes teleport the player out of bounds and phase changes don't trigger
        if(vanillaEnemyName == "CHAL_M_Scarlet_01")
        {
            // Remove levelTargetFilter from phase 2 HP condition
            DataTableExport conditionsTable = (DataTableExport)conditionsAsset.Exports[0];
            List<StructPropertyData> conditions = conditionsTable.Table.Data;
            foreach(StructPropertyData row in conditions)
            {
                if(row.Name.Value.ToString() == "NK_Boss_Scarlet_Condition_001")
                {
                    ((StrPropertyData)row["CustomStr01"]).Value = FString.FromString(null, Encoding.UTF8);
                    ((StrPropertyData)row["CustomStr01"]).IsZero = true;
                }
            }

            // Change the target tag of the eventActorEffects to the tag of the spawnEvent - Required for the change to phase 2 to work
            // Modifying the tag of the spawnEvent would lead to issues with logic from the original arena (like intro cutscenes)
            DataTableExport eventActorEffectsTable = (DataTableExport)eventActorEffectsAsset.Exports[0];
            List<StructPropertyData> eventActorEffects = eventActorEffectsTable.Table.Data;
            foreach(StructPropertyData row in eventActorEffects)
            {
                if(row.Name.Value.ToString().Contains("NK_Boss_Scarlet"))
                {
                    NamePropertyData tagName = (NamePropertyData)row["TargetTagName"];
                    if(tagName.Value != null && tagName.Value.ToString() == "M_Scarlet")
                    {
                        StructPropertyData eventActorClone = row;
                        // Clone them for each Mann spawn so we can have more than one - But there's more work to be done for that...
                        //StructPropertyData eventActorClone = (StructPropertyData)row.Clone();
                        //((UInt32PropertyData)eventActorClone["ID"]).Value = incrementalEventActorEffectID;
                        //eventActorClone.Name = FName.FromString(eventActorEffectsAsset, $"{incrementalEventActorEffectID}_{row.Name.Value}_{spawnEventName}");
                        NamePropertyData newActorTag = (NamePropertyData)row["TargetTagName"];
                        newActorTag.Value = FName.FromString(eventActorEffectsAsset, ((NamePropertyData)spawnEvent["TagName"]).Value.ToString());

                        //eventActorEffects.Add(eventActorClone);
                        incrementalEventActorEffectID++;
                    }
                }
            }

            // Add conditional triggers for phase 2 to the spawn event (no theater events)
            ArrayPropertyData ConditionsTrigger = (ArrayPropertyData)spawnEvent["ConditionsTrigger"];
                ConditionsTrigger.Value = ConditionsTrigger.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "NK_Boss_Scarlet_Condition_001")}).ToArray();
                ConditionsTrigger.Value = ConditionsTrigger.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "NK_Boss_Scarlet_Condition_001")}).ToArray();
            ArrayPropertyData ConditionTriggerEvent = (ArrayPropertyData)spawnEvent["ConditionTriggerEvent"];
                ConditionTriggerEvent.Value = ConditionTriggerEvent.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "NK_Boss_Scarlet_E_ActorEff_003")}).ToArray();
                ConditionTriggerEvent.Value = ConditionTriggerEvent.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "NK_Boss_Scarlet_E_ActorEff_006")}).ToArray();
            ArrayPropertyData ConditionTriggerRunType = (ArrayPropertyData)spawnEvent["ConditionTriggerRunType"];
                ConditionTriggerRunType.Value = ConditionTriggerRunType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerRunType_Once")}).ToArray();
                ConditionTriggerRunType.Value = ConditionTriggerRunType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerRunType_Once")}).ToArray();
            ArrayPropertyData ConditionTriggerExecType = (ArrayPropertyData)spawnEvent["ConditionTriggerExecType"];
                ConditionTriggerExecType.Value = ConditionTriggerExecType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerExecType_RunTime")}).ToArray();
                ConditionTriggerExecType.Value = ConditionTriggerExecType.Value.Append(new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerExecType_RunTime")}).ToArray();
        }

        // Save cloned enemy under new name
        newEnemy.Name = FName.FromString(charactersAsset, newEnemyName);
        characters.Add(newEnemy);
        incrementalID++;
        return newEnemyName;
    }

    static void ModifySkillActiveSteps(UAsset asset)
    {
        Log("Modifying skillActiveSteps...");
        DataTableExport skillActiveStepsTable = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> skillActiveSteps = skillActiveStepsTable.Table.Data;
        
        foreach(StructPropertyData row in skillActiveSteps)
        {
            ArrayPropertyData selfMoves = (ArrayPropertyData)row["SelfMoveAliasArray"];
            ArrayPropertyData targetMoves = (ArrayPropertyData)row["TargetMoveAliasArray"];

            // Remove RavenBeast world coordinate teleportation steps
            if(row.Name.Value.ToString().Contains("M_RavenBeast_"))
            {
                // Old solution which caused some AI bugs - No longer necessary after altering the characterMove values
                /*selfMoves.Value = selfMoves.Value.Where(val =>
                    !val.ToString().Contains("M_RavenBeast_PhaseChange3_Move2") &&
                    !val.ToString().Contains("M_RavenBeast_ColonyDashSky_Move1") &&
                    !val.ToString().Contains("M_RavenBeast_ColonyDashSky_Move3") &&
                    !val.ToString().Contains("M_RavenBeast_ColonyDashSky_Move5") &&
                    !val.ToString().Contains("M_RavenBeast_ColonyDashSky_Move7") &&
                    !val.ToString().Contains("M_RavenBeast_FlyRoutine1_Move2")
                ).ToArray();*/

                targetMoves.Value = targetMoves.Value.Where(val => !val.ToString().Contains("M_RavenBeast_PhaseChange3_Move4")).ToArray();
            }
        }
    }

    static void ModifyCharacterMoves(UAsset asset)
    {
        Log("Modifying character movements...");
        DataTableExport characterMoveTable = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> characterMoves = characterMoveTable.Table.Data;
        
        foreach(StructPropertyData row in characterMoves)
        {
            // Replace M_RavenBeast world position movement with nothing-movement to prevent out of bounds teleportation
            if("M_RavenBeast_PhaseChange3_Move2, M_RavenBeast_PhaseChange3_Move4, M_RavenBeast_ColonyDashSky_Move1, M_RavenBeast_ColonyDashSky_Move3, M_RavenBeast_ColonyDashSky_Move5, M_RavenBeast_ColonyDashSky_Move7, M_RavenBeast_FlyRoutine1_Move2".Contains(row.Name.Value.ToString()))
            {
                ((EnumPropertyData)row["MoveType"]).Value = FName.FromString(asset, "MoveTransformType_None");
                ((EnumPropertyData)row["PositionType"]).Value = FName.FromString(asset, "MovePositionType_Self");
                ((FloatPropertyData)row["ForwardValue"]).Value = 0.0F;
                ((FloatPropertyData)row["RightValue"]).Value = 0.0F;
                ((FloatPropertyData)row["UpValue"]).Value = 0.0F;
            }

            // Replace M_Crawler map center warp with relative backwards warp
            if(row.Name.Value.ToString() == "M_Crawler_WarpMapCenter_Move1")
            {
                ((EnumPropertyData)row["MoveType"]).Value = FName.FromString(asset, "MoveTransformType_Static");
                ((EnumPropertyData)row["PositionType"]).Value = FName.FromString(asset, "MovePositionType_Target");
                ((FloatPropertyData)row["ForwardValue"]).Value = -2500.0F;
                ((FloatPropertyData)row["RightValue"]).Value = 0.0F;
                ((FloatPropertyData)row["UpValue"]).Value = 0.0F;
            }
        }
    }

    static void ModifyZoneEvents(UAsset asset)
    {
        Log("Modifying zoneEvents...");
        DataTableExport table = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> zoneEvents = table.Table.Data;
        
        foreach(StructPropertyData row in zoneEvents)
        {
            // Replace Mann phase 2 cutscene with the next event in the chain to prevent being teleported out of bounds
            if(row.Name.Value.ToString() == "Xion_Boss_Mann_E_ActorEff_006")
            {
                ArrayPropertyData finishEventsArray = (ArrayPropertyData)row["FinishEvents"];
                finishEventsArray.Value = finishEventsArray.Value.Append(new NamePropertyData { Value = FName.FromString(asset, "Xion_Boss_Mann_E_ActorEff_008")}).ToArray();

                ArrayPropertyData addEventsArray = (ArrayPropertyData)row["AddEvents"];
                addEventsArray.Value[0].IsZero = true;
                addEventsArray.Value = addEventsArray.Value.Where(val => !val.ToString().Contains("Xion_Boss_Mann_E_Theater_002")).ToArray();
            }
        }
    }

    static void ModifyAI(UAsset aiAsset)
    {
        Log("Modifying Tachy AI...");
        List<Export> exports = aiAsset.Exports;

        foreach(NormalExport exp in exports)
        {
            ArrayPropertyData skillName = (ArrayPropertyData)exp["SkillName"];
            if(skillName == null) continue;
            foreach(StrPropertyData skill in skillName.Value)
            {
                // Replace Tachy skills M_Tachy_BlinkStageMiddle1 and M_Tachy_BlinkStageMiddle2 to jump backwards instead of teleporting out of bounds
                if(skill.Value.ToString().Contains("M_Tachy_BlinkStageMiddle")) skill.Value = FString.FromString("M_Tachy_MoveBackFar", Encoding.UTF8);
            }
        }
    }

    static void Check(UAsset asset)
    {
        DataTableExport dtExport = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> entries = dtExport.Table.Data;

        foreach(StructPropertyData row in entries)
        {
            EnumPropertyData en = (EnumPropertyData)row["Action1"];
            if(en != null && en.Value.ToString() == "EffectAction_SummonActor"){
                //Log($"{row.Name.Value}");
            }
            /*
            NamePropertyData rank = (NamePropertyData)row["Rank"];
            IntPropertyData maxHP = (IntPropertyData)row["MaxHP"];
            FloatPropertyData physicAttackPower = (FloatPropertyData)row["PhysicAttackPower"];
            FloatPropertyData shieldAttackPower = (FloatPropertyData)row["ShieldAttackPower"];
            if(rank.Value != null && row.Name.Value.ToString().Contains("N_")) {
                Log($"{row.Name.Value} ({rank.Value})");
            };
            */
        }
    }

    static void ReplaceSummonEffect(UAsset charactersAsset, UAsset spawnEventsAsset, UAsset levelTargetFiltersAsset, UAsset eventActorEffectsAsset, UAsset conditionsAsset, UAsset effectsAsset, List<StructPropertyData> characters, StructPropertyData spawnEvent, Random rndCategory, Random rndAlias, string zoneName, ArrayPropertyData characterEffectArray)
    {
        DataTableExport table = (DataTableExport)effectsAsset.Exports[0];
        List<StructPropertyData> effects = table.Table.Data;

        foreach(NamePropertyData effect in characterEffectArray.Value)
        {
            string effectName = effect.Value.ToString();
            if(!effectName.Contains("Summon")) continue;
            if(!RelevantSummonEffectIndices.ContainsKey(effectName)) continue;

            string newEffectName = $"{incrementalEffectID}_{effectName}";
            NamePropertyData newArrayEffect = (NamePropertyData)effect.Clone();
            newArrayEffect.Value = FName.FromString(charactersAsset, newEffectName);

            // Clone original summon effect
            StructPropertyData originalEffect = effects[RelevantSummonEffectIndices[effectName]];
            StructPropertyData newEffect = (StructPropertyData)originalEffect.Clone();
            newEffect.Name = FName.FromString(effectsAsset, newEffectName);
            ((UInt32PropertyData)newEffect["ID"]).Value = incrementalEffectID;

            // Randomize the summoned enemy
            StrPropertyData actionValue1 = (StrPropertyData)newEffect["ActionValue1"];
            if(actionValue1.Value == null) continue;
            string actionValueStr = actionValue1.Value.ToString();
            EnemyRank rank = effectName.Contains("Skulling_Summon") ? EnemyRank.Normal : EnemyRank.Animal;
            string previousAlias = Regex.Replace(actionValueStr.Split("{\"CharacterAlias\":\"")[1], "\".*", $"");
            string replacementAlias = PickDiffCategoryEnemy(previousAlias, rank, rndCategory, rndAlias);
            string newEnemyName = ModifyEnemy(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, previousAlias, replacementAlias, characters, spawnEvent, rndCategory, rndAlias, zoneName);

            string newAction1Value = actionValueStr.Replace(previousAlias, newEnemyName);
            actionValue1.Value = FString.FromString(newAction1Value, Encoding.UTF8);
            effects.Add(newEffect);
            incrementalEffectID++;
        }
    }

    static void FindAndModifyEffects(UAsset charactersAsset, UAsset spawnEventsAsset, UAsset levelTargetFiltersAsset, UAsset eventActorEffectsAsset, UAsset conditionsAsset, UAsset effectsAsset, Random rndCategory, Random rndAlias)
    {
        Log("Parsing summon effects...");
        DataTableExport charactersTable = (DataTableExport)charactersAsset.Exports[0];
        List<StructPropertyData> characters = charactersTable.Table.Data;
        DataTableExport table = (DataTableExport)effectsAsset.Exports[0];
        List<StructPropertyData> effects = table.Table.Data;

        int x = -1;
        foreach(StructPropertyData row in effects)
        {
            // Save the index of relevant summon effects for later so we don't have to loop through all effects countless times
            x++;
            string effectName = row.Name.Value.ToString();
            if(!effectName.Contains("Summon")) continue;
            if(RelevantSummonEffectIndices.ContainsKey(effectName)) RelevantSummonEffectIndices[effectName] = x;

            // Randomize Maelstrom summons (consistent)
            if(!"M_Maelstrom_SummonSingle, M_Maelstrom_SummonSingle_AYL, M_Maelstrom_SummonProjectile_Summon, M_Maelstrom_SummonProjectile2_Summon".Contains(effectName)) continue;
            string zoneName = "Zone_ATL_03_MaelstromSummon";
            if(effectName.Contains("AYL") || effectName.Contains("Projectile2")) zoneName = "Zone_AYL_06_MaelstromSummon";
            StrPropertyData actionValue1 = (StrPropertyData)row["ActionValue1"];
            string actionValueStr = actionValue1.Value.ToString();
            string previousAlias = Regex.Replace(actionValueStr.Split("{\"CharacterAlias\":\"")[1], "\".*", $"");
            string replacementAlias = PickDiffCategoryEnemy(previousAlias, EnemyRank.Animal, rndCategory, rndAlias);
            // Knowlingly pass the effect instead of a spawnEvent since we don't have access to one here. Only the name is used in the function anyway so it doesn't matter
            string newEnemyName = ModifyEnemy(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, eventActorEffectsAsset, conditionsAsset, effectsAsset, previousAlias, replacementAlias, characters, row, rndCategory, rndAlias, zoneName);
            string newAction1Value = actionValueStr.Replace(previousAlias, newEnemyName);
            actionValue1.Value = FString.FromString(newAction1Value, Encoding.UTF8);
        }
    }

    static string PickDiffCategoryEnemy(string originalName, EnemyRank rank, Random rndCategory, Random rndAlias)
    {
        // Pick a different enemy category
        string replacementCategory = originalName;
        while(originalName.Contains(replacementCategory)){
            replacementCategory = EnemyCategoriesToPlace[rank][rndCategory.Next(EnemyCategoriesToPlace[rank].Length)];
        }

        // Pick a random enemy from the category
        List<string> tempEnemyList = [];
        foreach(string enemyAlias in EnemiesToPlace[rank]){
            if(enemyAlias.Contains(replacementCategory)) tempEnemyList.Add(enemyAlias);
        }
        int index = rndAlias.Next(tempEnemyList.Count);
        return tempEnemyList[index];
    }

    static void ShuffleNPCAppearances(UAsset asset)
    {
        DataTableExport dtExport = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> entries = dtExport.Table.Data;
        Random rndAppearance = new();

        foreach(StructPropertyData row in entries){
            NamePropertyData refAppearance = (NamePropertyData)row["RefAppearance"];
            string apr = refAppearance.Value.ToString();
            // Avoid replacing the Drone, Roxanne and Raven as they break parts of the game or have no effect
            if(refAppearance.Value != null && apr.StartsWith("N_") && !apr.Contains("Drone") && !apr.Contains("Roxa") && !apr.Contains("Raven")) {
                string replacementAppearance = EnemyAppearances[rndAppearance.Next(EnemyAppearances.Count)];
                refAppearance.Value = FName.FromString(asset, replacementAppearance);
                //NamePropertyData defaultStanceAlias = (NamePropertyData)row["DefaultStanceAlias"];
                //defaultStanceAlias.Value = FName.FromString(asset, replacementAppearance + "_Default");
                //Log($"{row.Name.Value}: {refAppearance.Value} => {replacementAppearance}");
            };
        }
    }
}