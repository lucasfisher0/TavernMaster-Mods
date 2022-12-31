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
using UltimateLibrary.Managers;
using UltimateLibrary.Interfaces;
using UnityEngine.SceneManagement;

using Logger = UltimateLibrary.Logger;

namespace UltimateLibrary;

/// <summary>
/// Main class for the Ultimate Library plugin
/// </summary>
[BepInPlugin( PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION )]
public partial class Main : BaseUnityPlugin
{
    /// <summary>
    /// Plugin GUID
    /// </summary>
    public const string PLUGIN_GUID = "org.bepinex.tavernmaster.UltimateLibrary";

    /// <summary>
    /// Plugin Name
    /// </summary>
    public const string PLUGIN_NAME = "Ultimate Library";

    /// <summary>
    /// Plugin Version
    /// </summary>
    public const string PLUGIN_VERSION = "1.0.0";

    internal static Main Instance;
    internal static Harmony Harmony;
    internal static GameObject RootObject;

    private List<IManager> Managers;

    /// <summary>
    /// Plugin startup logic
    /// </summary>
    private void Awake()
    {
        Instance = this;
        UltimateLibrary.Logger.Init();
        UltimateLibrary.Logger.LogInfo( $"Loading plugin {Info.Metadata.Name}" );

        // Harmony patches
        Harmony = new Harmony( PLUGIN_GUID );
        // Harmony.PatchAll( typeof( ModCompatibility ) );

        // Root Container for GameObjects in the DontDestroyOnLoad scene
        RootObject = new GameObject("_LibraryRoot");
        DontDestroyOnLoad( RootObject );

        // Create and initialize all managers
        Managers = new List<IManager>
        {
            DrinkManager.Instance,
        };
        foreach ( IManager manager in Managers )
        {
            UltimateLibrary.Logger.LogInfo( $"Initializing {manager.GetType().Name}..." );
            manager.Init();
        }

        // ModQuery.Init();
        // ModCompatibility.Init();

        #if DEBUG
            // Enable helper on DEBUG build
            // RootObject.AddComponent<DebugUtils.DebugHelper>();
        #endif

        UltimateLibrary.Logger.LogInfo( $"Ultimate Library v{PLUGIN_VERSION} loaded successfully." );
    }

    private void Start()
    {
        // TODO ENABLE MOD-DEPENDENT PATCHING
        // InitializePatches();
    }

    private void OnApplicationQuit()
    {
        // Unload still loaded asset bundles to keep unity from crashing
        AssetBundle.UnloadAllAssetBundles( false );
    }
}