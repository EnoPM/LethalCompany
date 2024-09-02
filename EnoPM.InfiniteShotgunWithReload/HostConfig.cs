using System;
using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using Unity.Netcode;

namespace EnoPM.InfiniteShotgunWithReload;

public sealed class HostConfig : SyncedConfig2<HostConfig>
{
    private static bool SyncedState { get; set; }

    public static bool IsSynced()
    {
        if (!NetworkManager.Singleton)
        {
            return false;
        }
        if (NetworkManager.Singleton.IsServer)
        {
            return true;
        }

        if (!SyncedState)
        {
            Plugin.Log.LogError($"({nameof(HostConfig)}) Client joined without synced state");
        }

        return SyncedState;
    }
    
    [field: SyncedEntryField]
    public SyncedEntry<bool> Enabled { get; }

    public HostConfig(ConfigFile configFile) : base(ProjectInfos.Guid)
    {
        Enabled = configFile.BindSyncedEntry(ProjectInfos.Guid, "Enabled", true, "Globally enable/disable the plugin");
        
        ConfigManager.Register(this);
    }

    internal static void OnInitialSyncCompleted(object _0, EventArgs _1)
    {
        Plugin.Log.LogInfo("HostConfig synced");
        SyncedState = true;
    }
}