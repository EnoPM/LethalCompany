using HarmonyLib;

namespace EnoPM.LethalCompanyPlus.Patches;

[HarmonyPatch(typeof(StartMatchLever))]
internal static class StartMatchLeverPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(StartMatchLever.PlayLeverPullEffectsClientRpc))]
    private static void PlayLeverPullEffectsClientRpcPostfix(ref StartOfRound ___playersManager, ref bool ___leverHasBeenPulled)
    {
        if (!___leverHasBeenPulled) return;
        PlayerControllerBPatches.MaximumHealth = ___playersManager.localPlayerController.health;
    }
}