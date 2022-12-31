using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Bootstrap;

using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

using UltimateLibrary.Interfaces;
namespace UltimateLibrary;

public partial class UltimateLibraryPlugin : BaseUnityPlugin, IUltimateLibrary
{
    public ConfigEntry<bool> modEnabled;

    public void InitializeConfig()
    {
        // General
        modEnabled = Config.Bind
        (
            new ConfigDefinition( "General", "ultimateModEnabled" ),
            true,
            new ConfigDescription( "Disable the mod here if desired." )
        );
    }
}