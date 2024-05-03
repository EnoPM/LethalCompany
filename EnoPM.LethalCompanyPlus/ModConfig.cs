using BepInEx;
using BepInEx.Configuration;
using UnityEngine.InputSystem;

namespace EnoPM.LethalCompanyPlus;

internal static class ModConfig
{
    internal static ConfigEntry<bool> EnableInfiniteSprint { get; private set; }
    internal static ConfigEntry<bool> NoItemSwitchCooldown { get; private set; }
    internal static ConfigEntry<bool> EnableDebugHostMenu { get; private set; }
    internal static ConfigEntry<Key> OpenModGuiKeyCode { get; private set; }
    internal static ConfigEntry<bool> EnableHealthRegeneration { get; private set; }
    internal static ConfigEntry<float> HealthRegenerationTimer { get; private set; }
    internal static ConfigEntry<float> ReachDistance { get; private set; }
    
    internal static void Load(BaseUnityPlugin plugin)
    {
        EnableInfiniteSprint = plugin.Config.Bind("LethalCompanyPlus", "Infinite Sprint", true, "Disable stamina consumption");
        NoItemSwitchCooldown = plugin.Config.Bind("LethalCompanyPlus", "No Switch Cooldown", true, "Disable switching item cooldown");
        EnableDebugHostMenu = plugin.Config.Bind("LethalCompanyPlus", "Debug Menu", true, "Allow host debug menu");
        OpenModGuiKeyCode = plugin.Config.Bind("LethalCompanyPlus", "Open Mod GUI", Key.F10, $"Open/close {ProjectInfos.Name} GUI");
        EnableHealthRegeneration = plugin.Config.Bind("Health Regeneration", "Enable", true, "Enable Health Regeneration");
        HealthRegenerationTimer = plugin.Config.Bind("Health Regeneration", "Interval", 0.1f, "Interval to regenerate health");
        ReachDistance = plugin.Config.Bind("Client Config", "Grab reach", 9999f, "Reach distance to grab items");
    }
}