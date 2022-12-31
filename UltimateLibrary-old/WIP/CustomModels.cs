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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UltimateLibrary.Models;

/*
static class CustomModelsUltimate
{
    public static List<Type> ModelTypes;
    public static List<IUltimateModel> CustomModels;
    
    public static UnityEvent<IUltimateModel, Type> OnModelCreated;
    public static UnityEvent<IUltimateModel, Type> OnModelInitialized;
    public static UnityEvent<IUltimateModel, Type> OnModelSaved;
    public static UnityEvent<IUltimateModel, Type> OnModelDestroyed;

    public static void InitializeModels( ref AllSavableModels allModels )
    {
        Debug.Log( "Initializing custom models." );
        foreach ( var model in CustomModels )
        {
            model.Init( allModels );
        }
    }

    public static void CreateNewModels( ref AllSavableModels allModels )
    {
        Debug.Log( "Creating new custom models!" );
        CustomModels = new();
        foreach ( var type in ModelTypes )
        {
            var model = (IUltimateModel)Activator.CreateInstance(type);
            model.Init( allModels );
            OnModelInitialized.Invoke( model, type );
            CustomModels.Add( model );
        }
    }

    public static void SaveCustomModels()
    {
        Debug.Log( "Creating new custom models!" );
        CustomModels = new();
        foreach ( var type in ModelTypes )
        {
            var model = (IUltimateModel)Activator.CreateInstance(type);
            model.Init( allModels );
            OnModelInitialized.Invoke( model, type );
            CustomModels.Add( model );
        }
    }
}
*/