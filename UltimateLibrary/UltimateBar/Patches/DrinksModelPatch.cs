using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( DrinksModel ) )]
public static class DrinksModelPatch
{
    [HarmonyPatch( "AddNewBarInfo" )]
    [HarmonyPrefix]
    public static bool AddNewBarInfo_Prefix( int id )
    {
        // Add default drinks to new bar
        return true;
    }

    [HarmonyPatch( "AddNewBarInfo" )]
    [HarmonyPostfix]
    public static void AddNewBarInfo_Postfix( int id, ref DrinksModel __instance )
    {
        var barInfo = __instance.BarInfos.Where( x => x.Id == id ).FirstOrDefault();
        var customDrink = UltimateBar.allDrinks.FirstOrDefault().Value;
        if ( barInfo != null && customDrink != null)
        {
            customDrink.Amount = 100;
            customDrink.Capacity = 200;
            customDrink.BarrelId = 1;

            barInfo.UnlockedBarrels.Add(1);
			barInfo.DrinkInfos.Add( customDrink );
            __instance.BarInfos[0] = barInfo;
        }
    }
    
    // Does this work? GetAvailableDrinks
    // Does this work? GetBarrelIndexWithEnoughDrink
    // Does this work? HasAvailable
    // Does this work? IsBarrelUnlocked
    // Does this work? SetInitialState
    // Does this work? StealBarrel
    // GetColoredDrinkName - need to patch DrinkFillBar.GetColorFromDrinkType(drinkType)

    [HarmonyPatch( "DrinkTypeToName" )]
    [HarmonyPrefix]
    public static bool DrinkTypeToName_Patch( DrinksModel.DrinkType drinkType, ref string __result )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

         if ( UltimateBar.allDrinks.ContainsKey( (int)drinkType ) )
        {
            __result = UltimateBar.allDrinks[(int)drinkType].GetDrinkName();
            return false;
        }
        else
        {
            Debug.LogError( $"UltimateBar: Could not find name for drink id {(int)drinkType}!" );
            return false;
        }
    }
}
