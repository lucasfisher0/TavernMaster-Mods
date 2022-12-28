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

using UnityEngine.SceneManagement;

namespace UltimateLibrary;

[BepInPlugin( PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION )]
public partial class UltimateLibraryPlugin : BaseUnityPlugin, IUltimateLibrary
{
    public const string PLUGIN_GUID = "org.bepinex.tavernmaster.UltimateLibrary";
    public const string PLUGIN_NAME = "Ultimate Library";
    public const string PLUGIN_VERSION = "1.0.1.0";
    private Harmony harmony = new ( PLUGIN_GUID );
    private bool isPatched = false;

    public static UltimateLibraryPlugin I;
    public UltimateLibraryPlugin()
    {
        UltimateLibraryPlugin.I = this;
    }

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

        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy()
    {
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} is unloading!" );
        harmony.UnpatchSelf();

        
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} successfully unloaded!" );
    }

    private void SceneLoaded( Scene scene,  LoadSceneMode loadSceneMode )
    {
        // Execute patching after unity has finished it's startup and loaded at least the first game scene
        if ( !isPatched )
        {
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
            Logger.LogInfo( $"Successfully patched {harmony.GetPatchedMethods().Count()} methods." );
            isPatched = true;

            // TEST - Load a new drink type!
            UltimateBar.UltimateBar.RegisterDrink(
                new()
                {
                    drinkName = "moonshine",
                    localizedNames = new()
                    {
                        { LocalizationModel.LanguageType.English, "Moonshine" },
                        { LocalizationModel.LanguageType.Spanish, "licor destilado ilegalmente" }
                    },
                    refillCost = 5,
                    price = 8,
                    customColor = Color.white
                }
            );
        }
    }

    private void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        if ( newActiveScene.name == "Tavern0" )
        {
            return;
        }
        
        if ( newActiveScene.name == "MainMenu" )
        {
            return;
        }
    }
}