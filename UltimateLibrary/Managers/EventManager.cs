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

/// <summary>
/// Handles integration for custom events and parties
/// </summary>
public class EventManager : IManager
{
    private static EventManager _instance;
    public static EventManager Instance => _instance ??= new EventManager();
    private EventManager() { }

    // Events
    // public static event Action OnItemsRegistered;

    /// <summary>
    /// Registers all hooks.
    /// </summary>
    public void Init()
    {
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( Patches ) );
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( EventHelper.Patches ) );
    }

    private static class Patches
    {
    }
}

public static class EventHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public static bool IsEmergencyInProgress
    {
        get
        {
            return TemporaryModel.I.IsFireInProgress;
        }
    }

    // TODO: replace all TemporaryModel.I.IsFireInProgress checks
    public static class Patches
    {

    }
}