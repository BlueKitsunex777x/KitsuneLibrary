using System;
using BepInEx;
using BlockStoryCore;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.BlueKitsunex777x.KitsuneLibrary", "Kitsune Library", "1.0.0")]
[BepInDependency(Core.Guid)]
public class KitsuneLibrary : BaseUnityPlugin
{
    public const string Guid = "com.BlueKitsunex777x.KitsuneLibrary";
    public static bool Enabled = PlayerPrefs.GetInt("WoodToCharcoal_Enabled", 1) != 0;

    public static bool _savedEnabled = Enabled;
    private bool _wasInWorld;

    private void Awake()
    {
        if (!BSMKCheck.Passed(this)) return;

        try { new Harmony(KitsuneLibrary.Guid).PatchAll(); L.Info("loaded"); }
        catch (Exception e) { L.Err("PatchAll", e); }

        ModRegistry.Register(new ModInfo
        {
            Name = "Kitsune Library",
            Description = "Library for my mods",
            GetEnabled = () => Enabled,
            SetEnabled = on =>
            {
                _savedEnabled = on;
                PlayerPrefs.SetInt("_Enabled", on ? 1 : 0);
                PlayerPrefs.Save();
                ApplySaved();
                L.Info($"enabled = {on}");
            },
            Config = new ModConfig()
        });
        L.Log("added to the mods menu, now waiting for a world to load");
    }

    // Run on game update cycle, checking if player has just left a world
    private void Update()
    {
        // leaving a world is when saved changes can take over, the next one loads with them
        bool inWorld = BSWorld.InWorld;
        if (_wasInWorld && !inWorld) { ApplySaved(); }
        _wasInWorld = inWorld;
    }

    // Saves any edit made to mods config while in-game?
    private static void ApplySaved()
    {
        if (BSWorld.InWorld)
        {
            L.Info("saved, used from the next world load");
            return;
        }
        Enabled = _savedEnabled;
    }
}