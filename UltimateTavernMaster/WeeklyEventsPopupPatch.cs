using System;
using HarmonyLib;
using UnityEngine;

namespace UltimateTavernMaster;

public static class UltimateUIPatches
{
    private static Sprite wideUi = Resources.Load<Sprite>( "wideUiWood" );

    [HarmonyPatch( typeof( WeeklyEventsPopup ), nameof( WeeklyEventsPopup.Setup ) ), HarmonyPostfix]
    public static void Setup_Patch( ref WeeklyEventsPopup __instance )
    {
        var imageComponent = __instance.GetComponent<UnityEngine.UI.Image>();
        if ( imageComponent != null && imageComponent.sprite != wideUi )
        {
            imageComponent.sprite = wideUi;
        }
    }
}