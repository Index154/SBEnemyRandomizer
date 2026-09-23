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
using System.Text.Unicode;
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

        // Extract datatables from game files and convert to legacy format using retoc
        await RetocToLegacy(eventSpawnTable);
        await RetocToLegacy(characterTable);
        await RetocToLegacy(levelTargetFilterTable);
        await RetocToLegacy(skillActiveStepTable);
        await RetocToLegacy(characterMoveTable);
        await RetocToLegacy(tachyAI);
        
        // Load legacy uasset files and modify them
        UAsset spawnEventsAsset = ReadUAsset($"{unpackTablePath}/{eventSpawnTable}", mapPath);
        UAsset charactersAsset = ReadUAsset($"{unpackTablePath}/{characterTable}", mapPath);
        UAsset levelTargetFiltersAsset = ReadUAsset($"{unpackTablePath}/{levelTargetFilterTable}", mapPath);
        UAsset skillActiveStepsAsset = ReadUAsset($"{unpackTablePath}/{skillActiveStepTable}", mapPath);
        UAsset characterMovesAsset = ReadUAsset($"{unpackTablePath}/{characterMoveTable}", mapPath);
        UAsset tachyAIAsset = ReadUAsset($"{unpackAIPath}/{tachyAI}", mapPath);
        if(randomizeNPCAppearances) ShuffleNPCAppearances(charactersAsset);
        RandomizeSpawns(spawnEventsAsset, charactersAsset, levelTargetFiltersAsset);
        ModifyAI(tachyAIAsset);
        ModifyCharacterMoves(characterMovesAsset);
        ModifySkillActiveSteps(skillActiveStepsAsset);
        //CheckEnemies(charactersAsset);

        // Save modified tables
        spawnEventsAsset.Write($"{repackTablePath}/{eventSpawnTable}");
        charactersAsset.Write($"{repackTablePath}/{characterTable}");
        levelTargetFiltersAsset.Write($"{repackTablePath}/{levelTargetFilterTable}");
        skillActiveStepsAsset.Write($"{repackTablePath}/{skillActiveStepTable}");
        characterMovesAsset.Write($"{repackTablePath}/{characterMoveTable}");
        tachyAIAsset.Write($"{repackAIPath}/{tachyAI}");

        // Repack modified uassets and convert to game-ready zen format using retoc
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
        await RunRetoc($"to-legacy \"{gamePath}\" \"{tempPath}/unpacked\" -f \"{uAssetName}\"");
        Log("Game files unpacked");
    }

    async static Task RetocToZen()
    {
        string modName = "SBEnemyRandomizer_P";
        await RunRetoc($"to-zen \"{tempPath}/modified\" \"{tempPath}/{modName}.utoc\" --version UE4_26");

        File.Move($"{tempPath}/{modName}.pak", gamePath + $"/~mods/{modName}.pak", overwrite: true);
        File.Move($"{tempPath}/{modName}.ucas", gamePath + $"/~mods/{modName}.ucas", overwrite: true);
        File.Move($"{tempPath}/{modName}.utoc", gamePath + $"/~mods/{modName}.utoc", overwrite: true);
        Log("Mod files moved to game directory");
    }

    static UAsset ReadUAsset(string uAssetPath, string mapPath){
        return new UAsset(uAssetPath, EngineVersion.VER_UE4_26, new Usmap(mapPath));
    }

    static void RandomizeSpawns(UAsset spawnEventsAsset, UAsset charactersAsset, UAsset levelTargetFiltersAsset)
    {
        DataTableExport spawnEventsTable = (DataTableExport)spawnEventsAsset.Exports[0];
        List<StructPropertyData> spawnEvents = spawnEventsTable.Table.Data;
        DataTableExport charactersTable = (DataTableExport)charactersAsset.Exports[0];
        List<StructPropertyData> characters = charactersTable.Table.Data;

        uint incrementalID = 990000000;
        Dictionary<string, string> consistentReplacements = [];
        // For challenge mode bosses - Currently unused because the zones are commented out in GameData.cs
        Dictionary<EnemyRank, Dictionary<string, string[]>> challengeBossesToPlaceOnce = new(){
            [EnemyRank.Boss] = EnemiesToPlaceOnce[EnemyRank.Boss].ToDictionary(pair => pair.Key, pair => pair.Value.ToArray())
        };

        // Seeding logic
        Random seedGenerator = (seed == -1) ? new() : new(seed);
        int randoSeed = seedGenerator.Next(int.MinValue, int.MaxValue);
        Random rndCategory = new(randoSeed);
        Random rndAlias = new(randoSeed);

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

            string[] enemySourceArray = EnemiesToPlace[rank];
            bool onlyPlaceOnce = false;
            if(onlyShuffleExistingBoss && rank == EnemyRank.Boss) onlyPlaceOnce = true;

            string replacementAlias = "";
            if(consistentReplacements.ContainsKey(characterAlias.Value.ToString()))
            {
                replacementAlias = consistentReplacements[characterAlias.Value.ToString()];
            }
            else if(onlyPlaceOnce)
            {
                Dictionary<string, string[]> enemyShuffleDict = EnemiesToPlaceOnce[rank];
                if(rank == EnemyRank.Boss && zone.Value.ToString().Contains("_Boss_")) enemyShuffleDict = challengeBossesToPlaceOnce[EnemyRank.Boss];
                
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

                // Remove selected enemy from the list
                List<string> tempList = enemyShuffleDict[keyToRemoveFrom].ToList();
                tempList.Remove(replacementAlias);
                enemyShuffleDict[keyToRemoveFrom] = tempList.ToArray();
                if(enemyShuffleDict[keyToRemoveFrom].Length < 1) enemyShuffleDict.Remove(keyToRemoveFrom);

            }else{

                // Pick a different enemy category
                string replacementCategory = characterAlias.Value.ToString();
                while(characterAlias.Value.ToString().Contains(replacementCategory)){
                    replacementCategory = EnemyCategoriesToPlace[rank][rndCategory.Next(EnemyCategoriesToPlace[rank].Length)];
                }

                // Pick a random enemy from the category
                List<string> tempEnemyList = [];
                foreach(string enemyAlias in enemySourceArray){
                    if(enemyAlias.Contains(replacementCategory)) tempEnemyList.Add(enemyAlias);
                }
                int index = rndAlias.Next(tempEnemyList.Count);
                replacementAlias = tempEnemyList[index];
            }

            // Force specific enemy for testing
            if(forceReplacementEnemyName != "") replacementAlias = forceReplacementEnemyName;

            // These bosses have two spawn events each so prevent these from being randomized separately
            if(!consistentReplacements.ContainsKey(characterAlias.Value.ToString()) && (characterAlias.Value.ToString() == "NST_M_Raven_01" || characterAlias.Value.ToString() == "NST_M_ElderPhase1_01"))
            {
                consistentReplacements.Add(characterAlias.Value.ToString(), replacementAlias);
            }

            // Clone the replacement charactertable entry and modify it. This is necessary so we can have different scaling for each copy of an enemy. We could add some more checks to reduce some of the duplicate entries this produces but I don't think that's worth the effort
            StructPropertyData originalEnemyEntry = new();
            StructPropertyData replacementEnemyEntry = new();
            foreach(StructPropertyData ch in characters){
                if(ch.Name.Value.ToString() == characterAlias.Value.ToString()) originalEnemyEntry = ch;
                if(ch.Name.Value.ToString() == replacementAlias)  replacementEnemyEntry = ch;
            }
            StructPropertyData newEnemyEntry = (StructPropertyData)replacementEnemyEntry.Clone();
            string newEntryName = Regex.Replace(replacementAlias, @".*_M_", $"_M_{rank}_{incrementalID}_");
            newEntryName = zone.Value.ToString().Replace("Zone_", "") + newEntryName;

            ScaleAndFixEnemy(charactersAsset, spawnEventsAsset, levelTargetFiltersAsset, row, newEnemyEntry, originalEnemyEntry, incrementalID, row.Name.Value.ToString());
            incrementalID++;
            newEnemyEntry.Name = FName.FromString(charactersAsset, newEntryName);
            characters.Add(newEnemyEntry);

            // Modify the spawn event to spawn the new custom charactertable entry instead
            Log($"{incrementalID - 1} | {row.Name.Value}: {characterAlias.Value} => {replacementAlias}");
            characterAlias.Value = FName.FromString(spawnEventsAsset, newEntryName);
        }
        
        Log($"Seed = {randoSeed}");
    }

    static void ScaleAndFixEnemy(UAsset charactersAsset, UAsset spawnEventsAsset, UAsset levelTargetFiltersAsset, StructPropertyData spawnEvent, StructPropertyData newEnemy, StructPropertyData originalEnemy, uint incrementalID, string spawnEventName)
    {
        // Assign every new enemy a unique ID in case it matters. Also makes troubleshooting easier
        ((UInt32PropertyData)newEnemy["ID"]).Value = incrementalID;

        // Keep some of the data of the replaced enemy such as combat data and drop tables for balance reasons
        string[] dataToRetain = ["Rank", "MaxHP", "MaxShield", "MaxStamina", "PhysicAttackPower", "RangeAttackPower", "ShieldAttackPower", "StaminaAttackPower", "ShieldRegenPerSecond", "ShieldRegenPerSecondWhenBattle", "StaminaRegenPerSecond", "HPRegenPerSecond", "ShieldIgnorePercentage", "HitDefenseLevel", "RewardGroupAlias", "RewardSpawnBucketType", "RewardOverrideSaveType", "RewardFormationAssetPath", "TargetFilterRadius", "ProjectileTargetFilterRadius", "DefaultDetectAIAlias", "NarrowDetectAIAlias", "AIAuditorySenseRadius", "AIAuditorySenseDecibel", "AIAuditorySenseDuration"];
        foreach(string s in dataToRetain)
        {
            newEnemy[s].RawValue = originalEnemy[s].RawValue;
        }
        // Lower stats for testing
        if(lowerEnemyHPForTesting)
        {
            IntPropertyData maxHP = (IntPropertyData)newEnemy["MaxHP"];
            maxHP.Value = 6000;
        }
        
        // Reset spawn when loading save - Fix for testing. Enemies with SaveType Save will have their name and last position written into your save file. This prevents rerandomizing enemies mid-playthrough without softlocking the game (in many cases). Most enemies are spawned the moment you enter the zone so it's very unwieldy to test the game without ever having enemy positions saved
        if(resetAllEnemySpawnsOnLoad)
        {
            EnumPropertyData saveType = (EnumPropertyData)spawnEvent["SaveType"];
            if(saveType.Value.ToString() == "ESBZoneObjSaveType_Save") saveType.Value = FName.FromString(spawnEventsAsset, "ESBZoneObjSaveType_ResetZone");
        }

        // Conditional fixes for specific enemies
        // --------------------------------------------------------------------------------------
        ArrayPropertyData defaultEffectArray = (ArrayPropertyData)newEnemy["DefaultEffectArray"];
        string newEnemyName = newEnemy.Name.Value.ToString();
        // Remove immortality from certain bosses
        if("SD_M_HedgeBoarBrute_01, SE_M_Marionette_01, DED_M_Opener_01, NST_M_ElderPhase1_01, NST_M_Raven_01, NST_M_ExoSuit_01, WLA_M_RoyalGuardFemale_01, WLB_M_RoyalGuardFemale_01, SE_M_WeaponMasterA_01, UME_M_Tachy_01, DED_M_GorillaB_01, UME_M_SkullJuggernaut_01, SE_M_WeaponMasterB_01, SE_M_Crawler_01".Contains(newEnemyName))
        {
            defaultEffectArray.Value = defaultEffectArray.Value.Where(val => val.ToString() != "Passive_Immortal").ToArray();
        }
        // Remove special stance from certain bosses (would trigger problematic cutscenes when finishing them)
        if("UME_M_Tachy_01, DED_M_GorillaB_01, UME_M_SkullJuggernaut_01, SE_M_WeaponMasterB_01".Contains(newEnemyName))
        {
            ArrayPropertyData stanceAliasArray = (ArrayPropertyData)newEnemy["StanceAliasArray"];
            stanceAliasArray.Value = stanceAliasArray.Value.Where(val => !val.ToString().Contains("_Finish")).ToArray();
        }
        // Add Mann condition triggers and filters from his vanilla spawn event
        if(newEnemyName == "CHAL_XION_M_Mann_01")
        {
            DataTableExport levelTargetFiltersTable = (DataTableExport)levelTargetFiltersAsset.Exports[0];
            List<StructPropertyData> levelTargetFilters = levelTargetFiltersTable.Table.Data;
            foreach(StructPropertyData row in levelTargetFilters)
            {
                if(row.Name.Value.ToString() == "Xion_Boss_Mann_LevelTargetFilter_001")
                {
                    ((NamePropertyData)row["SpawnPointName"]).Value = FName.FromString(levelTargetFiltersAsset, spawnEventName);
                }
            }

            ((ArrayPropertyData)spawnEvent["EventOnSpawning"]).Value = [ new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_E_ObjectC_001")} ];
            ((ArrayPropertyData)spawnEvent["EventOnBattle"]).Value = [ new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_E_BlocVolC_001")} ];

            ((ArrayPropertyData)spawnEvent["ConditionsTrigger"]).Value = [ new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_Condition_001")}, new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_Condition_002")} ];
            ((ArrayPropertyData)spawnEvent["ConditionTriggerEvent"]).Value = [ new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_E_ActorEff_001")}, new NamePropertyData { Value = FName.FromString(spawnEventsAsset, "Xion_Boss_Mann_E_ActorEff_006")} ];

            ((ArrayPropertyData)spawnEvent["ConditionTriggerRunType"]).Value = [ new EnumPropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerRunType_Once")}, new EnumPropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerRunType_Once")} ];
            ((ArrayPropertyData)spawnEvent["ConditionTriggerExecType"]).Value = [ new EnumPropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerExecType_RunTime")}, new EnumPropertyData { Value = FName.FromString(spawnEventsAsset, "ESBConditionTriggerExecType_RunTime")} ];
        }
        // Enable TurretLaser enemies by default (still doesn't fix their AI)
        if(newEnemyName.Contains("TurretLaser"))
        {
            defaultEffectArray.Value = defaultEffectArray.Value.Where(val => val.ToString() != "BlockAI_Infinite").ToArray();
        }
        // Proof of concept for adding new array values. Currently unused
        if(spawnEventName == "BingBongBingBong")
        {
            defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, "M_GorillaB_Default")}).ToArray();
        }
        // "WindowBreakHydra". Most likely not actually broken but I'm keeping this here in case it comes up again
        if(spawnEventName == "DED10_E_CharS_037") {     // 990000050
            //newEnemy.RawValue = originalEnemy.RawValue;
        }
    }

    static void ModifyAI(UAsset aiAsset)
    {
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

    static void ModifySkillActiveSteps(UAsset asset)
    {
        DataTableExport skillActiveStepsTable = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> skillActiveSteps = skillActiveStepsTable.Table.Data;
        
        foreach(StructPropertyData row in skillActiveSteps)
        {
            ArrayPropertyData selfMoves = (ArrayPropertyData)row["SelfMoveAliasArray"];
            ArrayPropertyData targetMoves = (ArrayPropertyData)row["TargetMoveAliasArray"];

            // Remove RavenBeast world coordinate teleportation steps
            if(row.Name.Value.ToString().Contains("M_RavenBeast_"))
            {
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

            // Replace M_Crawler map center warp with middle-range backwards warp
            if(row.Name.Value.ToString() == "M_Crawler_WarpMapCenter_Move1")
            {
                ((EnumPropertyData)row["MoveType"]).Value = FName.FromString(asset, "MoveTransformType_Static");
                ((EnumPropertyData)row["PositionType"]).Value = FName.FromString(asset, "MovePositionType_Target");
                ((FloatPropertyData)row["ForwardValue"]).Value = -1500.0F;
                ((FloatPropertyData)row["RightValue"]).Value = 0.0F;
                ((FloatPropertyData)row["UpValue"]).Value = 0.0F;
            }
        }
    }

    static void CheckEnemies(UAsset asset)
    {
        DataTableExport dtExport = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> entries = dtExport.Table.Data;

        foreach(StructPropertyData row in entries){
            NamePropertyData rank = (NamePropertyData)row["Rank"];
            IntPropertyData maxHP = (IntPropertyData)row["MaxHP"];
            FloatPropertyData physicAttackPower = (FloatPropertyData)row["PhysicAttackPower"];
            FloatPropertyData shieldAttackPower = (FloatPropertyData)row["ShieldAttackPower"];
            if(rank.Value != null && row.Name.Value.ToString().Contains("N_")) {
                Log($"{row.Name.Value} ({rank.Value})");
            };
        }

        //asset.Write($"{repackTablePath}/{characterTable}");
    }

    static void ShuffleNPCAppearances(UAsset asset)
    {
        DataTableExport dtExport = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> entries = dtExport.Table.Data;
        Random rndAppearance = new();

        foreach(StructPropertyData row in entries){
            NamePropertyData refAppearance = (NamePropertyData)row["RefAppearance"];
            string apr = refAppearance.Value.ToString();
            if(refAppearance.Value != null && apr.StartsWith("N_") && !apr.Contains("Drone") && !apr.Contains("Roxa")) {
                string replacementAppearance = EnemyAppearances[rndAppearance.Next(EnemyAppearances.Count)];
                refAppearance.Value = FName.FromString(asset, replacementAppearance);
                //NamePropertyData defaultStanceAlias = (NamePropertyData)row["DefaultStanceAlias"];
                //defaultStanceAlias.Value = FName.FromString(asset, replacementAppearance + "_Default");
                //Log($"{row.Name.Value}: {refAppearance.Value} => {replacementAppearance}");
            };
        }
    }
}