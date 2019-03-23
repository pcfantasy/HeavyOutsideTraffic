using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeavyOutsideTraffic.CustomAI
{
    class CustomCarAI
    {
        private static bool DisableCollisionCheck(ushort vehicleID, ref Vehicle vehicleData)
        {
            float num = Mathf.Max(Mathf.Abs(vehicleData.m_targetPos3.x), Mathf.Abs(vehicleData.m_targetPos3.z));
            float num2 = 8640f;
            if (num > num2 - 600f)
            {
                return true;
            }
            return false;
        }
    }
}
