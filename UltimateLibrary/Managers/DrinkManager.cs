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

namespace UltimateLibrary.Managers;

/// <summary>
/// Handles integration of custom drinks
/// </summary>
public partial class DrinkManager : IManager
{
    private static DrinkManager _instance;
    public static DrinkManager Instance => _instance ??= new DrinkManager();
    private DrinkManager() { }

    // Events
    // public static event Action OnItemsRegistered;

    // Drink Information

    /// <summary>
    /// Number of drinks present in the Vanilla game.
    /// </summary>
    public int vanillaDrinkCount { get; private set; }

    /// <summary>
    /// All custom drinks.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, CustomDrinkInfo> customDrinks = new();
    
    /// <summary>
    /// Registers a custom drink.
    /// </summary>
    /// <param name="newDrink"></param>
    /// <returns></returns>
    public void AddDrink( CustomDrinkInfo newDrink )
    {
        var index = Instance.vanillaDrinkCount + DrinkManager.Instance.customDrinks.Count + 1;
        newDrink.DrinkType = (DrinksModel.DrinkType)index;
        DrinkManager.Instance.customDrinks.Add( index, newDrink );
    }

    /// <summary>
    /// Adds drinks defined in a JSON file at given path, relative to BepInEx/plugins
    /// </summary>
    /// <param name="path">JSON file path, relative to BepInEx/plugins folder</param>
    public void AddDrinksFromJson( string path )
    {
        string json = AssetUtils.LoadText( path );

        if ( string.IsNullOrEmpty( json ) )
        {
            Logger.LogError( $"Failed to load drinks from invalid JSON: {path}" );
            Logger.LogError( json );
            return;
        }

        var drinks = JsonExtensions.DeserializeSingleOrList<CustomDrinkInfo>( json );
        if ( drinks.Any() )
        {
            foreach ( var drink in drinks )
            {
                
            }
        }
        else
        {
            Logger.LogWarning( "Attempted to load drinks from an empty or invalid JSON." );
        }
    }

