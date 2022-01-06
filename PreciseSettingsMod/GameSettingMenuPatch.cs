using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace PreciseSettingsMod
{
    [HarmonyPatch(typeof(GameSettingMenu))]
    class GameSettingMenuPatch
    {
        public static ManualLogSource log;



        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class GameSettingMenuStartPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch]
            public static void PostFix(GameSettingMenu __instance)
            {
                if (log == null) log = PreciseSettingsMod.log;


                Transform listOfSettingsTranform = __instance.transform;
                listOfSettingsTranform = listOfSettingsTranform.FindChild("Game Settings");
                listOfSettingsTranform = listOfSettingsTranform.FindChild("GameGroup");
                listOfSettingsTranform = listOfSettingsTranform.FindChild("SliderInner");
                
                log.LogMessage("------------------------------ listofsettingtransform found");
                

                SetupSettingInt(ref listOfSettingsTranform, "EmergencyCooldown");
                SetupSettingInt(ref listOfSettingsTranform, "DiscussionTime");
                SetupSettingInt(ref listOfSettingsTranform, "VotingTime");

                SetupSettingFloat(ref listOfSettingsTranform, "PlayerSpeed");
                SetupSettingFloat(ref listOfSettingsTranform, "CrewmateVision");
                SetupSettingFloat(ref listOfSettingsTranform, "ImpostorVision");
                SetupSettingFloat(ref listOfSettingsTranform, "KillCooldown");
            }

            /// <summary>
            /// Replace the text of a given parameter to a textbox and the increment to 1
            /// </summary>
            /// <param name="listOfSettingsTranform"></param>
            /// <param name="paramName"></param>
            private static void SetupSettingInt(ref Transform listOfSettingsTranform, string paramName)
            {
                Transform paramLineTransform = listOfSettingsTranform.FindChild(paramName);
                Transform childTextTransform = paramLineTransform.FindChild("Value_TMP");

                paramLineTransform.GetComponent<NumberOption>().Increment = 1;

                if (MatchMakerPatch.inputText != null)
                {
                    var inputText = GameObject.Instantiate(MatchMakerPatch.inputText, paramLineTransform);
                    inputText.SetActive(true);
                    inputText.transform.localPosition = new Vector3(childTextTransform.GetComponent<RectTransform>().anchoredPosition.x, 0, 0);
                    TextBoxTMP textBox = inputText.GetComponent<TextBoxTMP>();
                    textBox.outputText = childTextTransform.GetComponent<TextMeshPro>();

                    ButtonClickedEvent focusLostEvent = new ButtonClickedEvent();
                    focusLostEvent.AddListener(new Action(() =>
                    {
                        NumberOption option = inputText.transform.parent.GetComponent<NumberOption>();
                        string suffix = "";
                        switch (option.SuffixType)
                        {
                            case NumberSuffixes.None:
                                suffix = "";
                                break;
                            case NumberSuffixes.Multiplier:
                                suffix = "x";
                                break;
                            case NumberSuffixes.Seconds:
                                suffix = "s";
                                break;
                        }

                        if (int.TryParse(textBox.text, out int result))
                        {
                            if (result >= option.ValidRange.max)
                            {
                                option.Value = result - 1;
                                option.Increase();
                            }
                            else
                            {
                                option.Value = result + 1;
                                option.Decrease();
                            }
                        }
                        textBox.outputText.text = option.GetInt().ToString() + suffix;
                    }));

                    textBox.OnFocusLost = focusLostEvent;
                    textBox.OnEnter = focusLostEvent;
                    log.LogMessage("------------------------------ instantiate copy of input text for parameter " + paramName);
                }
                else
                {
                    log.LogMessage("------------------------------ copy of input text = NULL for parameter " + paramName);
                }
            }

            /// <summary>
            /// Replace the text of a given parameter to a textbox and the increment to 0.05
            /// </summary>
            /// <param name="listOfSettingsTranform"></param>
            /// <param name="paramName"></param>
            private static void SetupSettingFloat(ref Transform listOfSettingsTranform, string paramName)
            {
                Transform paramLineTransform = listOfSettingsTranform.FindChild(paramName);
                Transform childTextTransform = paramLineTransform.FindChild("Value_TMP");

                paramLineTransform.GetComponent<NumberOption>().Increment = 0.05f;

                if (MatchMakerPatch.inputText != null)
                {
                    var inputText = GameObject.Instantiate(MatchMakerPatch.inputText, paramLineTransform);
                    inputText.SetActive(true);
                    inputText.transform.localPosition = new Vector3(childTextTransform.GetComponent<RectTransform>().anchoredPosition.x, 0, 0);
                    TextBoxTMP textBox = inputText.GetComponent<TextBoxTMP>();
                    textBox.outputText = childTextTransform.GetComponent<TextMeshPro>();

                    ButtonClickedEvent focusLostEvent = new ButtonClickedEvent();
                    focusLostEvent.AddListener(new Action(() =>
                    {
                        NumberOption option = inputText.transform.parent.GetComponent<NumberOption>();
                        string suffix = "";
                        switch (option.SuffixType)
                        {
                            case NumberSuffixes.None:
                                suffix = "";
                                break;
                            case NumberSuffixes.Multiplier:
                                suffix = "x";
                                break;
                            case NumberSuffixes.Seconds:
                                suffix = "s";
                                break;
                        }


                        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
                        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

                        if (float.TryParse(textBox.text, style, culture, out float result))
                        {
                            if (result >= option.ValidRange.max)
                            {
                                option.Value = result - 0.05f;
                                option.Increase();
                            }
                            else
                            {
                                option.Value = result + 0.05f;
                                option.Decrease();
                            }
                        }
                        else
                        {
                            culture = CultureInfo.CreateSpecificCulture("fr-FR");
                            if (float.TryParse(textBox.text, style, culture, out result))
                            {
                                if (result >= option.ValidRange.max)
                                {
                                    option.Value = result - 0.05f;
                                    option.Increase();
                                }
                                else
                                {
                                    option.Value = result + 0.05f;
                                    option.Decrease();
                                }
                            }
                        }
                        textBox.outputText.text = option.GetFloat().ToString() + suffix;
                    }));

                    textBox.OnFocusLost = focusLostEvent;
                    textBox.OnEnter = focusLostEvent;
                    log.LogMessage("------------------------------ instantiate copy of input text for parameter " + paramName);
                }
                else
                {
                    log.LogMessage("------------------------------ copy of input text = NULL for parameter " + paramName);
                }
            }






        }
    }
}
