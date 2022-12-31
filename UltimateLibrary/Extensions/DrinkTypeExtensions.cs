
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UltimateLibrary.Managers;

namespace UltimateLibrary.Extensions;

/// <summary>
/// Extensions for the <see cref="DrinksModel.DrinkType"/> enum.
/// </summary>
public static class DrinkTypeExtensions
{
    /// <summary>
    /// Determines whether this drink is included in the vanilla game.
    /// </summary>
    /// <param name="drinkType"></param>
    public static bool IsVanillaDrink(this DrinksModel.DrinkType drinkType)
    {
        return (int)drinkType <= DrinkManager.Instance.vanillaDrinkCount;
    }
}