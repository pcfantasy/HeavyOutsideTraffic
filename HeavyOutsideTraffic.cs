using ColossalFramework.UI;
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
        public static int nonHighwayScale = 0;

        public string Name
        {
            get { return "HeavyOutsideTraffic"; }
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
            UIHelperBase group = helper.AddGroup("SideA Traffic(A侧车流)");
            group.AddDropdown("Traffic Select(车流选择)", new string[] { "Low Traffic(低车流量)", "Medium Traffic(中等车流量)", "Heavy Traffic(大车流量)" }, aTraffic, (index) => GetEffortIdex(index));

            UIHelperBase group1 = helper.AddGroup("SideB Traffic(B侧车流)");
            group1.AddDropdown("Traffic Select(车流选择)", new string[] { "Low Traffic(低车流量)", "Medium Traffic(中等车流量)", "Heavy Traffic(大车流量)" }, bTraffic, (index1) => GetEffortIdex1(index1));

            UIHelperBase group2 = helper.AddGroup("SideC Traffic(C侧车流)");
            group2.AddDropdown("Traffic Select(车流选择)", new string[] { "Low Traffic(低车流量)", "Medium Traffic(中等车流量)", "Heavy Traffic(大车流量)" }, cTraffic, (index2) => GetEffortIdex2(index2));

            UIHelperBase group3 = helper.AddGroup("SideD Traffic(D侧车流)");
            group3.AddDropdown("Traffic Select(车流选择)", new string[] { "Low Traffic(低车流量)", "Medium Traffic(中等车流量)", "Heavy Traffic(大车流量)" }, dTraffic, (index3) => GetEffortIdex3(index3));

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

                sr.Close();
                fs.Close();
            }
        }

        public void GetEffortIdex(int index)
        {
            aTraffic = index;
            SaveSetting();
            MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex1(int index1)
        {
            bTraffic = index1;
            SaveSetting();
            MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex2(int index2)
        {
            cTraffic = index2;
            SaveSetting();
            MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }

        public void GetEffortIdex3(int index3)
        {
            dTraffic = index3;
            SaveSetting();
            MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }
    }
}
