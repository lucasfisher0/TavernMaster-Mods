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
public class SaveManager : IManager
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance ??= new SaveManager();
    private SaveManager() { }

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
        [HarmonyPatch( typeof( TavernController ), nameof( TavernController.SaveGame ) ), HarmonyPrefix]
        public static bool Save_Prefix()
        {
            SettingsModel.I.Save();
            CustomSavableModel.CustomI.Save( isAutoSave: true);

            return false;
        }

        // LoadGameListController.RefreshList()
        // LoadGameListController.LoadGame()

        // AllSavableModels.GetAllSaveFiles()
    }
}