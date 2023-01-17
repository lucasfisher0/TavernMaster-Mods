using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UltimateLibrary.Interfaces;
using UltimateLibrary.Utility;
using UltimateLibrary.Entities;
using UltimateLibrary.Extensions;

using PartyType = TavernData.PartyType;
using PartyData = TavernData.PartyData;

namespace UltimateLibrary.Managers;

// TODO: Custom tech tree
// allSavableModels.TechTreeModelBase

/// <summary>
/// Handles integration for custom events and parties
/// </summary>
public class TechTreeManager : IManager
{
    private static TechTreeManager _instance;
    public static TechTreeManager Instance => _instance ??= new TechTreeManager();
    private TechTreeManager() { }

    // Events
    // public static event Action OnItemsRegistered;

    /// <summary>
    /// Registers all hooks.
    /// </summary>
    public void Init()
    {
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( Patches ) );
    }

    private static class Patches
    {
    }
}