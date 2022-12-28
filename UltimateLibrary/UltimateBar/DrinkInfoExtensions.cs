using System;

namespace UltimateLibrary.UltimateBar;

public static class DrinkInfoExtensions
{
    public static bool IsVanillaDrink( this DrinksModel.DrinkType drinkType )
    {
        return ( int )drinkType <= UltimateBar.vanillaDrinkCount;
    }
}
