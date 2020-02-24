using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeavyOutsideTraffic.CustomAI
{
    public class CustomCarAI
    {
        public static bool CarAIDisableCollisionCheckPrefix(ushort vehicleID, ref Vehicle vehicleData, ref bool __result)
        {
            if (HeavyOutsideTraffic.disableCollisionCheck)
            {
                float num = Mathf.Max(Mathf.Abs(vehicleData.m_targetPos3.x), Mathf.Abs(vehicleData.m_targetPos3.z));
                float num2 = 8640f;
                if (num > num2 - 600f)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }
    }
}
