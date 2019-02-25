using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeavyOutsideTraffic
{
    public class Language
    {
        public static string[] English =
        {
            "SideA Traffic",                                                                                      //0
            "SideB Traffic",                                                                               //1
            "SideC Traffic",
            "SideD Traffic",
            "Traffic Select",
            "Low Traffic",
            "Medium Traffic",
            "Heavy Traffic",
            "Type: ",
            "A outside",
            "B outside",
            "C outside",
            "D outside",
            "Disable collision check at the edge of the map to avoid traffic congestion",
            "DisableCollisionCheck",
        };



        public static string[] Chinese =
            {
            "A侧车流量",                                                                                      //0
            "B侧车流量",                                                                               //1
            "C侧车流量",
            "D侧车流量",
            "车流量选择",
            "低车流量",
            "中等车流量",
            "高车流量",
            "类型: ",
            "A侧外部道路",
            "B侧外部道路",
            "C侧外部道路",
            "D侧外部道路",
            "取消地图边缘部分的车辆碰撞检测,让地图边缘不再拥挤",
            "取消地图边缘部分的车辆碰撞检测",
        };


        public static string[] Strings = new string[English.Length];

        public static byte currentLanguage = 255;

        public static void LanguageSwitch(byte language)
        {
            if (language == 1)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = Chinese[i];
                }
                currentLanguage = 1;
            }
            else if (language == 0)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = English[i];
                }
                currentLanguage = 0;
            }
            else
            {
                DebugLog.LogToFileOnly("unknow language!! use English");
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = English[i];
                }
                currentLanguage = 0;
            }
        }
    }
}
