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


using CustomerType = CustomerController.CustomerType;

namespace UltimateLibrary.Managers;

/// <summary>
/// Handles customer features and changes
/// </summary>
public class CustomerManager : IManager
{
    private static CustomerManager _instance;
    public static CustomerManager Instance => _instance ??= new CustomerManager();
    private CustomerManager() { }

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
        [HarmonyPatch( typeof( VisualSettings ), nameof( VisualSettings.GetCustomerPrefab ) ), HarmonyPrefix]
        public static bool GetCustomerPrefab_Prefix( CustomerType customerType )
        {
            // TODO: Hook here to add extra customer prefabs
            return true;
        }
    }
}