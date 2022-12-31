using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace UltimateLibrary.Hooks;

static class SpawnerHooks
{   
    public static UnityEvent<GameObject> OnAdventurerSpawn = new();
    public static UnityEvent<GameObject, bool> OnCustomerSpawn = new();
    public static UnityEvent<ThiefController> OnThiefSpawn = new();
    public static UnityEvent<GuardController> OnGuardSpawn = new();

   
    [HarmonyPatch( typeof(ArmySpawner), "LoadFromPool" )]
    [HarmonyPostfix]
    static void AdventurerSpawn_Post( ref GameObject __result )
    {
        OnAdventurerSpawn.Invoke( __result );
    }
   
    [HarmonyPatch( typeof(CustomerSpawner), "PickPotentialHotelCustomer" )]
    [HarmonyPostfix]
    static void HotelCustomerSpawn_Post( ref GameObject __result )
    {
        OnCustomerSpawn.Invoke( __result, true );
    }

    [HarmonyPatch( typeof(ThiefSpawner), "SpawnThief" )]
    [HarmonyPostfix]
    static void ThiefSpawn_Post( ref ThiefController __result )
    {
        OnThiefSpawn.Invoke( __result );
    }
    [HarmonyPatch( typeof(ThiefSpawner), "SpawnGuard" )]
    [HarmonyPostfix]
    static void GuardSpawn_Post( ref GuardController __result )
    {
        OnGuardSpawn.Invoke( __result );
    }
}