using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.LethalCompanyPlus.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerControllerBPatches
{
    internal static int MaximumHealth = 100;
    
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControllerB.Update))]
    private static void UpdatePostfix(ref float ___timeSinceSwitchingSlots, ref float ___sprintMeter, ref float ___grabDistance)
    {
        if (ModConfig.NoItemSwitchCooldown.Value)
        {
            ___timeSinceSwitchingSlots = 1f;
        }
        if (ModConfig.EnableInfiniteSprint.Value)
        {
            ___sprintMeter = 1f;
        }
        ___grabDistance = ModConfig.ReachDistance.Value;
    }

    [HarmonyTranspiler, HarmonyPatch(nameof(PlayerControllerB.LateUpdate))]
    private static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        if (!ModConfig.EnableHealthRegeneration.Value) return instructions;
        var codes = new List<CodeInstruction>(instructions);

        for (var i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldfld && ((FieldInfo)codes[i].operand).Name == "health")
            {
                if (i + 1 < codes.Count && codes[i + 1].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i + 1].operand == 20)
                {
                    codes[i + 1].operand = (sbyte)MaximumHealth;
                }
            }
            else if (codes[i].opcode == OpCodes.Stfld && ((FieldInfo)codes[i].operand).Name == "healthRegenerateTimer")
            {
                if (i - 1 >= 0 && codes[i - 1].opcode == OpCodes.Ldc_R4 && Mathf.Approximately((float)codes[i - 1].operand, 1f))
                {
                    codes[i - 1].operand = ModConfig.HealthRegenerationTimer.Value;
                }
            }
        }

        return codes.AsEnumerable();
    }
}