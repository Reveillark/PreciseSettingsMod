using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PreciseSettingsMod
{
    [BepInPlugin(Id, Name, Version)]
    [BepInProcess("Among Us.exe")]
    public class PreciseSettingsMod : BasePlugin
    {
        public const string Id = "fr.phenix.preciseSettings";
        public const string Name = "Precise Settings";
        public const string Version = "1.0.0";

        public Harmony Harmony { get; } = new Harmony(Id);
        public static ManualLogSource log;

        public static string pluginFolder;

        public static Object _prefab;




        public override void Load()
        {
            log = Log;

            log.LogMessage(Name + " Mod loaded");

            // Mandatory to comply to Innersloth mod policy
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, loadSceneMode) =>
            {
                ModManager.Instance.ShowModStamp();
            }));


            Harmony.PatchAll();
        }


        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionShowerPatch
        {
            public static void Postfix(VersionShower __instance)
            {
                //__instance.text.text += " + <color=#5E4CA6FF>TEST</color> by Phenix";
                __instance.text.text += "\n + <color=#5E4CA6FF> " + Name + " v" + Version + "</color> by Phenix";
            }
        }

    }
}
