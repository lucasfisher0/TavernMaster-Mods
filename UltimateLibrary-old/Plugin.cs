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

namespace UltimateLibrary;

[BepInPlugin( PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION )]
public partial class Plugin : BaseUnityPlugin, IUltimateLibrary
{
    public const string PLUGIN_GUID = "org.bepinex.tavernmaster.UltimateLibrary";
    public const string PLUGIN_NAME = "Ultimate Library";
    public const string PLUGIN_VERSION = "1.0.0.0";
    private Harmony harmony = new( PLUGIN_GUID );

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} is loaded!" );

        InitializeConfig();
        if ( !modEnabled.Value )
        {
            Logger.LogWarning( $"{PLUGIN_NAME} is disabled from configuration, and will not load!" );
            return;
        }

        harmony.PatchAll( Assembly.GetExecutingAssembly() );
        Logger.LogInfo( $"Successfully patched {harmony.GetPatchedMethods().Count()} methods." );

        InitializePlugins();
    }

    private void OnDestroy()
    {
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} is unloading!" );
        harmony.UnpatchSelf();

        
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} successfully unloaded!" );
    }
}