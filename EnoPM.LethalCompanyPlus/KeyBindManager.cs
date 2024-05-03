using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnoPM.LethalCompanyPlus;

public class KeyBindManager : MonoBehaviour
{
    internal readonly List<ModdedKeyBind> KeyBinds = [];

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        foreach (var keyBind in KeyBinds)
        {
            keyBind.Update(keyboard);
        }
    }
}

public class ModdedKeyBind
{
    private readonly ConfigEntry<Key> _entry;
    private bool KeyWasDown { get; set; }
    public event Action Pressed;
    public event Action Released;
    public ModdedKeyBind(ConfigEntry<Key> key)
    {
        _entry = key;
        Plugin.KeyBindManager.KeyBinds.Add(this);
    }

    internal void Update(Keyboard keyboard)
    {
        var btn = keyboard[_entry.Value];
        if (btn.wasPressedThisFrame && !KeyWasDown)
        {
            KeyWasDown = true;
            Pressed?.Invoke();
        }
        if (btn.wasReleasedThisFrame && KeyWasDown)
        {
            KeyWasDown = false;
            Released?.Invoke();
        }
    }
}