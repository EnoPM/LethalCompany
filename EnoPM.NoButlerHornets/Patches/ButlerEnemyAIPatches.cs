using System;
using System.Collections;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace EnoPM.NoButlerHornets.Patches;

[HarmonyPatch(typeof(ButlerEnemyAI))]
internal static class ButlerEnemyAIPatches
{
    private static readonly int Popping = Animator.StringToHash("Popping");
    private static readonly int PopFinish = Animator.StringToHash("popFinish");

    [HarmonyPrefix, HarmonyPatch(nameof(ButlerEnemyAI.ButlerBlowUpAndPop))]
    private static bool ButlerBlowUpAndPopPrefix(ButlerEnemyAI __instance, out IEnumerator __result)
    {
        __result = __instance.CoButlerBlowUpAndPop();
        return false;
    }
    
    private static IEnumerator CoButlerBlowUpAndPop(this ButlerEnemyAI butlerEnemyAi)
    {
        butlerEnemyAi.creatureAnimator.SetTrigger(Popping);
        butlerEnemyAi.creatureAnimator.SetLayerWeight(1, 0.0f);
        butlerEnemyAi.popAudio.PlayOneShot(butlerEnemyAi.enemyType.audioClips[3]);
        yield return new WaitForSeconds(1.1f);
        butlerEnemyAi.creatureAnimator.SetBool(PopFinish, true);
        butlerEnemyAi.popAudio.Play();
        butlerEnemyAi.popAudioFar.Play();
        butlerEnemyAi.popParticle.Play(true);
        var distance = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, butlerEnemyAi.transform.position);
        if (distance < 8.0)
        {
            Landmine.SpawnExplosion(butlerEnemyAi.transform.position + Vector3.up * 0.15f, killRange: 0.0f, damageRange: 2f, nonLethalDamage: 30, physicsForce: 80f);
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            SoundManager.Instance.earsRingingTimer = 0.8f;
        }
        else if (distance < 27.0)
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
        if (butlerEnemyAi.IsServer)
        {
            //RoundManager.Instance.SpawnEnemyGameObject(butlerEnemyAi.transform.position, 0.0f, -1, butlerEnemyAi.butlerBeesEnemyType);
            UnityEngine.Object.Instantiate(butlerEnemyAi.knifePrefab, butlerEnemyAi.transform.position + Vector3.up * 0.5f, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer).GetComponent<NetworkObject>().Spawn();
        }
    }
}