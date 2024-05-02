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
    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private static readonly int Stunned1 = Animator.StringToHash("stunned");
    private static readonly int Stun = Animator.StringToHash("stun");
    private static readonly int Dead = Animator.StringToHash("Dead");

    [HarmonyPrefix, HarmonyPatch(nameof(ButlerEnemyAI.KillEnemy))]
    private static bool KillEnemyPrefix(ButlerEnemyAI __instance, bool destroy)
    {
        __instance.OriginalKillEnemy(destroy);
        if (__instance.currentSearch.inProgress)
            __instance.StopSearch(__instance.currentSearch);
        __instance.ambience1.Stop();
        __instance.ambience1.volume = 0.0f;
        __instance.ambience2.Stop();
        __instance.ambience2.volume = 0.0f;
        __instance.agent.speed = 0.0f;
        __instance.agent.acceleration = 1000f;
        if (__instance.startedButlerDeathAnimation) return false;
        __instance.startedButlerDeathAnimation = true;
        __instance.StartCoroutine(__instance.CoButlerBlowUpAndPop());
        return false;
    }

    private static void OriginalKillEnemy(this EnemyAI enemyAi, bool destroy = false)
    {
        Debug.Log($"Kill enemy called; destroy: {destroy}");
        if (destroy)
        {
            if (!enemyAi.IsServer)
                return;
            Debug.Log("Despawn network object in kill enemy called!");
            if (!enemyAi.thisNetworkObject.IsSpawned)
                return;
            enemyAi.thisNetworkObject.Despawn();
        }
        else
        {
            var componentInChildren = enemyAi.gameObject.GetComponentInChildren<ScanNodeProperties>();
            if (componentInChildren)
            {
                var collider = componentInChildren.gameObject.GetComponent<Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }
            }
            enemyAi.isEnemyDead = true;
            if (enemyAi.creatureVoice)
            {
                enemyAi.creatureVoice.PlayOneShot(enemyAi.dieSFX);
            }
            try
            {
                if (enemyAi.creatureAnimator)
                {
                    enemyAi.creatureAnimator.SetBool(Stunned, false);
                    enemyAi.creatureAnimator.SetBool(Stunned1, false);
                    enemyAi.creatureAnimator.SetBool(Stun, false);
                    enemyAi.creatureAnimator.SetTrigger(nameof (EnemyAI.KillEnemy));
                    enemyAi.creatureAnimator.SetBool(Dead, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"enemy did not have bool in animator in KillEnemy, error returned; {ex}");
            }
            enemyAi.CancelSpecialAnimationWithPlayer();
            enemyAi.SubtractFromPowerLevel();
            enemyAi.agent.enabled = false;
        }
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
            UnityEngine.Object.Instantiate(butlerEnemyAi.knifePrefab, butlerEnemyAi.transform.position + Vector3.up * 0.5f, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer).GetComponent<NetworkObject>().Spawn();
        }
    }
}