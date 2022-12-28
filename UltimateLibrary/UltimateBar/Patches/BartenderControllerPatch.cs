using System;
using HarmonyLib;

namespace UltimateLibrary.UltimateBar;

[HarmonyPatch( typeof( BartenderController ) )]
public static class BartenderControllerPatch
{
    // private IEnumerator FillSingleTypeDrink(DrinksModel.DrinkType drinkType, int customerId, List<OrdersModel.Order> allOrdersTaken)
}