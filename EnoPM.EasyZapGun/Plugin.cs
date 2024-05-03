using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace EnoPM.EasyZapGun;

[BepInPlugin(ProjectInfos.Guid, ProjectInfos.Name, ProjectInfos.Version)]
public class Plugin : BaseUnityPlugin
{
    private static readonly Harmony HarmonyPatcher = new(ProjectInfos.Guid);
    
    internal static ConfigEntry<bool> DisableGunOverheat { get; private set; }
    
    private void Awake()
    {
        DisableGunOverheat = Config.Bind("Zap Gun", "DisableOverheat", true, "Disable Zap gun overheat");
        HarmonyPatcher.PatchAll();
        Logger.LogInfo($"Plugin {ProjectInfos.Guid} is loaded!");
    }
}