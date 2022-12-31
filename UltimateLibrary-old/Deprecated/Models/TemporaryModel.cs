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

namespace UltimateLibrary.Models.Old;

public static class CustomTemporaryModel
{
    // TODO extra emergency types
}

[HarmonyPatch]
public static class TemporaryModelPatches
{
    // [HarmonyPatch( typeof( TemporaryModel ), "Save" )]
    // [HarmonyPrefix]
    // static void SaveCurrentGame( bool isAutoSave, string customName = null )
    // {
    // 
    //    
    // }
}