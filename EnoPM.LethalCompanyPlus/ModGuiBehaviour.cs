using EnoPM.LethalCompanyPlus.Gui;
using Unity.Netcode;
using UnityEngine;

namespace EnoPM.LethalCompanyPlus;

internal class ModGuiBehaviour : MonoBehaviour
{
    
    private bool ShouldBeDisplayed { get; set; }
    
    private ModdedKeyBind ToggleKeyBind { get; set; }

    private void Start()
    {
        ToggleKeyBind = new ModdedKeyBind(ModConfig.OpenModGuiKeyCode);
        ToggleKeyBind.Pressed += OnToggleKeyBindPressed;
    }

    private void OnToggleKeyBindPressed()
    {
        if (!ModConfig.EnableDebugHostMenu.Value) return;
        ShouldBeDisplayed = !ShouldBeDisplayed;
    }
    
    private string ScrapValue { get; set; }

    private void OnGUI()
    {
        if (!ShouldBeDisplayed) return;
        GUI.Box(new Rect(0f, 0f, 500f, Screen.height), ImGUI.PrimaryTexture, Styles.BaseBox);
        GUI.Label(new Rect(0f, 0f, 500f, 40f), $"{ProjectInfos.Name} <size=16>v{ProjectInfos.Version}</size>", Styles.TitleLabel);
        if (!StartOfRound.Instance || !StartOfRound.Instance.allItemsList) return;
        ScrapValue = GUI.TextField(new Rect(0f, 30f, 500f, 25f), ScrapValue, Styles.BaseTextField);
        const int maxColumns = 3;
        const int columnWidth = 490 / maxColumns;
        var row = 1;
        var column = 1;
        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
        {
            if (column >= maxColumns)
            {
                column = 1;
                row++;
            }
            else
            {
                column++;
            }
            var x = (column - 1) * (columnWidth + 5);
            if (GUI.Button(new Rect(x, 30f + 30f * row, columnWidth, 25f), item.itemName, Styles.BaseButton))
            {
                BuyItem(item);
            }
        }
        //GUI.Window(1, new Rect(0f, 0f, 500f, Screen.height), MainWindow, string.Empty, Styles.BaseWindow);
    }

    private void BuyItem(Item item)
    {
        if (!GameNetworkManager.Instance || !GameNetworkManager.Instance.localPlayerController || !StartOfRound.Instance || !StartOfRound.Instance.allItemsList) return;
        var pos = GameNetworkManager.Instance.localPlayerController.transform.position;
        var componentObject = Instantiate(item.spawnPrefab, pos, Quaternion.identity, StartOfRound.Instance.propsContainer);
        var component = componentObject.GetComponent<GrabbableObject>();
        component.fallTime = 0f;
        if (ScrapValue != string.Empty && int.TryParse(ScrapValue, out var value))
        {
            component.SetScrapValue(value);
        }
        componentObject.GetComponent<NetworkObject>().Spawn();
        
        Plugin.Log.LogMessage($"Buy item {item.itemName}");
    }
}