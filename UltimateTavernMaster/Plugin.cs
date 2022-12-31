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

namespace UltimateTavernMaster;

[BepInPlugin( PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION )]
[BepInDependency( "org.bepinex.tavernmaster.UltimateLibrary",  "1.0" )]
internal class Plugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony( PluginInfo.PLUGIN_GUID );

    private void Awake()
    {
    
        Logger.LogInfo( $"Plugin {PluginInfo.PLUGIN_GUID} is loaded!" );
        harmony.PatchAll( typeof( WeeklyEventsPopupPatch ) );
        SceneManager.activeSceneChanged += OnSceneChange;

        UltimateLibrary.Managers.DrinkManager.RegisterDrink(
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

    void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        Logger.LogDebug( "Active Scene Changed." );
        Logger.LogDebug( $"Previous scene: {previousActiveScene.name} with PATH {previousActiveScene.path}" );
        Logger.LogDebug( $"New scene: {newActiveScene.name} with PATH {newActiveScene.path}" );

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
            var gameObj = topHeader.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "Bg":
                case "StatsButton": // missing notifications mostly
                case "IndicatorsButton": // bottom menu
                case "MoneyLabel":
                case "PrestigeLabel":
                case "GuestsCounter":
                case "FloorControlPanel": // move up/down floors
                case "TimeControlButtons":
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