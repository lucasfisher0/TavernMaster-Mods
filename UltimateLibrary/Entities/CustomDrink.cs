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

namespace UltimateLibrary.Entities;

/// <summary>
/// Contains all information for a custom drink.
/// </summary>
[Serializable]
public class CustomDrinkInfo : DrinksModel.DrinkInfo
{
    /// <summary>
    /// Drink Name
    /// </summary>
    public string drinkName { get; set; }

    /// <summary>
    /// Base refill cost
    /// </summary>
    public int refillCost { get; set; }

    /// <summary>
    /// Base price
    /// </summary>
    public int price { get; set; }

    /// <summary>
    /// Drink color
    /// </summary>
    public Color? customColor { get; set; }

    /// <summary>
    /// Wrapper to get the localized drink name
    /// </summary>
    /// <returns></returns>
    public string GetDrinkName()
    {
        return Localization.GetText( drinkName );
    }
}