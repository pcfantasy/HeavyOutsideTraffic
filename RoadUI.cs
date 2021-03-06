﻿using ColossalFramework;
using ColossalFramework.UI;
using HeavyOutsideTraffic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeavyOutsideTraffic
{
    public class RoadUI : UIPanel
    {
        public static readonly string cacheName = "RoadUI";

        private static readonly float SPACING = 15f;

        public RoadWorldInfoPanel baseBuildingWindow;

        public static bool refeshOnce = false;

        public static ushort lastSegment = 0;

        //1、citizen tax income
        private UILabel Type;
        //private UILabel alivevisitcount;

        public override void Update()
        {
            this.RefreshDisplayData();
            base.Update();
        }

        public override void Awake()
        {
            base.Awake();
            this.DoOnStartup();
        }

        public override void Start()
        {
            base.Start();
            this.canFocus = true;
            this.isInteractive = true;
            base.isVisible = true;
            this.BringToFront();
            base.opacity = 1f;
            base.cachedName = cacheName;
            this.RefreshDisplayData();
            base.Hide();
        }

        private void DoOnStartup()
        {
            this.ShowOnGui();
            base.Hide();
        }


        private void ShowOnGui()
        {
            this.Type = base.AddUIComponent<UILabel>();
            this.Type.text = Localization.Get("TYPE");
            this.Type.relativePosition = new Vector3(SPACING, 20f);
            this.Type.autoSize = true;
        }

        private void RefreshDisplayData()
        {
            if (refeshOnce || (lastSegment != WorldInfoPanel.GetCurrentInstanceID().NetSegment))
            {
                lastSegment = WorldInfoPanel.GetCurrentInstanceID().NetSegment;
                float x = Singleton<NetManager>.instance.m_segments.m_buffer[(int)lastSegment].m_middlePosition.x;
                float z = Singleton<NetManager>.instance.m_segments.m_buffer[(int)lastSegment].m_middlePosition.z;

                if (x > -8000 && x < 8000 && z > -8000 && z < 8000)
                {
                    this.Hide();
                }
                else
                {
                    if (base.isVisible)
                    {
                        if (x > 8000)
                        {
                            this.Type.text = Localization.Get("TYPE") + ": " + Localization.Get("A_OUTSIDE");
                        }
                        else if (z > 8000)
                        {
                            this.Type.text = Localization.Get("TYPE") + ": " + Localization.Get("B_OUTSIDE");
                        }
                        else if (x < -8000)
                        {
                            this.Type.text = Localization.Get("TYPE") + ": " + Localization.Get("C_OUTSIDE");
                        }
                        else if (z < -8000)
                        {
                            this.Type.text = Localization.Get("TYPE") + ": " + Localization.Get("D_OUTSIDE");
                        }
                        else
                        {
                            this.Type.text = "";
                        }

                        refeshOnce = false;
                        this.BringToFront();
                    }
                    else
                    {
                        this.Hide();
                    }
                }
            }
        }
    }
}
