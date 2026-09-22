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

namespace SBEnemyRandomizer;

public class Program{

    async static Task Main()
    {
        // Create folders
        Directory.CreateDirectory(unpackPath);
        Directory.CreateDirectory(repackPath);

        // Extract datatables from game files and convert to legacy format using retoc
        await RetocToLegacy(eventSpawnTable);
        await RetocToLegacy(characterTable);

        // Load legacy uasset files and modify them
        UAsset spawnEventsAsset = ReadUAsset($"{unpackPath}/{eventSpawnTable}", mapPath);
        UAsset charactersAsset = ReadUAsset($"{unpackPath}/{characterTable}", mapPath);
        if(randomizeNPCAppearances) ShuffleNPCAppearances(charactersAsset);
        RandomizeSpawns(spawnEventsAsset, charactersAsset);
        //CheckEnemies(charactersAsset);

        // Save modified tables
        spawnEventsAsset.Write($"{repackPath}/{eventSpawnTable}");
        charactersAsset.Write($"{repackPath}/{characterTable}");

        // Repack modified uassets and convert to game-ready zen format using retoc
        await RetocToZen();

        // Save file reset for testing
        File.Copy(Environment.ExpandEnvironmentVariables("%userprofile%/Downloads/StellarBladeSave03.sav"), Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/SB/Saved/SaveGames/76561198169967897/StellarBladeSave03.sav"), overwrite: true);
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
        if(File.Exists($"{unpackPath}/{uAssetName}"))
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

    static void RandomizeSpawns(UAsset spawnEventsAsset, UAsset charactersAsset)
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

            ScaleAndFixEnemy(charactersAsset, spawnEventsAsset, row, newEnemyEntry, originalEnemyEntry, incrementalID, row.Name.Value.ToString());
            incrementalID++;
            newEnemyEntry.Name = FName.FromString(charactersAsset, newEntryName);
            characters.Add(newEnemyEntry);

            // Modify the spawn event to spawn the new custom charactertable entry instead
            Log($"{incrementalID - 1} | {row.Name.Value}: {characterAlias.Value} => {replacementAlias}");
            characterAlias.Value = FName.FromString(spawnEventsAsset, newEntryName);
        }
        
        Log($"Seed = {randoSeed}");
    }

    static void ScaleAndFixEnemy(UAsset charactersAsset, UAsset spawnEventsAsset, StructPropertyData spawnEvent, StructPropertyData newEnemy, StructPropertyData originalEnemy, uint incrementalID, string spawnEventName)
    {
        // Assign every new enemy a unique ID in case it matters. Also makes troubleshooting easier
        ((UInt32PropertyData)newEnemy["ID"]).Value = incrementalID;

        // Keep some of the data of the replaced enemy such as combat data and drop tables for balance reasons
        string[] dataToRetain = ["Rank", "MaxHP", "MaxShield", "MaxStamina", "PhysicAttackPower", "RangeAttackPower", "ShieldAttackPower", "StaminaAttackPower", "ShieldRegenPerSecond", "ShieldRegenPerSecondWhenBattle", "StaminaRegenPerSecond", "HPRegenPerSecond", "ShieldIgnorePercentage", "DifficultyStatGroupAlias", "HitDefenseLevel", "RewardGroupAlias", "RewardSpawnBucketType", "RewardOverrideSaveType", "RewardFormationAssetPath", "TargetFilterRadius", "ProjectileTargetFilterRadius", "DefaultDetectAIAlias", "NarrowDetectAIAlias", "AIAuditorySenseRadius", "AIAuditorySenseDecibel", "AIAuditorySenseDuration"];
        foreach(string s in dataToRetain){
            newEnemy[s].RawValue = originalEnemy[s].RawValue;
        }
        // Lower stats for testing
        if(lowerEnemyHPForTesting){
            IntPropertyData maxHP = (IntPropertyData)newEnemy["MaxHP"];
            maxHP.Value = 3000;
        }
        
        // Reset spawn when loading save - Fix for testing. Enemies with SaveType Save will have their name and last position written into your save file. This prevents rerandomizing enemies mid-playthrough without softlocking the game (in many cases). Most enemies are spawned the moment you enter the zone so it's very unwieldy to test the game without ever having enemy positions saved
        if(resetAllEnemySpawnsOnLoad){
            EnumPropertyData saveType = (EnumPropertyData)spawnEvent["SaveType"];
            if(saveType.Value.ToString() == "ESBZoneObjSaveType_Save") saveType.Value = FName.FromString(spawnEventsAsset, "ESBZoneObjSaveType_ResetZone");
        }

        // Conditional fixes for specific enemies
        // --------------------------------------------------------------------------------------
        ArrayPropertyData defaultEffectArray = (ArrayPropertyData)newEnemy["DefaultEffectArray"];
        string newEnemyName = newEnemy.Name.Value.ToString();
        // Remove immortality from certain enemies
        if("SD_M_HedgeBoarBrute_01, SE_M_Marionette_01, DED_M_Opener_01, NST_M_ElderPhase1_01, NST_M_Raven_01, NST_M_ExoSuit_01, WLA_M_RoyalGuardFemale_01, WLB_M_RoyalGuardFemale_01, SE_M_WeaponMasterA_01".Contains(newEnemyName)){
            defaultEffectArray.Value = defaultEffectArray.Value.Where(val => val.ToString() != "Passive_Immortal").ToArray();
        }
        // Test Maelstrom
        if(newEnemyName.Contains("Maelstrom")){
            
        }
        // Proof of concept for adding new array values. Currently unused
        if(spawnEventName == "BingBongBingBong"){
            defaultEffectArray.Value = defaultEffectArray.Value.Append(new NamePropertyData { Value = FName.FromString(charactersAsset, "M_GorillaB_Default")}).ToArray();
        }
        // "WindowBreakHydra". Most likely not actually broken but I'm keeping this here in case it comes up again
        if(spawnEventName == "DED10_E_CharS_037") {     // 990000050
            //newEnemy.RawValue = originalEnemy.RawValue;
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

        //asset.Write($"{repackPath}/{characterTable}");
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