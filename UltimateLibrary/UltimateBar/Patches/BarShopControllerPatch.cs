using System;
using HarmonyLib;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( BarShopController ) )]
public static class BarShopControllerPatch
{
    [HarmonyPatch( "OnBuy" )]
    [HarmonyPrefix]
    public static bool OnBuy_Patch( VisualSettings.BarrelPropInfo propInfo )
    {
        //TODO: Custom barrel setup
        return true; 
    }
}