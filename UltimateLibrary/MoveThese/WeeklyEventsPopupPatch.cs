using System;
using HarmonyLib;
using UnityEngine;

namespace UltimateLibrary;

[HarmonyPatch( typeof( WeeklyEventsPopup ) )]
public static class WeeklyEventsPopupPatch
{
    private static Sprite wideUi = Resources.Load<Sprite>( "wideUiWood" );

    [HarmonyPatch( "Setup" )]
    [HarmonyPostfix]
    public static void Setup_Patch( ref WeeklyEventsPopup __instance )
    {
        var imageComponent = __instance.GetComponent<UnityEngine.UI.Image>();
        if ( imageComponent != null && imageComponent.sprite != wideUi )
        {
            imageComponent.sprite = wideUi;
        }
    }
}