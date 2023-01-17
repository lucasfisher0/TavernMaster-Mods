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

using StaffType = TavernData.StaffType;
using ResponsibilityEnum = TavernData.ResponsibilityEnum;
using ChefPriorityEnum = TavernData.ChefPriorityEnum;
using ResponsibilityPriority = TavernData.ResponsibilityPriority;

using ResponsibilityItem = TavernData.ResponsibilityItem;
using ChefPriorityItem = TavernData.ChefPriorityItem;
using TraitEnum = TavernData.TraitEnum;

using StatType = TavernData.StatType;
using Stat = TavernData.Stat;

using StaffInfoTemplate = TavernData.StaffInfoTemplate;
using StaffInfo = TavernData.StaffInfo;
using GuardInfo = TavernData.GuardInfo;

// StaffBase : AiAgentBase

namespace UltimateLibrary.Managers;

/// <summary>
/// Handles integration of employee logic
/// </summary>
public class EmployeeManager : IManager
{
    private static EmployeeManager _instance;
    public static EmployeeManager Instance => _instance ??= new EmployeeManager();
    private EmployeeManager() { }

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