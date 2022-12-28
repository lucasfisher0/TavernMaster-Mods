using System;
using HarmonyLib;
using UnityEngine;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( Economy ) )]
public static class EconomyPatch
{

    [HarmonyPatch( "GetRefillCost" )]
    [HarmonyPrefix]
    public static bool GetRefillCost_Patch( DrinksModel.DrinkType drinkType, ref int __result )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

        if ( UltimateBar.allDrinks.ContainsKey( (int)drinkType ) )
        {
            __result = UltimateBar.allDrinks[ (int)drinkType ].refillCost;
            return false;
        }
        else
        {
            Debug.LogError( $"UltimateBar: Could not find refill cost for drink id {(int)drinkType}!" );
            return false;
        }
    }

    [HarmonyPatch( "GetDrinkPriceBase" )]
    [HarmonyPrefix]
    public static bool GetDrinkPriceBase_Patch( DrinksModel.DrinkType drinkType, ref int __result )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

        if ( UltimateBar.allDrinks.ContainsKey( (int)drinkType ) )
        {
            __result = UltimateBar.allDrinks[ (int)drinkType ].price;
            return false;
        }
        else
        {
            Debug.LogError( $"UltimateBar: Could not find price for drink id {(int)drinkType}!" );
            return false;
        }
    }
}