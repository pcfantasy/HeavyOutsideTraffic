﻿using Harmony;
using System.Reflection;
using System;
using UnityEngine;
using HeavyOutsideTraffic.CustomAI;

namespace HeavyOutsideTraffic.Util
{
    internal static class HarmonyDetours
    {
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
            var harmony = HarmonyInstance.Create("HeavyOutsideTraffic");
            //1
            var outsideConnectionAISimulationStep = typeof(OutsideConnectionAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType()}, null);
            var outsideConnectionAISimulationStepPostFix = typeof(CustomOutsideConnectionAI).GetMethod("OutsideConnectionAISimulationStepPostFix");
            harmony.ConditionalPatch(outsideConnectionAISimulationStep,
                null,
                new HarmonyMethod(outsideConnectionAISimulationStepPostFix));
            //2
            var carAIDisableCollisionCheck = typeof(CarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
            var carAIDisableCollisionCheckPrefix = typeof(CustomCarAI).GetMethod("CarAIDisableCollisionCheckPrefix");
            harmony.ConditionalPatch(carAIDisableCollisionCheck,
                new HarmonyMethod(carAIDisableCollisionCheckPrefix),
                null);
            Loader.HarmonyDetourInited = true;
            DebugLog.LogToFileOnly("Harmony patches applied");
        }

        public static void DeApply()
        {
            var harmony = HarmonyInstance.Create("HeavyOutsideTraffic");
            //1
            var outsideConnectionAISimulationStep = typeof(OutsideConnectionAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType()}, null);
            var outsideConnectionAISimulationStepPostFix = typeof(CustomOutsideConnectionAI).GetMethod("OutsideConnectionAISimulationStepPostFix");
            harmony.ConditionalUnPatch(outsideConnectionAISimulationStep,
                null,
                new HarmonyMethod(outsideConnectionAISimulationStepPostFix));
            //2
            var carAIDisableCollisionCheck = typeof(CarAI).GetMethod("DisableCollisionCheck", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
            var carAIDisableCollisionCheckPrefix = typeof(CustomCarAI).GetMethod("CarAIDisableCollisionCheckPrefix");
            harmony.ConditionalUnPatch(carAIDisableCollisionCheck,
                new HarmonyMethod(carAIDisableCollisionCheckPrefix),
                null);
            Loader.HarmonyDetourInited = false;
            DebugLog.LogToFileOnly("Harmony patches DeApplied");
        }
    }
}
