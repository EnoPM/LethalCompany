using HarmonyLib;

namespace EnoPM.LethalCompanyPlus.Patches;

[HarmonyPatch(typeof(Terminal))]
internal static class TerminalPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(Terminal.Start))]
    private static void StartPostfix(Terminal __instance)
    {
        __instance.moonsCatalogueList = StartOfRound.Instance.levels;
        foreach (var moon in __instance.moonsCatalogueList)
        {
            moon.lockedForDemo = false;
        }
    }
}