using System;
using System.Collections;
using System.Linq;
using DigitalRuby.ThunderAndLightning;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.EasyZapGun.Patches;

[HarmonyPatch(typeof(PatcherTool))]
internal static class PatcherToolPatches
{
    private static readonly int Scan = Animator.StringToHash("Scan");
    [HarmonyPrefix, HarmonyPatch(nameof(PatcherTool.ShiftBendRandomizer))]
    private static void ShiftBendRandomizerPrefix(PatcherTool __instance)
    {
        __instance.bendMultiplier = 0f;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PatcherTool.LateUpdate))]
    private static void LateUpdatePostfix(PatcherTool __instance)
    {
        if (!Plugin.DisableGunOverheat.Value) return;
        __instance.gunOverheat = 0f;
    }

    private static IEnumerator CoBeginScan(this PatcherTool patcherTool)
    {
        patcherTool.effectAnimator.SetTrigger(Scan);
        patcherTool.gunAudio.PlayOneShot(patcherTool.scanAnomaly);
        patcherTool.lightningScript = patcherTool.lightningObject.GetComponent<LightningSplineScript>();
        patcherTool.lightningDest.SetParent(null);
        patcherTool.lightningBend1.SetParent(null);
        patcherTool.lightningBend2.SetParent(null);
        Debug.Log("Scan A");
        for (int i = 0; i < 12; ++i)
        {
            if (patcherTool.IsOwner)
            {
                Debug.Log("Scan B");
                if (patcherTool.isPocketed)
                {
                    yield break;
                }
                patcherTool.ray = new Ray(patcherTool.playerHeldBy.gameplayCamera.transform.position - patcherTool.playerHeldBy.gameplayCamera.transform.forward * 3f, patcherTool.playerHeldBy.gameplayCamera.transform.forward);
                Debug.DrawRay(patcherTool.playerHeldBy.gameplayCamera.transform.position - patcherTool.playerHeldBy.gameplayCamera.transform.forward * 3f, patcherTool.playerHeldBy.gameplayCamera.transform.forward * 6f, Color.red, 5f);
                var num = Physics.SphereCastNonAlloc(patcherTool.ray, 5f, patcherTool.raycastEnemies, 5f, patcherTool.anomalyMask, QueryTriggerInteraction.Collide);
                patcherTool.raycastEnemies = patcherTool.raycastEnemies.OrderBy(x => x.distance).ToArray();
                for (int index = 0; index < num; ++index)
                {
                    if (index < patcherTool.raycastEnemies.Length)
                    {
                        patcherTool.hit = patcherTool.raycastEnemies[index];
                        IShockableWithGun component;
                        if (!(patcherTool.hit.transform == null) && patcherTool.hit.transform.gameObject.TryGetComponent(out component) && component.CanBeShocked())
                        {
                            Vector3 shockablePosition = component.GetShockablePosition();
                            Debug.Log("Got shockable transform name : " + component.GetShockableTransform().gameObject.name);
                            if (patcherTool.GunMeetsConditionsToShock(patcherTool.playerHeldBy, shockablePosition, 60f))
                            {
                                patcherTool.gunAudio.Stop();
                                patcherTool.BeginShockingAnomalyOnClient(component);
                                yield break;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.125f);
        }
        Debug.Log("Zap gun light off!!!");
        patcherTool.SwitchFlashlight(false);
        patcherTool.isScanning = false;
    }
}