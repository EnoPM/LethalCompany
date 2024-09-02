using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.InfiniteShotgunWithReload.Patches;

[HarmonyPatch(typeof(ShotgunItem))]
internal static class ShotgunItemPatches
{
    private static readonly int Reloading = Animator.StringToHash("Reloading");
    private static readonly int ReloadShotgun = Animator.StringToHash("ReloadShotgun");
    private static readonly int ReloadShotgun2 = Animator.StringToHash("ReloadShotgun2");

    [HarmonyPostfix, HarmonyPatch(nameof(ShotgunItem.ReloadedGun))]
    private static void ReloadedGunPostfix(ref bool __result)
    {
        if (!HostConfig.IsSynced() || !Plugin.HostConfig.Enabled) return;
        __result = true;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(ShotgunItem.StartReloadGun))]
    private static bool StartReloadGunPrefix(ShotgunItem __instance)
    {
        if (!HostConfig.IsSynced() || !Plugin.HostConfig.Enabled) return true;
        if (__instance.ReloadedGun())
        {
            if (!__instance.IsOwner) return false;
            if (__instance.gunCoroutine != null)
            {
                __instance.StopCoroutine(__instance.gunCoroutine);
            }
            __instance.gunCoroutine = __instance.StartCoroutine(__instance.CoReloadGunAnimation());
        }
        else
        {
            __instance.gunAudio.PlayOneShot(__instance.noAmmoSFX);
        }

        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(ShotgunItem.reloadGunAnimation))]
    private static bool ReloadGunAnimationPrefix(ShotgunItem __instance)
    {
        __instance.gunCoroutine = __instance.StartCoroutine(__instance.CoReloadGunAnimation());
        return false;
    }

    private static IEnumerator CoReloadGunAnimation(this ShotgunItem shotgunItem)
    {
        shotgunItem.isReloading = true;
        if (shotgunItem.shellsLoaded <= 0)
        {
            shotgunItem.playerHeldBy.playerBodyAnimator.SetBool(ReloadShotgun, true);
            shotgunItem.shotgunShellLeft.enabled = false;
            shotgunItem.shotgunShellRight.enabled = false;
        }
        else
        {
            shotgunItem.playerHeldBy.playerBodyAnimator.SetBool(ReloadShotgun2, true);
            shotgunItem.shotgunShellRight.enabled = false;
        }
        yield return new WaitForSeconds(0.3f);
        shotgunItem.gunAudio.PlayOneShot(shotgunItem.gunReloadSFX);
        shotgunItem.gunAnimator.SetBool(Reloading, true);
        shotgunItem.ReloadGunEffectsServerRpc();
        yield return new WaitForSeconds(0.95f);
        shotgunItem.shotgunShellInHand.enabled = true;
        shotgunItem.shotgunShellInHandTransform.SetParent(shotgunItem.playerHeldBy.leftHandItemTarget);
        shotgunItem.shotgunShellInHandTransform.localPosition = new Vector3(-0.0555f, 0.1469f, -0.0655f);
        shotgunItem.shotgunShellInHandTransform.localEulerAngles = new Vector3(-1.956f, 143.856f, -16.427f);
        yield return new WaitForSeconds(0.95f);
        if (shotgunItem.ammoSlotToUse != -1)
        {
            shotgunItem.playerHeldBy.DestroyItemInSlotAndSync(shotgunItem.ammoSlotToUse);
            shotgunItem.ammoSlotToUse = -1;
        }
        shotgunItem.shellsLoaded = Mathf.Clamp(shotgunItem.shellsLoaded + 1, 0, 2);
        shotgunItem.shotgunShellLeft.enabled = true;
        if (shotgunItem.shellsLoaded == 2)
        {
            shotgunItem.shotgunShellRight.enabled = true;
        }
        shotgunItem.shotgunShellInHand.enabled = false;
        shotgunItem.shotgunShellInHandTransform.SetParent(shotgunItem.transform);
        yield return new WaitForSeconds(0.45f);
        shotgunItem.gunAudio.PlayOneShot(shotgunItem.gunReloadFinishSFX);
        shotgunItem.gunAnimator.SetBool(Reloading, false);
        shotgunItem.playerHeldBy.playerBodyAnimator.SetBool(ReloadShotgun, false);
        shotgunItem.playerHeldBy.playerBodyAnimator.SetBool(ReloadShotgun2, false);
        shotgunItem.isReloading = false;
        shotgunItem.ReloadGunEffectsServerRpc(false);
    }
}