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

namespace UltimateLibrary.Interfaces;

public enum ModelTypes
{
    Tavern,
    Drinks,
    Foods,
    Events,
    TechTree,
    Structure,
    Staff,
    Stats,
    Quests,
    Adventures,
    Time,
    Custom
}

public enum ModelExtensionType
{
    Addition,
    Modification,
    Replacement,
    Custom
}

public interface IUltimateModel
{
    public ModelTypes ModelType { get; }

    public ModelExtensionType ExtensionType { get; }
    
    public bool Init( AllSavableModels models );
}