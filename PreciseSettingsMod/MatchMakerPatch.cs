using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace PreciseSettingsMod
{
    [HarmonyPatch(typeof(CreateGameOptions))]
    class MatchMakerPatch
    {
        public static GameObject inputText;

        public static ManualLogSource log;

        [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Show))]
        public static class MatchMakerAwakePatch
        {
            /// <summary>
            /// Fetch the component to instantiate a textbox later on
            /// </summary>
            [HarmonyPostfix]
            [HarmonyPatch]
            public static void Prefix()
            {
                if (log == null) log = PreciseSettingsMod.log;
                log.LogMessage("------------------------------ fetching input text");

                if (inputText == null)
                {
                    inputText = Object.FindObjectsOfType<GameObject>(true).ToList().Find(o => o.name == "GameIdText");

                    inputText = Object.Instantiate(inputText);
                    Object.DontDestroyOnLoad(inputText);
                    inputText.SetActive(false);
                    inputText.GetComponent<BoxCollider2D>().size = new Vector2(0.85f, 0.5f);
                    TextBoxTMP textBox = inputText.GetComponent<TextBoxTMP>();
                    textBox.ClearOnFocus = true;
                    textBox.ForceUppercase = false;
                    textBox.AllowSymbols = true;
                    Object.Destroy(inputText.transform.GetChild(1).gameObject);
                    Object.Destroy(inputText.transform.GetChild(2).gameObject);
                }

                log.LogMessage("------------------------------ input text = " + inputText);
            }
        }
    }
}
