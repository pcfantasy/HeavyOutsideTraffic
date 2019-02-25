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

namespace HeavyOutsideTraffic
{
    public class Loader : LoadingExtensionBase
    {
        public static LoadMode CurrentLoadMode;

        public static UIPanel roadInfo;

        public static GameObject roadWindowGameObject;

        public static RoadUI guiPanel;

        public static bool isGuiRunning = false;

        public static bool isDetour = false;

        public static RedirectCallsState state1;

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
                    Detour();
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
                RevertDetour();
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

        public static void Detour()
        {
            if (HeavyOutsideTraffic.disableCollisionCheck && !isDetour)
            {
                if (!Check3rdPartyModLoaded("AdvancedJunctionRule", true))
                {
                    var srcMethod1 = typeof(CarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    var destMethod1 = typeof(CustomCarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    state1 = RedirectionHelper.RedirectCalls(srcMethod1, destMethod1);
                }
                else
                {
                    Assembly as1 = Assembly.Load("AdvancedJunctionRule");
                    var srcMethod1 = as1.GetType("AdvancedJunctionRule.NewCarAI").GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    var destMethod1 = typeof(CustomCarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    state1 = RedirectionHelper.RedirectCalls(srcMethod1, destMethod1);
                }
                isDetour = true;
            }
        }

        public static void RevertDetour()
        {
            if (isDetour)
            {
                if (!Check3rdPartyModLoaded("AdvancedJunctionRule", true))
                {
                    var srcMethod1 = typeof(CarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    RedirectionHelper.RevertRedirect(srcMethod1, state1);
                }
                else
                {
                    Assembly as1 = Assembly.Load("AdvancedJunctionRule");
                    var srcMethod1 = as1.GetType("AdvancedJunctionRule.NewCarAI").GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                    RedirectionHelper.RevertRedirect(srcMethod1, state1);
                }
                isDetour = false;
            }
        }

        public static bool Check3rdPartyModLoaded(string namespaceStr, bool printAll = false)
        {
            bool thirdPartyModLoaded = false;

            var loadingWrapperLoadingExtensionsField = typeof(LoadingWrapper).GetField("m_LoadingExtensions", BindingFlags.NonPublic | BindingFlags.Instance);
            List<ILoadingExtension> loadingExtensions = (List<ILoadingExtension>)loadingWrapperLoadingExtensionsField.GetValue(Singleton<LoadingManager>.instance.m_LoadingWrapper);

            if (loadingExtensions != null)
            {
                foreach (ILoadingExtension extension in loadingExtensions)
                {
                    if (printAll)
                        DebugLog.LogToFileOnly($"Detected extension: {extension.GetType().Name} in namespace {extension.GetType().Namespace}");
                    if (extension.GetType().Namespace == null)
                        continue;

                    var nsStr = extension.GetType().Namespace.ToString();
                    if (namespaceStr.Equals(nsStr))
                    {
                        DebugLog.LogToFileOnly($"The mod '{namespaceStr}' has been detected.");
                        thirdPartyModLoaded = true;
                        break;
                    }
                }
            }
            else
            {
                DebugLog.LogToFileOnly("Could not get loading extensions");
            }

            return thirdPartyModLoaded;
        }
    }
}

