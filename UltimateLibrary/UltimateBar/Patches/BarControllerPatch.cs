using System;
using HarmonyLib;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( BarController ) )]
public static class BarControllerPatch
{

    [HarmonyPatch( "SetLiquid" )]
    [HarmonyPrefix]
    public static bool SetLiquid_Patch( bool isActive, DrinksModel.DrinkType drinkType )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

        // TODO: Custom liquid texture
        isActive = false;
        return true;
    }
}