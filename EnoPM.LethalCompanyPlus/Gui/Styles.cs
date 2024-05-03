using System.Collections.Generic;
using UnityEngine;

namespace EnoPM.LethalCompanyPlus.Gui;

internal static class Styles
{
    private static readonly Dictionary<string, GUIStyle> Cache = new();

    internal static GUIStyle BaseWindow
    {
        get
        {
            if (!Cache.TryGetValue(nameof(BaseWindow), out var value))
            {
                value = Cache[nameof(BaseWindow)] = new GUIStyle(GUI.skin.window);
                value.SetBackgroundFix(ImGUI.DarkTexture);
            }
            return value;
        }
    }
    
    internal static GUIStyle BaseBox
    {
        get
        {
            if (!Cache.TryGetValue(nameof(BaseBox), out var value))
            {
                value = Cache[nameof(BaseBox)] = new GUIStyle(GUI.skin.box);
                value.SetBackgroundFix(ImGUI.PrimaryTexture);
            }
            return value;
        }
    }
    
    internal static GUIStyle BaseTextField
    {
        get
        {
            if (!Cache.TryGetValue(nameof(BaseTextField), out var value))
            {
                value = Cache[nameof(BaseTextField)] = new GUIStyle(GUI.skin.textField);
                value.SetBackgroundFix(ImGUI.DarkTexture);
                value.SetTextColorFix(Color.white);
                value.fontSize = 16;
            }
            return value;
        }
    }
    
    internal static GUIStyle BaseButton
    {
        get
        {
            if (!Cache.TryGetValue(nameof(BaseButton), out var value))
            {
                value = Cache[nameof(BaseButton)] = new GUIStyle(GUI.skin.button);
                value.SetBackgroundFix(ImGUI.SecondaryTexture);
                value.SetTextColorFix(Color.black);
                value.fontSize = 14;
                value.fontStyle = FontStyle.Normal;
            }
            return value;
        }
    }
    
    internal static GUIStyle OpenMenuHelpLabel
    {
        get
        {
            if (!Cache.TryGetValue(nameof(OpenMenuHelpLabel), out var value))
            {
                value = Cache[nameof(OpenMenuHelpLabel)] = new GUIStyle(GUI.skin.label);
                value.SetTextColorFix(ImGUI.SecondaryColor);
                value.alignment = TextAnchor.MiddleLeft;
                value.fontSize = 16;
            }
            return value;
        }
    }
    
    internal static GUIStyle TitleLabel
    {
        get
        {
            if (!Cache.TryGetValue(nameof(TitleLabel), out var value))
            {
                value = Cache[nameof(TitleLabel)] = new GUIStyle(GUI.skin.label);
                value.SetTextColorFix(Color.white);
                value.SetBackgroundFix(ImGUI.DarkTexture);
                value.alignment = TextAnchor.MiddleCenter;
                value.fontSize = 22;
                value.fontStyle = FontStyle.Bold;
            }
            return value;
        }
    }

    private static void SetBackgroundFix(this GUIStyle style, Texture2D texture)
    {
        style.normal.background = texture;
        style.active.background = texture;
        style.hover.background = texture;
        style.focused.background = texture;
        style.onActive.background = texture;
        style.onFocused.background = texture;
        style.onHover.background = texture;
        style.onNormal.background = texture;
    }
    
    private static void SetTextColorFix(this GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.active.textColor = color;
        style.hover.textColor = color;
        style.focused.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
        style.onHover.textColor = color;
        style.onNormal.textColor = color;
    }
}