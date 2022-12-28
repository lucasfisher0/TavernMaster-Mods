using System;
using System.Collections.Generic;
using BepInEx;
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
            new ConfigDefinition( "General", "modEnabled" ),
            true,
            new ConfigDescription( "Disable the mod here if desired." )
        );
    }
}