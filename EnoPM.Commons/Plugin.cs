using BepInEx;

namespace EnoPM.Commons;

[BepInPlugin(ProjectInfos.Guid, ProjectInfos.Name, ProjectInfos.Version)]
[BepInProcess("Lethal Company")]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {ProjectInfos.Guid} is loaded!");
    }
}