using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( CustomerController ) )]
public static class CustomerControllerPatch
{
    [HarmonyPatch( "RememberPrices" )]
    [HarmonyPrefix]
    public static bool RememberPrices_Patch( ref CustomerController __instance )
    {
        try
        {
            var priceDic = Traverse.Create( __instance ).Field( "drinkTypeToPrice" ).GetValue<Dictionary<DrinksModel.DrinkType, int>>();
            foreach( var drink in DrinksModel.I.GetAvailableDrinks() )
            {
                priceDic.Add( drink, Economy.GetDrinkPrice( drink ) );
            }
            Traverse.Create( __instance ).Field( "drinkTypeToPrice" ).SetValue( priceDic );
            
            return false; 
        }
        catch ( Exception ex )
        {
            Debug.LogError( $"UltimateBar: Exception in RememberPrices. {ex.Message}" );
            return true;
        }
    } 
}