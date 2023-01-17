using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UltimateLibrary.Interfaces;
using UltimateLibrary.Utility;
using UltimateLibrary.Entities;
using UltimateLibrary.Extensions;

namespace UltimateLibrary.Managers;

public partial class DrinkManager : IManager
{
    /// <summary>
    /// Contains patches for all bar-relateed classes.
    /// Ex. BarController, BarShopController, BarTooltipController
    /// </summary>
    private static class BarPatches
    {
        [HarmonyPatch( typeof( BarShopController ), "OnBuy" ), HarmonyPrefix] // TODO: fix nameof?
        public static bool OnBuy_Patch( VisualSettings.BarrelPropInfo propInfo )
        {
            //TODO: Custom barrel setup
            return true; 
        }

        [HarmonyPatch( typeof( BarController ), nameof( BarController.SetLiquid ) ), HarmonyPrefix]
        public static bool SetLiquid_Patch( bool isActive, DrinksModel.DrinkType drinkType )
        {
            if ( (int)drinkType <= DrinkManager.Instance.vanillaDrinkCount )
                return true;

            // TODO: Custom liquid texture
            isActive = false;
            return true;
        }

        /*
        // TODO: Is this patch needed or is data just invalid?
        [HarmonyPatch( typeof( BarController ), nameof( BarController.UpdateNotification ) ), HarmonyPrefix] 
        public static bool UpdateNotification_Prefix( ref BarController __instance, bool isNormalMode, bool isTransitionInProgress )
        {
            
            var icon = Traverse.Create( __instance ).Field( "notificationIcon" ).GetValue<SpriteRenderer>();
            ( (PropController) __instance ).UpdateNotification( isNormalMode, isTransitionInProgress );
    
            if ( !isNormalMode || EventHelper.IsEmergencyInProgress || __instance.IsBurnt || isTransitionInProgress )
            {
                icon.gameObject.SetActive( false );
                return false;
            }

            bool active = false;
            for ( int i = 0; i < __instance.BarInfo.DrinkInfos.Count; i++ )
            {
                if ( __instance.BarInfo.DrinkInfos[i].Amount == 0 )
                {
                    active = true;
                    break;
                }
            }
            icon.gameObject.SetActive( active );
            return false;
        }
        */

        [HarmonyPatch( typeof( BarTooltipController ), nameof( BarTooltipController.ShowTooltip ) ), HarmonyPrefix]
        public static bool ShowTooltip_Prefix( ref BarTooltipController __instance, DrinksModel.BarInfo barInfo )
        {
            return false;

            // TODO: this is not needed, the tooltip fillBars should be updated when drinks are changed.
            /*
            var showNow = Traverse.Create( __instance ).Field( "showNow" ); // bool
            var fillBars = Traverse.Create( __instance ).Field( "fillBars" ); // DrinkFillBar[]

            if ( TutorialModel.I.IsFinished && barInfo != null )
            {
                __instance.showNow = true;
                rect.gameObject.SetActive(value: true);
                for (int i = 0; i < barInfo.DrinkInfos.Count; i++)
                {
                    fillBars[i].gameObject.SetActive(value: true);
                    fillBars[i].SetData(barInfo.DrinkInfos[i], barInfo, isClickable: false);
                }
                for (int j = barInfo.DrinkInfos.Count; j < fillBars.Length; j++)
                {
                    fillBars[j].gameObject.SetActive(value: false);
                }
                Resize(barInfo.DrinkInfos.Count);
            }

            return false;

            */
        }
    }
}