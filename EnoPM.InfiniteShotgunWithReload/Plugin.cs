using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace EnoPM.InfiniteShotgunWithReload;

[BepInPlugin(ProjectInfos.Guid, ProjectInfos.Name, ProjectInfos.Version)]
[BepInDependency("com.sigurd.csync", "5.0.1")]
public sealed class Plugin : BaseUnityPlugin
{
    private static readonly Harmony HarmonyPatcher = new(ProjectInfos.Guid);
    
    public static HostConfig HostConfig { get; private set; }
    public static ManualLogSource Log { get; private set; }
    
    private void Awake()
    {
        Log = Logger;
        
        Config.SaveOnConfigSet = false;
        HostConfig = new HostConfig(Config);
        Config.Save();
        
        HarmonyPatcher.PatchAll();
        Logger.LogInfo($"Plugin {ProjectInfos.Guid} is loaded!");

        HostConfig.InitialSyncCompleted += HostConfig.OnInitialSyncCompleted;
    }
}