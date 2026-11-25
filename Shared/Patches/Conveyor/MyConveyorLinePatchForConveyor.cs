using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game.GameSystems.Conveyors;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPatch(typeof(MyConveyorLine))]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class MyConveyorLinePatchForConveyor
    {
        private static IPluginConfig Config => Common.Config;

        // Only the power branch calls FlagForRecomputation, the others stop the line silently
        [HarmonyPatch(nameof(MyConveyorLine.UpdateIsWorking))]
        [HarmonyPrefix]
        [EnsureCode("6a58a31c")]
        private static void UpdateIsWorkingPrefix(MyConveyorLine __instance, ref bool __state)
        {
            __state = __instance.IsWorking;
        }

        [HarmonyPatch(nameof(MyConveyorLine.UpdateIsWorking))]
        [HarmonyPostfix]
        [EnsureCode("6a58a31c")]
        private static void UpdateIsWorkingPostfix(MyConveyorLine __instance, bool __state)
        {
            if (Config.FixConveyor && __instance.IsWorking != __state)
                MyGridConveyorSystemPatch.InvalidateCache((__instance.GetEndpoint(0) ?? __instance.GetEndpoint(1))?.CubeBlock?.SlimBlock?.CubeGrid);
        }
    }
}
