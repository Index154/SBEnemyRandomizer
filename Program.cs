using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;
using UAssetAPI.Unversioned;
using System.Diagnostics;
using static SBEnemyRandomizer.src.GlobalSettings;
using static SBEnemyRandomizer.src.GameData;
using System.Text.RegularExpressions;

namespace SBEnemyRandomizer;

public class Program{

    async static Task Main(){
        // Extract datatables from game files and convert to legacy format using retoc
        await RetocToLegacy(eventSpawnTable);
        await RetocToLegacy(characterTable);

        // Load legacy uasset files and modify them
        UAsset charactersAsset = ReadUAsset(tempPath + $"/unpacked/{assetSubdirectory}/{characterTable}", mapPath);
        RandomizeSpawns(ReadUAsset(tempPath + $"/unpacked/{assetSubdirectory}/{eventSpawnTable}", mapPath), charactersAsset);
        //CheckEnemies(charactersAsset);

        // Repack modified uassets and convert to game-ready zen format using retoc
        await RetocToZen();
    }

    async static Task RetocToLegacy(string uAssetName){
        if(File.Exists($"{tempPath}/unpacked/{assetSubdirectory}/{uAssetName}")){
            Console.WriteLine($"Unpacked [{uAssetName}] already exists");
            return;
        }

        var psi = new ProcessStartInfo
        {
            FileName = retocPath,
            Arguments = $"to-legacy \"{gamePath}\" \"{tempPath}/unpacked\" -f \"{uAssetName}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
    }

    async static Task RetocToZen(){
        string modName = "SBEnemyRandomizer_P";

        var psi = new ProcessStartInfo
        {
            FileName = retocPath,
            Arguments = $"to-zen \"{tempPath}/modified\" \"{tempPath}/{modName}.utoc\" --version UE4_26",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        Console.WriteLine(output);

        File.Move($"{tempPath}/{modName}.pak", gamePath + $"/~mods/{modName}.pak", overwrite: true);
        File.Move($"{tempPath}/{modName}.ucas", gamePath + $"/~mods/{modName}.ucas", overwrite: true);
        File.Move($"{tempPath}/{modName}.utoc", gamePath + $"/~mods/{modName}.utoc", overwrite: true);

        // Save file reset for testing
        //File.Copy(Environment.ExpandEnvironmentVariables("%userprofile%/Downloads/StellarBladeSave03.sav"), Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/SB/Saved/SaveGames/76561198169967897/StellarBladeSave03.sav"), overwrite: true);
    }

    static UAsset ReadUAsset(string uAssetPath, string mapPath){
        Usmap mappings = new Usmap(mapPath);
        UAsset asset = new UAsset(uAssetPath, EngineVersion.VER_UE4_26, mappings);
        return asset;
    }

    static void RandomizeSpawns(UAsset spawnEventsAsset, UAsset charactersAsset){

        DataTableExport spawnEventsTable = (DataTableExport)spawnEventsAsset.Exports[0];
        List<StructPropertyData> spawnEvents = spawnEventsTable.Table.Data;
        DataTableExport charactersTable = (DataTableExport)charactersAsset.Exports[0];
        List<StructPropertyData> characters = charactersTable.Table.Data;

        uint incrementalID = 990000000;
        Dictionary<string, string> consistentReplacements = [];

        foreach(StructPropertyData row in spawnEvents){

            ArrayPropertyData characterAliasArray = (ArrayPropertyData)row["CharacterAlias"];
            if(characterAliasArray.Value == null || characterAliasArray.Value.Length < 1) continue;
            NamePropertyData characterAlias = (NamePropertyData)characterAliasArray.Value[0];

            // Ignore spawn events in irrelevant zones
            NamePropertyData zone = (NamePropertyData)row["Zone"];
            if(zone.Value == null || RelevantZones.IndexOf(zone.Value.ToString()) == -1) continue;
            // Testing for specific zone
            //if(!zone.Value.ToString().Contains("DED")) continue;

            // Determine enemy rank
            EnemyRank rank;
            if(EnemiesToReplace[EnemyRank.Animal].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Animal;
            else if(EnemiesToReplace[EnemyRank.Normal].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Normal;
            else if(EnemiesToReplace[EnemyRank.Boss].Any(characterAlias.Value.ToString().Contains)) rank = EnemyRank.Boss;
            else continue;
            //if(rank != EnemyRank.Boss) continue;

            string[] enemySourceArray = EnemiesToPlace[rank];
            Dictionary<string, string[]> enemySourceDict = [];
            bool onlyPlaceOnce = false;
            if(onlyShuffleExistingBoss && rank == EnemyRank.Boss) onlyPlaceOnce = true;

            string replacementAlias = "";
            if(consistentReplacements.ContainsKey(characterAlias.Value.ToString())){
                replacementAlias = consistentReplacements[characterAlias.Value.ToString()];
            }else if(onlyPlaceOnce){
                
                Random rndCategory = new();
                Random rndAlias = new();
                string keyToRemoveFrom = "";
                while(replacementAlias == ""){
                    KeyValuePair<string, string[]> randomKVP = EnemiesToPlaceOnce[rank].ElementAt(rndCategory.Next(EnemiesToPlaceOnce[rank].Count));
                    string[] replacementCategory = randomKVP.Value;
                    if(!characterAlias.Value.ToString().Contains(randomKVP.Key)){
                        keyToRemoveFrom = randomKVP.Key;
                        replacementAlias = replacementCategory[rndAlias.Next(replacementCategory.Length)];
                    }
                }

                // Remove selected enemy from the list
                List<string> tempList = EnemiesToPlaceOnce[rank][keyToRemoveFrom].ToList();
                tempList.Remove(replacementAlias);
                EnemiesToPlaceOnce[rank][keyToRemoveFrom] = tempList.ToArray();
                if(EnemiesToPlaceOnce[rank][keyToRemoveFrom].Length < 1){
                    EnemiesToPlaceOnce[rank].Remove(keyToRemoveFrom);
                }

            }else{

                // Pick a different enemy category
                Random rndCategory = new();
                string replacementCategory = characterAlias.Value.ToString();
                while(characterAlias.Value.ToString().Contains(replacementCategory)){
                    replacementCategory = EnemyCategoriesToPlace[rank][rndCategory.Next(EnemyCategoriesToPlace[rank].Length)];
                }

                // Pick a random enemy from the category
                Random rndAlias = new();
                List<string> tempEnemyList = [];
                foreach(string enemyAlias in enemySourceArray){
                    if(enemyAlias.Contains(replacementCategory)){
                        tempEnemyList.Add(enemyAlias);
                    }
                }
                int index = rndAlias.Next(tempEnemyList.Count);
                replacementAlias = tempEnemyList[index];

            }

            // Test specific enemy
            //replacementAlias = "UME_M_SkullGunner_01";

            // These bosses have two spawn events each so prevent these from being randomized separately
            if(!consistentReplacements.ContainsKey(characterAlias.Value.ToString()) && (characterAlias.Value.ToString() == "NST_M_Raven_01" || characterAlias.Value.ToString() == "NST_M_ElderPhase1_01")) {
                consistentReplacements.Add(characterAlias.Value.ToString(), replacementAlias);
            }

            // Clone the target character row and replace the stats
            StructPropertyData originalEnemyEntry = new();
            StructPropertyData replacementEnemyEntry = new();
            foreach(StructPropertyData ch in characters){
                if(ch.Name.Value.ToString() == characterAlias.Value.ToString()) {
                    originalEnemyEntry = ch;
                }
                if(ch.Name.Value.ToString() == replacementAlias) {
                    replacementEnemyEntry = ch;
                }
            }
            StructPropertyData newEnemyEntry = (StructPropertyData)replacementEnemyEntry.Clone();
            string newEntryName = Regex.Replace(replacementAlias, @".*_M_", $"_M_{rank}_{incrementalID}_");
            newEntryName = zone.Value.ToString().Replace("Zone_", "") + newEntryName;

            if(!characters.Any(ch => ch.Name.Value.ToString() == newEntryName)){
                ScaleAndFixEnemy(newEnemyEntry, originalEnemyEntry, incrementalID, row.Name.Value.ToString());
                incrementalID++;
                newEnemyEntry.Name = FName.FromString(charactersAsset, newEntryName);
                characters.Add(newEnemyEntry);
            }

            // Replace the enemy spawn
            Console.WriteLine($"{incrementalID - 1} | {row.Name.Value}: {characterAlias.Value} => {replacementAlias} / {newEntryName}");
            characterAlias.Value = FName.FromString(spawnEventsAsset, newEntryName);
            
        }

        spawnEventsAsset.Write(tempPath + $"/modified/{assetSubdirectory}/{eventSpawnTable}");
        charactersAsset.Write(tempPath + $"/modified/{assetSubdirectory}/{characterTable}");
    }

    static void ScaleAndFixEnemy(StructPropertyData newEnemy, StructPropertyData originalEnemy, uint incrementalID, string spawnEventName){

        ((UInt32PropertyData)newEnemy["ID"]).Value = incrementalID;

        // Keep some of the data of the replaced enemy such as combat data and drop tables for balance reasons
        string[] dataToRetain = ["Rank", "MaxHP", "MaxShield", "MaxStamina", "PhysicAttackPower", "RangeAttackPower", "ShieldAttackPower", "StaminaAttackPower", "ShieldRegenPerSecond", "ShieldRegenPerSecondWhenBattle", "StaminaRegenPerSecond", "HPRegenPerSecond", "ShieldIgnorePercentage", "DifficultyStatGroupAlias", "HitDefenseLevel", "RewardGroupAlias", "RewardSpawnBucketType", "RewardOverrideSaveType", "RewardFormationAssetPath"];
        foreach(string s in dataToRetain){
            newEnemy[s].RawValue = originalEnemy[s].RawValue;
        }

        // Conditional fixes for specific enemies
        // --------------------------------------------------------------------------------------
        // Tutorial Hedgeboar Brute is unkillable
        if(newEnemy.Name.Value.ToString() == "SD_M_HedgeBoarBrute_01"){
            ArrayPropertyData defaultEffectArray = (ArrayPropertyData)newEnemy["DefaultEffectArray"];
            defaultEffectArray.Value = (PropertyData[])defaultEffectArray.Value.Where(val => val.ToString() != "Passive_Immortal").ToArray();
        }
        // "WindowBreakHydra" replacement in DED10 is prone to breaking for yet unknown reasons. Make a backup of your save file before putting in the first fusion cell!!! Triggering the spawn event autosaves the game and if the spawn happens to fail then your save is bricked forever because the game never places the enemy correctly again even if you undo the replacement!
        if(spawnEventName == "DED10_E_CharS_037") {     // 990000050
            newEnemy.RawValue = originalEnemy.RawValue;
        }
    }

    static void CheckEnemies(UAsset asset){

        DataTableExport dtExport = (DataTableExport)asset.Exports[0];
        List<StructPropertyData> entries = dtExport.Table.Data;

        foreach(StructPropertyData row in entries){
            NamePropertyData rank = (NamePropertyData)row["Rank"];
            IntPropertyData maxHP = (IntPropertyData)row["MaxHP"];
            FloatPropertyData physicAttackPower = (FloatPropertyData)row["PhysicAttackPower"];
            FloatPropertyData shieldAttackPower = (FloatPropertyData)row["ShieldAttackPower"];
            if(rank.Value != null && row.Name.Value.ToString().Contains("DED")) {
                Console.WriteLine($"{row.Name.Value} ({rank.Value}): {maxHP.Value}, {physicAttackPower.Value}, {shieldAttackPower}");
            };
        }

        //asset.Write(tempPath + $"/modified/{assetSubdirectory}/{characterTable}");
    }
}