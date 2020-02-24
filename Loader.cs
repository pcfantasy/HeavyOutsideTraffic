using ColossalFramework.UI;
using ICities;
using UnityEngine;
using System.IO;
using ColossalFramework;
using System.Reflection;
using System;
using System.Linq;
using ColossalFramework.Math;
using System.Collections.Generic;
using HeavyOutsideTraffic.Util;
using HeavyOutsideTraffic.CustomAI;

namespace HeavyOutsideTraffic
{
    public class Loader : LoadingExtensionBase
    {
        public static LoadMode CurrentLoadMode;
        public static UIPanel roadInfo;
        public static GameObject roadWindowGameObject;
        public static RoadUI guiPanel;
        public static bool isGuiRunning = false;
        public static bool HarmonyDetourInited = false;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Loader.CurrentLoadMode = mode;
            if (HeavyOutsideTraffic.IsEnabled)
            {
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                {
                    DebugLog.LogToFileOnly("OnLevelLoaded");
                    SetupRoadGui();
                    HarmonyInitDetour();
                    HeavyOutsideTraffic.LoadSetting();
                    if (mode == LoadMode.NewGame)
                    {
                        DebugLog.LogToFileOnly("New Game");
                    }
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (HeavyOutsideTraffic.IsEnabled)
            {
                if (CurrentLoadMode == LoadMode.LoadGame || CurrentLoadMode == LoadMode.NewGame)
                {
                    HarmonyRevertDetour();
                    if (isGuiRunning)
                    {
                        //remove RoadUI
                        if (guiPanel != null)
                        {
                            if (guiPanel.parent != null)
                            {
                                guiPanel.parent.eventVisibilityChanged -= roadInfo_eventVisibilityChanged;
                            }
                        }
                        if (roadWindowGameObject != null)
                        {
                            UnityEngine.Object.Destroy(roadWindowGameObject);
                        }
                    }
                }
            }

            HeavyOutsideTraffic.SaveSetting();
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }

        public static void SetupRoadGui()
        {
            roadWindowGameObject = new GameObject("roadWindowObject");
            guiPanel = (RoadUI)roadWindowGameObject.AddComponent(typeof(RoadUI));


            roadInfo = UIView.Find<UIPanel>("(Library) RoadWorldInfoPanel");
            if (roadInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) RoadWorldInfoPanel\nAvailable panels are:\n");
            }
            guiPanel.transform.parent = roadInfo.transform;
            guiPanel.size = new Vector3(roadInfo.size.x, roadInfo.size.y);
            guiPanel.baseBuildingWindow = roadInfo.gameObject.transform.GetComponentInChildren<RoadWorldInfoPanel>();
            guiPanel.position = new Vector3(roadInfo.size.x, roadInfo.size.y);
            roadInfo.eventVisibilityChanged += roadInfo_eventVisibilityChanged;
            Loader.isGuiRunning = true;
        }

        public static void roadInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            guiPanel.isEnabled = value;
            if (value)
            {
                Loader.guiPanel.transform.parent = Loader.roadInfo.transform;
                Loader.guiPanel.size = new Vector3(Loader.roadInfo.size.x, Loader.roadInfo.size.y);
                Loader.guiPanel.baseBuildingWindow = Loader.roadInfo.gameObject.transform.GetComponentInChildren<RoadWorldInfoPanel>();
                Loader.guiPanel.position = new Vector3(Loader.roadInfo.size.x, Loader.roadInfo.size.y);
                guiPanel.Show();
            }
            else
            {
                guiPanel.Hide();
            }
        }

        public void HarmonyInitDetour()
        {
            if (!HarmonyDetourInited)
            {
                DebugLog.LogToFileOnly("Init harmony detours");
                HarmonyDetours.Apply();
            }
        }

        public void HarmonyRevertDetour()
        {
            if (HarmonyDetourInited)
            {
                DebugLog.LogToFileOnly("Revert harmony detours");
                HarmonyDetours.DeApply();
            }
        }
    }
}

