using System;
using System.Collections;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;
using static Reputation_Overhaul.Logger;
using BattleTech;
using UnityEngine;
using BattleTech.Framework;

namespace Reputation_Overhaul
{
    public static class Core
    {
        #region Init

        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("Reputation.Overhaul");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            // read settings
            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settings);
                Settings.modDirectory = modDir;
            }
            catch (Exception)
            {
                Settings = new ModSettings();
            }

            // blank the logfile
            Clear();
            PrintObjectFields(Settings, "Settings");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        // logs out all the settings and their values at runtime
        internal static void PrintObjectFields(object obj, string name)
        {
            LogDebug($"[START {name}]");

            var settingsFields = typeof(ModSettings)
                .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in settingsFields)
            {
                if (field.GetValue(obj) is IEnumerable &&
                    !(field.GetValue(obj) is string))
                {
                    LogDebug(field.Name);
                    foreach (var item in (IEnumerable)field.GetValue(obj))
                    {
                        LogDebug("\t" + item);
                    }
                }
                else
                {
                    LogDebug($"{field.Name,-30}: {field.GetValue(obj)}");
                }
            }

            LogDebug($"[END {name}]");
        }

        #endregion

        internal static ModSettings Settings;
    }


    //Increase Contract value based upon MRB Rating.
    [HarmonyPatch(typeof(SimGameState), "PrepContract")]
    public static class SGS_PrepContract_Patch
    {
        static void Postfix(SimGameState __instance, ref Contract contract)
        {
            var MRBRep = __instance.GetRawReputation(__instance.GetFactionDef("MercenaryReviewBoard").FactionValue);
            float initialCV = (float)contract.InitialContractValue;
            int newInitialContractValue = (int)(initialCV + initialCV * (MRBRep / __instance.Constants.Story.MRBRepMaxCap));
            Traverse.Create(contract).Property("InitialContractValue").SetValue(newInitialContractValue);
        }
    }

    //Give us complete control over what contracts can and cannot be taken.
    [HarmonyPatch(typeof(SimGameState), "ContractUserMeetsReputation_Career")]
    public static class SGS_ContractUserMeetsRep_Patch
    {
        static bool Prefix(SimGameState __instance, Contract c)
        {
            int num = Mathf.Min(c.Override.finalDifficulty + c.Override.difficultyUIModifier, (int)__instance.Constants.Story.GlobalContractDifficultyMax);
            int repLevel = __instance.GetCurrentMRBLevel();
            num -= repLevel;
            FactionValue teamFaction = c.GetTeamFaction("ecc8d4f2-74b4-465d-adf6-84445e5dfc230");
            if (!teamFaction.DoesGainReputation || c.Override.contractDisplayStyle == ContractDisplayStyle.BaseCampaignStory
            || c.Override.contractDisplayStyle == ContractDisplayStyle.BaseCampaignRestoration || c.Override.contractDisplayStyle == ContractDisplayStyle.BaseFlashpoint
            || c.Override.contractDisplayStyle == ContractDisplayStyle.HeavyMetalFlashpointCampaign)
            {
                return true;
            }
            switch (__instance.GetReputation(teamFaction))
            {
                case SimGameReputation.LOATHED:
                    return (float)num <= __instance.Constants.CareerMode.LoathedMaxContractDifficulty;
                case SimGameReputation.HATED:
                    return (float)num <= __instance.Constants.CareerMode.HatedMaxContractDifficulty;
                case SimGameReputation.DISLIKED:
                    return (float)num <= __instance.Constants.CareerMode.DislikedMaxContractDifficulty;
                case SimGameReputation.INDIFFERENT:
                    return (float)num <= __instance.Constants.CareerMode.IndifferentMaxContractDifficulty;
                case SimGameReputation.LIKED:
                    return (float)num <= __instance.Constants.CareerMode.LikedMaxContractDifficulty;
                case SimGameReputation.FRIENDLY:
                    return (float)num <= __instance.Constants.CareerMode.FriendlyMaxContractDifficulty;
                default:
                    return (float)num <= __instance.Constants.CareerMode.HonoredMaxContractDifficulty;
            }

            return false;
        }
    }

    //Blocks system shops from factions that are Hated.
    [HarmonyPatch(typeof(StarSystem), "CanUseSystemStore")]
    public static class StarSystem_CanUseSystemStore_Patch
    {
        static bool Prefix(StarSystem __instance)
        {
            FactionValue ownerValue = __instance.Def.OwnerValue;
            return __instance.Sim.GetReputation(ownerValue) > SimGameReputation.HATED;
        }
    }

    //More contracts available per system based upon MRB rating.
    [HarmonyPatch(typeof(StarSystem), "GetSystemMaxContracts")]
    public static class StarSystem_GetSystemMaxContracts_Patch
    {
        static void Prefix(StarSystem __instance)
        {
            __instance.Sim.Constants.Story.MaxContractsPerSystem = ModSettings.MaxContractsPerSystem;
        }
        static void Postfix(StarSystem __instance, ref int __result)
        {
            int repLevel = __instance.Sim.GetCurrentMRBLevel();
            __result += repLevel;
        }
    }
}

