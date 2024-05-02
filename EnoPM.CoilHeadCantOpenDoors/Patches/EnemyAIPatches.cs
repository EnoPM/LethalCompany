using HarmonyLib;

namespace EnoPM.CoilHeadCantOpenDoors.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal static class EnemyAIPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(EnemyAI.Start))]
    private static void StartPostfix(EnemyAI __instance)
    {
        if (__instance.__getTypeName() == nameof(SpringManAI))
        {
            __instance.openDoorSpeedMultiplier = 0f;
        }
    }
}