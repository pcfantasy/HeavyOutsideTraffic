using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeavyOutsideTraffic
{
    public class HeavyOutsideTrafficThreading : ThreadingExtensionBase
    {
        public override void OnAfterSimulationFrame()
        {
            base.OnAfterSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (HeavyOutsideTraffic.IsEnabled)
                {
                    uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                    int num4 = (int)(currentFrameIndex & 255u);
                    if (num4 == 255)
                    {
                        RoadUI.refeshOnce = true;
                    }
                }
            }
        }
    }
}
