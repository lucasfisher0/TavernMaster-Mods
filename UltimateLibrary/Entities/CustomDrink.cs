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
/// 
/// </summary>
[Serializable]
public class CustomDrinkInfo : DrinksModel.DrinkInfo
{
    /// <summary>
    /// Drink Name
    /// </summary>
    public string drinkName;

    /// <summary>
    /// Localized names, defaults to Drink Name
    /// </summary>
    public Dictionary<LocalizationModel.LanguageType, string> localizedNames;
    
    /// <summary>
    /// Base refill cost
    /// </summary>
    public int refillCost;

    /// <summary>
    /// Base price
    /// </summary>
    public int price;

    /// <summary>
    /// Drink color
    /// </summary>
    public Color? customColor;

    /// <summary>
    /// Wrapper to get the localized drink name
    /// </summary>
    /// <returns></returns>
    public string GetDrinkName()
    {
        return Localization.GetText( drinkName );
    }
}