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
using UltimateLibrary.Interfaces;

namespace UltimateLibrary;

public partial class UltimateLibraryPlugin : BaseUnityPlugin, IUltimateLibrary
{
    internal static Dictionary<string, IUltimatePlugin> plugins = new Dictionary<string, IUltimatePlugin>();

    public void InitializePlugins()
    {
        foreach ( var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos )
        {
            if ( plugin.Value.Instance is IUltimatePlugin ultimatePlugin)
            {
                plugins.Add( plugin.Value.Metadata.GUID, ultimatePlugin );
            }
        }
    }
}