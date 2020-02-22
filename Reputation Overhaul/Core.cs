using System;
using System.Collections;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;
using static Reputation_Overhaul.Logger;
using BattleTech;

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
            // PrintObjectFields(Settings, "Settings");
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
}

