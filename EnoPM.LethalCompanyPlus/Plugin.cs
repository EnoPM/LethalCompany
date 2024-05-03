using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.LethalCompanyPlus;

[BepInPlugin(ProjectInfos.Guid, ProjectInfos.Name, ProjectInfos.Version)]
public class Plugin : BaseUnityPlugin
{
    private GameObject GuiObject { get; set; }
    
    internal static KeyBindManager KeyBindManager { get; private set; }
    internal static ModGuiBehaviour GuiBehaviour { get; set; }
    internal static ManualLogSource Log { get; private set; }
    private static readonly Harmony HarmonyPatcher = new(ProjectInfos.Guid);
    
    private void Awake()
    {
        Log = Logger;
        ModConfig.Load(this);
        GuiObject = new GameObject($"{nameof(ProjectInfos.Name)}Gui") { layer = 4 };
        DontDestroyOnLoad(GuiObject);
        GuiObject.hideFlags = HideFlags.HideAndDontSave;
        KeyBindManager = GuiObject.AddComponent<KeyBindManager>();
        GuiBehaviour = GuiObject.AddComponent<ModGuiBehaviour>();
        HarmonyPatcher.PatchAll();
        Log.LogInfo($"Plugin {ProjectInfos.Guid} is loaded!");
    }
}