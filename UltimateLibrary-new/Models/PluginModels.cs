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
    internal static Dictionary<string, IUltimateModel> customModels = new Dictionary<string, IUltimateModel>();

    public void InitializePlugins()
    {
        foreach ( var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos )
        {
            if ( plugin.Value.Instance is ISavablePlugin savable )
            {
                savablePlugins.Add( plugin.Value.Metadata.GUID, savable );
            }
        }
    }












    // internal static Dictionary<string, ISavablePlugin> savablePlugins = new Dictionary<string, ISavablePlugin>();
// 
    // public void InitializePluginSaving()
    // {
    //     foreach ( var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos )
    //     {
    //         if ( plugin.Value.Instance is ISavablePlugin savable )
    //         {
    //             savablePlugins.Add( plugin.Value.Metadata.GUID, savable );
    //         }
    //     }
    // }
}