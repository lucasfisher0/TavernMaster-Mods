using System;
using System.IO;
using UnityEngine;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using TMPro;

namespace UltimateLibrary.Utility;

/// <summary>
/// Util functions for creating game menus.
/// </summary>
public static class InterfaceUtils
{
    /// <summary>
    /// Create a blank UI object.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    public static GameObject CreateUiObject( string name, Transform parent = null)
    {
        var obj = new GameObject( name, new Type[] { typeof( RectTransform ), typeof( CanvasRenderer ) } );
        if ( parent != null )
        {
            obj.transform.SetParent( parent );
        }

        obj.transform.localScale = Vector3.oneVector;
        return obj;
    }

    /// <summary>
    /// Adds a text label with localization to any game object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="key"></param>
    /// <param name="parameters"></param>
    public static void AddLocalizedLabel( GameObject obj, string key, string[] parameters = null )
    {
        // Text Component
        var textComp = obj.AddComponent<TextMeshProUGUI>();
        textComp.alignment = TextAlignmentOptions.MidlineLeft;
        // material should be BMYEONSUGN_ttf SDF Material
        // textComp.alpha = 1;
        // textComp.characterSpacing = 0;
        // textComp.characterWidthAdjustment = 0;
        // textComp.color = new Color( 0.6902f, 0.6902f, 0.6902f, 1f );
        // TODO set material

        // Label Component
        var labelComp = obj.AddComponent<LocalizedLabel>();
        Traverse.Create( labelComp ).Field( "key" ).SetValue( key );
        Traverse.Create( labelComp ).Field( "parameters" ).SetValue( parameters );
        Traverse.Create( labelComp ).Method( "Refresh" ).GetValue();
    }
}