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


using TablePositionData = TavernData.TablePositionData;
using PropState = TavernData.PropState;
using PropType = TavernData.PropType;

using FurnitureColor = TavernData.FurnitureColor;
using StructureType = TavernData.StructureType;
using WallType = TavernData.WallType;
using WallMeshType = TavernData.WallMeshType;
using WallPaintType = TavernData.WallPaintType;
using FloorType = TavernData.FloorType;
using WallData = TavernData.WallData;
using FloorData = TavernData.FloorData;
using BedPriceData = TavernData.BedPriceData;


namespace UltimateLibrary.Managers;

/// <summary>
/// Handles custom props and structures
/// </summary>
public class PropManager : IManager
{
    private static PropManager _instance;
    public static PropManager Instance => _instance ??= new PropManager();
    private PropManager() { }

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