using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;
using UltimateLibrary;
using UltimateLibrary.Utility;
using UltimateLibrary.Entities;
using UltimateLibrary.Managers;

namespace UltimateTavernMaster;

[BepInPlugin( PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION )]
[BepInDependency( "UltimateLibrary",  "1.0.0" )]
internal class UltimateTavernMaster : BaseUnityPlugin
{
    private static Harmony Harmony;

    /* CONFIGURABLE SETTINGS */
    public static ConfigEntry<bool> enableClockMod;
    public static ConfigEntry<bool> hideHudBar;
    public static ConfigEntry<bool> showMinutes;
    public static ConfigEntry<bool> uppercasePeriod;
    public static ConfigEntry<int> superSpeedModifier;

    private void Awake()
    {
        UltimateLibrary.Logger.LogInfo( $"Loading plugin {Info.Metadata.Name}" );

        // Events
        UltimateLibraryPlugin.OnSceneMainMenu += PatchMainUI;
        UltimateLibraryPlugin.OnSceneGame += PatchGameUI;

        // Localization
        {
            var locFile = @"UltimateTavernMaster\Assets\localization.csv";
            var locText = AssetUtils.LoadText( locFile );
            Localization.AddCsvText( locText, locFile );
        }

        // Register new items and behaviors
        UltimateLibrary.Logger.LogInfo( $"Registering items..." );
        AddDrinks();
        AddConfiguration();

        // Harmony patches
        UltimateLibrary.Logger.LogInfo( $"Registering hooks..." );
        Harmony = new Harmony( Info.Metadata.GUID );
        Harmony.PatchAll( typeof( UltimateUIPatches ) );
        UltimateLibrary.Logger.LogInfo( $"Successfully patched {Harmony.GetPatchedMethods().Count()} methods." );

        UltimateLibrary.Logger.LogInfo( $"Plugin {PluginInfo.PLUGIN_GUID} is loaded!" );
    }

    private void AddDrinks()
    {
        UltimateLibrary.Logger.LogDebug( "Loading drinks..." );
        DrinkManager.Instance.AddDrinksFromJson(  @"UltimateTavernMaster\Assets\drinks.json" );
    }

    private void AddConfiguration()
    {
        Config.SaveOnConfigSet = true;

        // Clock Mod
        enableClockMod = Config.Bind( "Clock Mod", "EnableClockMod", true,
            new ConfigDescription( "Enable Clock Mod and all related UI tweaks.", null,
            new ConfigurationManagerAttributes() ) );

        hideHudBar = Config.Bind( "Clock Mod", "HideHudBar", true,
            new ConfigDescription( "Hides the top bar in game.", null,
            new ConfigurationManagerAttributes() ) );

        showMinutes = Config.Bind( "Clock Mod", "ShowMinutes", true,
            new ConfigDescription( "Adds minutes to the time display.", null,
            new ConfigurationManagerAttributes() ) );

        uppercasePeriod = Config.Bind( "Clock Mod", "UppercasePeriod", true,
            new ConfigDescription( "Uppercases the AM/PM for 12-hr clock. Requires showing minutes.", null,
            new ConfigurationManagerAttributes() ) );

        superSpeedModifier = Config.Bind( "Clock Mod", "SuperSpeedModifier", 5,
            new ConfigDescription( "Super-speed time modifier. The default fast-forward is 5x speed, vanilla is 3x.", null,
            new ConfigurationManagerAttributes() ) );
    }

    private void PatchMainUI( bool isInitialLoad )
    {
        UltimateLibrary.Logger.LogInfo( $"Patching main-menu UI!" );

        var ui = GameObject.Find( "MainMenuUI" );
        for ( int i = 0; i < ui.transform.childCount; i++ )
        {
            var gameObj = ui.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "ButtonsPanel":
                case "Logo (2)":
                case "DiscordButton":
                case "QqButton":
                case "JoinCommunity":
                case "NewUpdatePanel":
                case "PopupHolder":
                    break;
                default:
                    UltimateLibrary.Logger.LogDebug( $"Main menu object has no hook: {gameObj.name}" );
                    break;
            }
        }
    }

    private void PatchGameUI()
    {
        UltimateLibrary.Logger.LogInfo( "Patching game menu..." );

        var ui = GameObject.Find( "GameUI" );
        for ( int i = 0; i < ui.transform.childCount; i++ )
        {
            var gameObj = ui.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "TopHeader":
                    HeaderTweaks( gameObj );
                    break;
                case "SeatIconsParent":
                case "IconTooltip":
                case "DragArea":
                case "SelectOutline":
                case "PropUI":
                case "ThirdPersonUIRenderer":
                case "PropShop": // buy furniture
                case "BarShop": // buy barrels
                case "StructureShop": // building
                case "NotificationCenter": // missing notifications mostly
                case "PathfindingHeatmapLegend":
                case "FireButtons":
                case "LaunchPad": // bottom menu
                case "LowerRightGroup":
                case "TutorialMessage":
                case "TutorialHand":
                case "PopupHolder":
                case "TutorialMessageForPopups":
                case "TutorialHandForPopups":
                    break;
                default:
                    UltimateLibrary.Logger.LogDebug( $"In-game menu object has no hook: {gameObj.name}" );
                    break;
            }
        }
    }

    void HeaderTweaks( GameObject topHeader )
    {
        for ( int i = 0; i < topHeader.transform.childCount; i++ )
        {
            var gameObj = topHeader.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "Bg":
                    gameObj.SetActive( false );
                    break;
                case "TimeControlButtons":
                    break;
                case "StatsButton": // missing notifications mostly
                case "IndicatorsButton": // bottom menu
                case "MoneyLabel":
                case "PrestigeLabel":
                case "GuestsCounter":
                case "FloorControlPanel": // move up/down floors
                case "FireTooltip":
                case "PauseMenuButton":
                    break;
                default:
                    UltimateLibrary.Logger.LogDebug( $"Menu header object has no hook: {gameObj.name}" );
                    break;
            }
        }
    }
}