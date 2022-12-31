using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;

namespace UltimateLibrary.Models;

[HarmonyPatch]
public static class ModelSavePatches
{
    // internal static Dictionary<string, IModCanSave> allModData = new Dictionary<string, IModCanSave>();

    [HarmonyPatch( typeof( AllSavableModels ), "Save" )]
    [HarmonyPrefix]
    static bool Save_Prefix( ref AllSavableModels __instance, bool isAutoSave, string customName = null )
    {
        // replace TemporaryModel.I.IsFireInProgress
        return true;
    }

    [HarmonyPatch( typeof( AllSavableModels ), "SaveToDevice" )]
    [HarmonyPrefix]
    static bool SaveToDevice_Prefix( ref AllSavableModels __instance, string json, bool isAutoSave, string customName )
    {
        string text = Application.persistentDataPath + "/" + AllSavableModels.CustomSaveFolderName;
		if (!isAutoSave && !Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = string.Format(AllSavableModels.SaveFileNameFormat, customName);
		string text3 = Application.persistentDataPath + (isAutoSave ? ("/" + AllSavableModels.AutoSaveFileName + ".json") : string.Concat(new string[]
		{
			"/",
			AllSavableModels.CustomSaveFolderName,
			"/",
			text2,
			"-mod.json"
		}));
		File.Delete(text3);
		File.WriteAllLines(text3, new List<string> { json } );


        return true;
    }


    [HarmonyPatch( typeof(AllSavableModels), "AllSavableModels" )]
    [HarmonyPostfix]
    static void Constructor_Post( ref AllSavableModels __result )
    {
        CustomModelHandler.InitializeModels( ref __result );
    }

    [HarmonyPatch( typeof(AllSavableModels), "CreateNewModels" )]
    [HarmonyPostfix]
    static void CreateNewModels_Post( ref AllSavableModels __instance )
    {
        Debug.Log( "CreateNewModels hook!" );
        CustomModelHandler.CreateNewModels( ref __instance );
        
    }
}
*/