using System;
using HarmonyLib;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( DrinkController ) )]
public static class DrinkControllerPatch
{

    [HarmonyPatch( "SetData" )]
    [HarmonyPrefix]
    public static bool SetData_Patch( DrinksModel.DrinkType drinkType, bool isFull, bool adjustToHandPosition )
    {
        if ( (int)drinkType <= UltimateBar.vanillaDrinkCount )
            return true;

        // TODO: Custom cup visuals
        return true;
    }
}