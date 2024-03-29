﻿using System;
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

namespace UltimateTavernMaster;

[BepInPlugin( PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION )]
//[BepInDependency( "org.bepinex.tavernmaster.UltimateLibrary",  "~1.0" )]
public class Plugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony( PluginInfo.PLUGIN_GUID );
    private UltimateConfigManager configManager;
    private ClockMod clockMod;

    // Inherits from UnityEngine.MonoBehavior and can use any of those methods
    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    private void Awake()
    {
        // Plugin startup logic
        // https://github.com/BepInEx/HarmonyX/wiki/
        Logger.LogInfo( $"Plugin {PluginInfo.PLUGIN_GUID} is loaded!" );
        
        // Configuration
        //Logger.LogDebug( $"Initializing config..." );
        configManager = new( this );

        if ( !configManager.modEnabled.Value )
        {
            Logger.LogInfo( $"{PluginInfo.PLUGIN_GUID} is disabled from the configuration, aborting...!" );
            return;
        }

        // Apply Harmony Patches
        Logger.LogInfo( "Applying ClockMod." );
        ClockPatch.superFastSpeed = configManager.superSpeedModifier.Value;
        ClockPatch.showMinutes    = configManager.showMinutes.Value;
        harmony.PatchAll( typeof( ClockPatch ) );

        Logger.LogInfo( $"Successfully patched {harmony.GetPatchedMethods().Count()} methods." );

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        Logger.LogDebug( "Active Scene Changed." );
        Logger.LogDebug( $"Previous scene: {previousActiveScene.name} with PATH {previousActiveScene.path}" );
        Logger.LogDebug( $"New scene: {newActiveScene.name} with PATH {newActiveScene.path}" );

        clockMod = null;

        if ( newActiveScene.name == "MainMenu" )
        {
            Logger.LogDebug( "Patching main menu..." );
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
                        Logger.LogDebug( $"Main menu object has no hook: {gameObj.name}" );
                        break;
                }

            }
        }
        else if ( newActiveScene.name == "Tavern0" )
        {
            Logger.LogDebug( "Patching game menu..." );
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
                        Logger.LogDebug( $"In-game menu object has no hook: {gameObj.name}" );
                        break;
                }

            }

        }
    }

    void HeaderTweaks( GameObject topHeader )
    {
        for ( int i = 0; i < topHeader.transform.childCount; i++ )
        {
            Logger.LogDebug( "Patching UI header..." );
            var gameObj = topHeader.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "Bg":
                    if ( configManager.transparentHeader.Value )
                    {
                        gameObj.SetActive( false );
                    }
                    break;
                case "StatsButton": // missing notifications mostly
                    break;
                case "IndicatorsButton": // bottom menu
                    if ( configManager.changeIndicatorButton.Value )
                    {
                        // TODO change indicator button icon
                        // var comp = gameObj.GetComponent<UnityEngine.UI.Image>();
                    }
                    break;
                case "MoneyLabel":
                case "PrestigeLabel":
                case "GuestsCounter":
                case "FloorControlPanel": // move up/down floors
                    break;
                case "TimeControlButtons":
                    if ( configManager.useEnhancedClock.Value )
                    {
                        clockMod = new ClockMod( gameObj );
                    }
                    break;
                case "FireTooltip":
                case "PauseMenuButton":
                    break;
                default:
                    Logger.LogDebug( $"Menu header object has no hook: {gameObj.name}" );
                    break;
            }
        }
    }
}