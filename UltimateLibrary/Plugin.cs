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
using UltimateLibrary.Extensions;
using UltimateLibrary.Utility;


namespace UltimateLibrary;

/// <summary>
/// Main class for the Ultimate Library plugin
/// </summary>
[BepInPlugin( PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION )]
public partial class UltimateLibraryPlugin : BaseUnityPlugin
{
    internal static UltimateLibraryPlugin Instance;
    internal static Harmony Harmony;
    internal static GameObject RootObject;

    /// <summary>
    /// Action invoked when the main is opened, with a bool
    /// of whether it is the initial load.
    /// </summary>
    public static event Action<bool> OnSceneMainMenu;

    /// <summary>
    /// Action invoked when the Tavern world is loaded.
    /// </summary>
    public static event Action OnSceneGame;

    private List<IManager> Managers;

    /// <summary>
    /// Plugin startup logic
    /// </summary>
    private void Awake()
    {
        Instance = this;
        UltimateLibrary.Logger.Init();
        UltimateLibrary.Logger.LogInfo( $"Loading plugin {Info.Metadata.Name}" );

        // Root Container for GameObjects in the DontDestroyOnLoad scene
        RootObject = new GameObject("_LibraryRoot");
        DontDestroyOnLoad( RootObject );

        // Patching and events
        Harmony = new Harmony( Info.Metadata.GUID );
        Harmony.PatchAll( typeof( Patches ) );
        InitializeEvents();
        InitializeManagers();
        
        /*
        ModQuery.Init();
        ModCompatibility.Init();

        #if DEBUG
            Enable helper on DEBUG build
            RootObject.AddComponent<DebugUtils.DebugHelper>();
        #endif
        */

        UltimateLibrary.Logger.LogInfo( $"Ultimate Library v{PluginInfo.PLUGIN_VERSION} loaded successfully." );
    }

    private void InitializeManagers()
    {
        Managers = new List<IManager>
        {
            DrinkManager.Instance,
            MenuConfigManager.Instance, // currently broken, not retrieving config files properly
        };

        foreach ( IManager manager in Managers )
        {
            UltimateLibrary.Logger.LogInfo( $"Initializing {manager.GetType().Name}..." );
            manager.Init();
        }
    }

    private void InitializeEvents()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
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

    void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        if ( newActiveScene.name == "MainMenu" )
        {
            OnSceneMainMenu?.SafeInvoke<bool>( previousActiveScene.name == null );
        }
        else if ( newActiveScene.name == "Tavern0" )
        {
            OnSceneGame?.SafeInvoke();
        }
    }

    private static class Patches
    {
        
        [HarmonyPatch( typeof( LocalizationModel ), nameof( LocalizationModel.GetText ) ), HarmonyPrefix]
        public static void GetText_Prefix( string key, ref string __state )
        {
            __state = key;
        }


        [HarmonyPatch( typeof( LocalizationModel ), nameof( LocalizationModel.GetText) ), HarmonyFinalizer]
        public static Exception GetText_Finalizer( Exception __exception, ref string __result, string __state )
        {
            if ( __exception == null )
                return __exception;

            UltimateLibrary.Logger.LogDebug( $"Localization: Attempting to resolve '{__state}' from custom dictionaries." );
            var locText = Localization.GetText( __state );

            // if ( locText != key )
            __result = locText;
            return null;

            UltimateLibrary.Logger.LogError( $"Localization: '{__state}' could not be resolved!" );
            return __exception;
        }
    }
}