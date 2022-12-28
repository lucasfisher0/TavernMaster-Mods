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

namespace UltimateLibrary.UltimateBar;

public static class UltimateBar
{
    public static int vanillaDrinkCount = 19;
    public static Dictionary<int, CustomDrinkInfo> allDrinks = new();
    
    public static void RegisterDrink( CustomDrinkInfo newDrink )
    {
        foreach( var loc in newDrink.localizedNames )
        {
            Localization.UpsertText( loc.Key, newDrink.drinkName, loc.Value );
        }

        var index = vanillaDrinkCount + allDrinks.Count + 1;
        newDrink.DrinkType = (DrinksModel.DrinkType)index;
        allDrinks.Add( index, newDrink );
    }
}

[Serializable]
public class CustomDrinkInfo : DrinksModel.DrinkInfo
{
    public string drinkName;
    public Dictionary<LocalizationModel.LanguageType, string> localizedNames;
    
    public int refillCost;
    public int price;

    public Color? customColor;

    public string GetDrinkName()
    {
        return Localization.GetText( drinkName );
    }
}