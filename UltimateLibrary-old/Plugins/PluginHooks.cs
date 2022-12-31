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
using UnityEngine.SceneManagement;

using UltimateLibrary.Plugins;

namespace UltimateLibrary;

public partial class Plugin : BaseUnityPlugin, IUltimateLibrary
{
    void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        if ( newActiveScene.name == "MainMenu" )
        {
            foreach ( var plugin in plugins )
            {
                if ( !plugin.Value.IsLoaded() && plugin.Value.GetLoadType() == PluginLoadType.Autoload )
                {
                    var success = plugin.Value.Load( this );
                    if ( !success )
                    {
                        continue;
                    }
                }
                
                var hooks = plugin.Value.GetAllHooks();
                foreach ( var hook in hooks )
                {
                    // todo trigger hooks                    
                }
            }



            // Logger.LogDebug( "Patching main menu..." );
            // var ui = GameObject.Find( "MainMenuUI" );
            // for ( int i = 0; i < ui.transform.childCount; i++ )
            // {
            //     var gameObj = ui.transform.GetChild( i ).gameObject;
            //     switch ( gameObj.name )
            //     {
            //         case "ButtonsPanel":
            //         case "Logo (2)":
            //         case "DiscordButton":
            //         case "QqButton":
            //         case "JoinCommunity":
            //         case "NewUpdatePanel":
            //         case "PopupHolder":
            //             break;
            //         default:
            //             Logger.LogDebug( $"Main menu object has no hook: {gameObj.name}" );
            //             break;
            //     }
// 
            // }
        }
        else if ( newActiveScene.name == "Tavern0" )
        {
            // Logger.LogDebug( "Patching game menu..." );
            // var ui = GameObject.Find( "GameUI" );
            // for ( int i = 0; i < ui.transform.childCount; i++ )
            // {
            //     var gameObj = ui.transform.GetChild( i ).gameObject;
            //     switch ( gameObj.name )
            //     {
            //         case "TopHeader":
            //             HeaderTweaks( gameObj );
            //             break;
            //         case "SeatIconsParent":
            //         case "IconTooltip":
            //         case "DragArea":
            //         case "SelectOutline":
            //         case "PropUI":
            //         case "ThirdPersonUIRenderer":
            //         case "PropShop": // buy furniture
            //         case "BarShop": // buy barrels
            //         case "StructureShop": // building
            //         case "NotificationCenter": // missing notifications mostly
            //         case "PathfindingHeatmapLegend":
            //         case "FireButtons":
            //         case "LaunchPad": // bottom menu
            //         case "LowerRightGroup":
            //         case "TutorialMessage":
            //         case "TutorialHand":
            //         case "PopupHolder":
            //         case "TutorialMessageForPopups":
            //         case "TutorialHandForPopups":
            //             break;
            //         default:
            //             Logger.LogDebug( $"In-game menu object has no hook: {gameObj.name}" );
            //             break;
            //     }
// 
            // }

        }
    }
}