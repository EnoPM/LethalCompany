using BepInEx;
using HarmonyLib;

namespace EnoPM.CoilHeadCantOpenDoors;

[BepInPlugin(ProjectInfos.Guid, ProjectInfos.Name, ProjectInfos.Version)]
public class Plugin : BaseUnityPlugin
{
    private static readonly Harmony HarmonyPatcher = new(ProjectInfos.Guid);
    
    private void Awake()
    {
        HarmonyPatcher.PatchAll();
        Logger.LogInfo($"Plugin {ProjectInfos.Guid} is loaded!");
    }
}