using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Bootstrap;
using UltimateLibrary.Interfaces;
using UltimateLibrary.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UltimateLibrary.Entities;

/// <summary>
/// Contains mod-meta data to include in saves.
/// TODO: make this a wrapper for AllSavableModels
/// </summary>
[Serializable]
public class CustomSavableModel : AllSavableModels
{
    public static CustomSavableModel CustomI;

    public CustomSavableModel( string saveFileName, bool isSandbox = false )
        : base(saveFileName, isSandbox)
    {
        CustomI = this;
    }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, BaseUnityPlugin> activePlugins;
}