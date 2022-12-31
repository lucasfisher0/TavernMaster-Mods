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

namespace UltimateLibrary.Old;

public interface ICustomModel : ISavableModel
{
    // string 
    
    void Init( AllSavableModels models );
}

static class ModelHooks
{
    public static List<Type> ModelTypes;
    public static List<ICustomModel> CustomModels;
    public static UnityEvent<ICustomModel, Type> OnModelCreated;
   
    [HarmonyPatch( typeof(AllSavableModels), "AllSavableModels" )]
    [HarmonyPostfix]
    static void Constructor_Post( ref AllSavableModels __result )
    {
        Debug.Log( "AllSavableModels constructor hook!" );
        foreach ( var model in CustomModels )
        {
            model.Init( __result );
        }
    }

    [HarmonyPatch( typeof(AllSavableModels), "CreateNewModels" )]
    [HarmonyPostfix]
    static void CreateNewModels_Post( AllSavableModels __instance )
    {
        Debug.Log( "CreateNewModels hook!" );
        CustomModels = new();
        foreach ( var type in ModelTypes )
        {
            var model = (ICustomModel)Activator.CreateInstance(type);
            model.Init( __instance );
            OnModelCreated.Invoke( model, type );
            CustomModels.Add( model );
        }
    }
}