    /// <summary>
    /// Registers all hooks.
    /// </summary>
    public void Init()
    {
        SetupVanillaInfo();
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( Patches ) );
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( BarPatches ) );
    }

    private void SetupVanillaInfo()
    {
        vanillaDrinkCount = (int)Enum.GetValues( typeof( DrinksModel.DrinkType ) )
                                     .Cast<DrinksModel.DrinkType>().Last();
    }

    private static class Patches
    {
        

        [HarmonyPatch( typeof( CustomerController ), "RememberPrices" ), HarmonyPrefix] // TODO: fix nameof?
        public static bool RememberPrices_Patch( ref CustomerController __instance )
        {
            try
            {
                var priceDic = Traverse.Create( __instance ).Field( "drinkTypeToPrice" ).GetValue<Dictionary<DrinksModel.DrinkType, int>>();
                foreach( var drink in DrinksModel.I.GetAvailableDrinks() )
                {
                    priceDic.Add( drink, Economy.GetDrinkPrice( drink ) );
                }
                Traverse.Create( __instance ).Field( "drinkTypeToPrice" ).SetValue( priceDic );
                
                return false; 
            }
            catch ( Exception ex )
            {
                Logger.LogError( $"Exception in RememberPrices. {ex.Message}" );
                return true;
            }
        }

        [HarmonyPatch( typeof( DrinkController ), nameof( DrinkController.SetData ) ), HarmonyPrefix]
        public static bool SetData_Patch( DrinksModel.DrinkType drinkType, bool isFull, bool adjustToHandPosition )
        {
            if ( (int)drinkType <= DrinkManager.Instance.vanillaDrinkCount )
                return true;

            // TODO: Custom cup visuals
            return true;
        }

        /* [HarmonyPatch( typeof( BartenderController ) )]
        public static class BartenderControllerPatch
        {
            // private IEnumerator FillSingleTypeDrink(DrinksModel.DrinkType drinkType, int customerId, List<OrdersModel.Order> allOrdersTaken)
        } */

        [HarmonyPatch( typeof( DrinksModel ), nameof( DrinksModel.AddNewBarInfo ) ), HarmonyPrefix]
        public static bool AddNewBarInfo_Prefix( int id )
        {
            // Add default drinks to new bar
            return true;
        }

        [HarmonyPatch( typeof( DrinksModel ), nameof( DrinksModel.AddNewBarInfo ) ), HarmonyPostfix]
        public static void AddNewBarInfo_Postfix( int id, ref DrinksModel __instance )
        {
            var barInfo = __instance.BarInfos.Where( x => x.Id == id ).FirstOrDefault();
            var customDrink = DrinkManager.Instance.customDrinks.FirstOrDefault().Value;
            if ( barInfo != null && customDrink != null)
            {
                customDrink.Amount = 100;
                customDrink.Capacity = 200;
                customDrink.BarrelId = 1;

                barInfo.UnlockedBarrels.Add(1);
                barInfo.DrinkInfos.Add( customDrink );
                __instance.BarInfos[0] = barInfo;
            }
        }
            
        // Does this work? GetAvailableDrinks
        // Does this work? GetBarrelIndexWithEnoughDrink
        // Does this work? HasAvailable
        // Does this work? IsBarrelUnlocked
        // Does this work? SetInitialState
        // Does this work? StealBarrel
        // GetColoredDrinkName - need to patch DrinkFillBar.GetColorFromDrinkType(drinkType)

        [HarmonyPatch( typeof( DrinksModel ), nameof( DrinksModel.DrinkTypeToName ) ), HarmonyPrefix]
        public static bool DrinkTypeToName_Patch( DrinksModel.DrinkType drinkType, ref string __result )
        {
            if ( drinkType.IsVanillaDrink() )
                return true;

            if ( DrinkManager.Instance.customDrinks.ContainsKey( (int)drinkType ) )
            {
                __result = DrinkManager.Instance.customDrinks[(int)drinkType].GetDrinkName();
                return false;
            }
            else
            {
                Logger.LogError( $"Could not find name for drink id {(int)drinkType}!" );
                return false;
            }
        }

        [HarmonyPatch( typeof( Economy ), nameof( Economy.GetRefillCost ) ), HarmonyPrefix]
        public static bool GetRefillCost_Patch( DrinksModel.DrinkType drinkType, ref int __result )
        {
            if ( drinkType.IsVanillaDrink() )
                return true;

            if ( DrinkManager.Instance.customDrinks.ContainsKey( (int)drinkType ) )
            {
                __result = DrinkManager.Instance.customDrinks[ (int)drinkType ].refillCost;
                return false;
            }
            else
            {
                Logger.LogError( $"Could not find refill cost for drink id {(int)drinkType}!" );
                return false;
            }
        }

        [HarmonyPatch( typeof( Economy ), "GetDrinkPriceBase" ), HarmonyPrefix] // TODO: Fix nameof
        public static bool GetDrinkPriceBase_Patch( DrinksModel.DrinkType drinkType, ref int __result )
        {
            if ( drinkType.IsVanillaDrink() )
                return true;

            if ( DrinkManager.Instance.customDrinks.ContainsKey( (int)drinkType ) )
            {
                __result = DrinkManager.Instance.customDrinks[ (int)drinkType ].price;
                return false;
            }
            else
            {
                Logger.LogError( $"Could not find price for drink id {(int)drinkType}!" );
                return false;
            }
        }

        [HarmonyPatch( typeof( DrinkFillBar ), nameof( DrinkFillBar.GetColorFromDrinkType) ), HarmonyPrefix]
        public static bool GetColorFromDrinkType_Patch( DrinksModel.DrinkType drinkType, ref Color __result )
        {
            if ( drinkType.IsVanillaDrink() )
                return true;

            if ( DrinkManager.Instance.customDrinks.ContainsKey( (int)drinkType ) && DrinkManager.Instance.customDrinks[ (int)drinkType ].customColor.HasValue )
            {
                __result = DrinkManager.Instance.customDrinks[ (int)drinkType ].customColor.Value;
                return false;
            }
            else
            {
                __result = Color.green;
                Logger.LogError( $"Could not find color for drink id {(int)drinkType}!" );
                return false;
            }
        }

        // void DrinkFillBar::OpenRefillPopup() - May be needed if tutorial changes drink from beer

        [HarmonyPatch( typeof( DrinkPricePopup ), "Refresh" ), HarmonyPrefix] // TODO: fix nameof
        public static void Refresh_Patch( ref DrinkPricePopup __instance )
        {
            try
            {
                Text[] drinkNames = Traverse.Create( __instance ).Field( "drinkNames" ).GetValue<Text[]>();
                Text[] refillPrices = Traverse.Create( __instance ).Field( "refillPrices" ).GetValue<Text[]>();
                Text[] sellPrices = Traverse.Create( __instance ).Field( "sellPrices" ).GetValue<Text[]>();

                int i = drinkNames.Count();
                foreach( var drink in DrinkManager.Instance.customDrinks )
                {
                    DrinksModel.DrinkType drinkType = (DrinksModel.DrinkType)drink.Key;
                    drinkNames[i].text = DrinksModel.I.GetColoredDrinkName(drinkType, false);
                    refillPrices[i].text = Economy.GetRefillCost(drinkType).ToString();
                    sellPrices[i].text = Economy.GetDrinkPrice(drinkType).ToString();
                    i++;
                }

                Traverse.Create( __instance ).Field( "drinkNames" ).SetValue( drinkNames );
                Traverse.Create( __instance ).Field( "refillPrices" ).SetValue( refillPrices );
                Traverse.Create( __instance ).Field( "sellPrices" ).SetValue( sellPrices );
            }
            catch ( Exception ex )
            {
                Logger.LogError( $"DrinkPricePopup failed! {ex.Message}" );
            }
        }

        /*
        // TODO: Is this patch needed or is data just invalid?
        [HarmonyPatch( typeof( BarController ), nameof( BarController.UpdateNotification ) ), HarmonyPrefix] 
        public static bool UpdateNotification_Prefix( ref BarController __instance, bool isNormalMode, bool isTransitionInProgress )
        {
            
            var icon = Traverse.Create( __instance ).Field( "notificationIcon" ).GetValue<SpriteRenderer>();
            ( (PropController) __instance ).UpdateNotification( isNormalMode, isTransitionInProgress );
    
            if ( !isNormalMode || EventHelper.IsEmergencyInProgress || __instance.IsBurnt || isTransitionInProgress )
            {
                icon.gameObject.SetActive( false );
                return false;
            }

            bool active = false;
            for ( int i = 0; i < __instance.BarInfo.DrinkInfos.Count; i++ )
            {
                if ( __instance.BarInfo.DrinkInfos[i].Amount == 0 )
                {
                    active = true;
                    break;
                }
            }
            icon.gameObject.SetActive( active );
            return false;
        }
        */

        // void DrinkPricePopup::OpenRefillPopup() - May be needed if tutorial changes drink from beer
    }
}