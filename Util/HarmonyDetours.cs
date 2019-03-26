﻿using Harmony;
using System.Reflection;
using System;
using UnityEngine;
using HeavyOutsideTraffic.CustomAI;

namespace HeavyOutsideTraffic.Util
{
    public static class HarmonyDetours
    {
        private static HarmonyInstance harmony = null;
        private static void ConditionalPatch(this HarmonyInstance harmony, MethodBase method, HarmonyMethod prefix, HarmonyMethod postfix)
        {
            var fullMethodName = string.Format("{0}.{1}", method.ReflectedType?.Name ?? "(null)", method.Name);
            if (harmony.GetPatchInfo(method)?.Owners?.Contains(harmony.Id) == true)
            {
                DebugLog.LogToFileOnly("Harmony patches already present for {0}" + fullMethodName.ToString());
            }
            else
            {
                DebugLog.LogToFileOnly("Patching {0}..." + fullMethodName.ToString());
                harmony.Patch(method, prefix, postfix);
            }
        }

        private static void ConditionalUnPatch(this HarmonyInstance harmony, MethodBase method, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
        {
            var fullMethodName = string.Format("{0}.{1}", method.ReflectedType?.Name ?? "(null)", method.Name);
            if (prefix != null)
            {
                DebugLog.LogToFileOnly("UnPatching Prefix{0}..." + fullMethodName.ToString());
                harmony.Unpatch(method, HarmonyPatchType.Prefix);
            }
            if (postfix != null)
            {
                DebugLog.LogToFileOnly("UnPatching Postfix{0}..." + fullMethodName.ToString());
                harmony.Unpatch(method, HarmonyPatchType.Postfix);
            }
        }

        public static void Apply()
        {
            harmony = HarmonyInstance.Create("HeavyOutsideTraffic");
            //1
            var outsideConnectionAISimulationStep = typeof(OutsideConnectionAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType()}, null);
            var outsideConnectionAISimulationStepPostFix = typeof(CustomOutsideConnectionAI).GetMethod("OutsideConnectionAISimulationStepPostFix");
            harmony.ConditionalPatch(outsideConnectionAISimulationStep,
                null,
                new HarmonyMethod(outsideConnectionAISimulationStepPostFix));
            Loader.HarmonyDetourInited = true;
            DebugLog.LogToFileOnly("Harmony patches applied");
        }

        public static void DeApply()
        {
            //1
            var outsideConnectionAISimulationStep = typeof(OutsideConnectionAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType()}, null);
            var outsideConnectionAISimulationStepPostFix = typeof(CustomOutsideConnectionAI).GetMethod("OutsideConnectionAISimulationStepPostFix");
            harmony.ConditionalUnPatch(outsideConnectionAISimulationStep,
                null,
                new HarmonyMethod(outsideConnectionAISimulationStepPostFix));
            Loader.HarmonyDetourInited = false;
            DebugLog.LogToFileOnly("Harmony patches DeApplied");
        }
    }
}
