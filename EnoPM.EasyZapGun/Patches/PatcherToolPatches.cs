using HarmonyLib;

namespace EnoPM.EasyZapGun.Patches;

[HarmonyPatch(typeof(PatcherTool))]
internal static class PatcherToolPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(PatcherTool.ShiftBendRandomizer))]
    private static void ShiftBendRandomizerPrefix(PatcherTool __instance)
    {
        if (!HostConfig.IsSynced() || !Plugin.HostConfig.Enabled)
        {
            return;
        }
        __instance.bendMultiplier = 0f;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PatcherTool.LateUpdate))]
    private static void LateUpdatePostfix(PatcherTool __instance)
    {
        if (!HostConfig.IsSynced() || !Plugin.HostConfig.DisableGunOverheat)
        {
            return;
        }
        __instance.gunOverheat = 0f;
    }
}