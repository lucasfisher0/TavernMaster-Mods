using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( DrinkFillBar ) )]
public static class DrinkFillBarPatch
{
    [HarmonyPatch( "GetColorFromDrinkType" )]
    [HarmonyPrefix]
    public static bool GetColorFromDrinkType_Patch( DrinksModel.DrinkType drinkType, ref Color __result )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

        if ( UltimateBar.allDrinks.ContainsKey( (int)drinkType ) && UltimateBar.allDrinks[ (int)drinkType ].customColor.HasValue )
        {
            __result = UltimateBar.allDrinks[ (int)drinkType ].customColor.Value;
            return false;
        }
        else
        {
            __result = Color.green;
            Debug.LogError( $"UltimateBar: Could not find color for drink id {(int)drinkType}!" );
            return false;
        }
    }

    // void OpenRefillPopup() - May be needed if tutorial changes drink from beer
}

[HarmonyPatch( typeof( DrinkPricePopup ) )]
public static class DrinkPricePopupPatch
{
    [HarmonyPatch( "Refresh" )]
    [HarmonyPostfix]
    public static void Refresh_Patch( ref DrinkPricePopup __instance )
    {
        try
        {
            Text[] drinkNames = Traverse.Create( __instance ).Field( "drinkNames" ).GetValue<Text[]>();
            Text[] refillPrices = Traverse.Create( __instance ).Field( "refillPrices" ).GetValue<Text[]>();
            Text[] sellPrices = Traverse.Create( __instance ).Field( "sellPrices" ).GetValue<Text[]>();

            int i = drinkNames.Count();
            foreach( var drink in UltimateBar.allDrinks )
            {
                DrinksModel.DrinkType drinkType = (DrinksModel.DrinkType)drink.Key;
                drinkNames[i].text = DrinksModel.I.GetColoredDrinkName(drinkType, false);
                refillPrices[i].text = Economy.GetRefillCost(drinkType).ToString();
                sellPrices[i].text = Economy.GetDrinkPrice(drinkType).ToString();
                i++;
            }

            Traverse.Create( __instance ).Field( "drinkNames" ).SetValue( drinkNames );
            Traverse.Create( __instance ).Field( "refillPrices" ).SetValue( refillPrices );
            Traverse.Create( __instance ).Field( "sellPrices" ).SetValue( sellPrices );
        }
        catch ( Exception ex )
        {
            Debug.LogError( $"UltimateBar: DrinkPricePopup failed! {ex.Message}" );
        }
    }

    // void OpenRefillPopup() - May be needed if tutorial changes drink from beer
}
