﻿using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using HeavyOutsideTraffic.Util;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeavyOutsideTraffic
{
    public class HeavyOutsideTraffic : IUserMod
    {
        public static bool IsEnabled = false;
        public static int aTraffic = 0;
        public static int bTraffic = 0;
        public static int cTraffic = 0;
        public static int dTraffic = 0;
        public static bool disableCollisionCheck = false;
        public static int nonHighwayScale = 0;
        public static int languageIdex = 0;

        public string Name
        {
            get { return "Heavy Outside Traffic"; }
        }

        public string Description
        {
            get { return "Generate Heavy Outside Traffic to go through your city."; }
        }

        public void OnEnabled()
        {
            IsEnabled = true;
            FileStream fs = File.Create("HeavyOutsideTraffic.txt");
            fs.Close();
        }

        public void OnDisabled()
        {
            IsEnabled = false;
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadSetting();
            UIHelperBase group = helper.AddGroup(Localization.Get("TRAFFIC_SELECT"));
            group.AddDropdown(Localization.Get("SIDEA_TRAFFIC"), new string[] { Localization.Get("LOW_TRAFFIC"), Localization.Get("MEDIUM_TRAFFIC"), Localization.Get("HEAVY_TRAFFIC") }, aTraffic, (index) => GetEffortIdex(index));
            UIHelperBase group1 = helper.AddGroup(Localization.Get("TRAFFIC_SELECT"));
            group1.AddDropdown(Localization.Get("SIDEB_TRAFFIC"), new string[] { Localization.Get("LOW_TRAFFIC"), Localization.Get("MEDIUM_TRAFFIC"), Localization.Get("HEAVY_TRAFFIC") }, bTraffic, (index1) => GetEffortIdex1(index1));
            UIHelperBase group2 = helper.AddGroup(Localization.Get("TRAFFIC_SELECT"));
            group2.AddDropdown(Localization.Get("SIDEC_TRAFFIC"), new string[] { Localization.Get("LOW_TRAFFIC"), Localization.Get("MEDIUM_TRAFFIC"), Localization.Get("HEAVY_TRAFFIC") }, cTraffic, (index2) => GetEffortIdex2(index2));
            UIHelperBase group3 = helper.AddGroup(Localization.Get("TRAFFIC_SELECT"));
            group3.AddDropdown(Localization.Get("SIDED_TRAFFIC"), new string[] { Localization.Get("LOW_TRAFFIC"), Localization.Get("MEDIUM_TRAFFIC"), Localization.Get("HEAVY_TRAFFIC") }, dTraffic, (index3) => GetEffortIdex3(index3));
            UIHelperBase group4 = helper.AddGroup(Localization.Get("DISABLE_COLLISION_CHECK_DISCRIPTION"));
            group4.AddCheckbox(Localization.Get("DISABLE_COLLISION_CHECK_ENABLE"), disableCollisionCheck, (index) => IsDisableCollisionCheck(index));
        }

        public void IsDisableCollisionCheck(bool index)
        {
            disableCollisionCheck = index;
            SaveSetting();
        }

        public static void SaveSetting()
        {
            //save langugae
            FileStream fs = File.Create("HeavyOutsideTrafficSetting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(aTraffic);
            streamWriter.WriteLine(bTraffic);
            streamWriter.WriteLine(cTraffic);
            streamWriter.WriteLine(dTraffic);
            streamWriter.WriteLine(disableCollisionCheck);
            streamWriter.Flush();
            fs.Close();
        }

        public static void LoadSetting()
        {
            if (File.Exists("HeavyOutsideTrafficSetting.txt"))
            {
                FileStream fs = new FileStream("HeavyOutsideTrafficSetting.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = sr.ReadLine();

                if (strLine == "2")
                {
                    aTraffic = 2;
                }
                else if(strLine == "1")
                {
                    aTraffic = 1;
                }
                else
                {
                    aTraffic = 0;
                }

                strLine = sr.ReadLine();

                if (strLine == "2")
                {
                    bTraffic = 2;
                }
                else if (strLine == "1")
                {
                    bTraffic = 1;
                }
                else
                {
                    bTraffic = 0;
                }

                strLine = sr.ReadLine();

                if (strLine == "2")
                {
                    cTraffic = 2;
                }
                else if (strLine == "1")
                {
                    cTraffic = 1;
                }
                else
                {
                    cTraffic = 0;
                }

                strLine = sr.ReadLine();

                if (strLine == "2")
                {
                    dTraffic = 2;
                }
                else if (strLine == "1")
                {
                    dTraffic = 1;
                }
                else
                {
                    dTraffic = 0;
                }

                strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    disableCollisionCheck = true;
                }
                else
                {
                    disableCollisionCheck = false;
                }

                sr.Close();
                fs.Close();
            }
        }

        public void GetEffortIdex(int index)
        {
            aTraffic = index;
            SaveSetting();
            //MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            //method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex1(int index1)
        {
            bTraffic = index1;
            SaveSetting();
            //MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            //method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex2(int index2)
        {
            cTraffic = index2;
            SaveSetting();
            //MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            //method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex3(int index3)
        {
            dTraffic = index3;
            SaveSetting();
            //MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            //method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }
    }
}
