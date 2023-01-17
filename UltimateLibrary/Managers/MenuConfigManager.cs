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
using BepInEx.Configuration;

namespace UltimateLibrary.Managers;

/// <summary>
/// Handles integration of configuration menus for mods
/// </summary>
public class MenuConfigManager : IManager
{
    private MenuConfigManager() { }
    private static MenuConfigManager _instance;
    public static MenuConfigManager Instance => _instance ??= new MenuConfigManager();

    private static Dictionary<string, ConfigFile> configs;
    public static int holderIndex;

    /// <summary>
    /// Registers all hooks.
    /// </summary>
    public void Init()
    {
        UltimateLibraryPlugin.Harmony.PatchAll( typeof( Patches ) );

        UltimateLibraryPlugin.OnSceneMainMenu += RefreshPlugins;
    }

    internal void RefreshPlugins( bool isInitialLoad )
    {
        // Load plugin configs
        if ( isInitialLoad )
        {
            var updatedConfigs = new Dictionary<string, ConfigFile>();
            var plugins = BepInExUtils.GetDependentPlugins( true );
            foreach ( var plugin in plugins )
            {
                Logger.LogDebug( $"Registering config file for {plugin.Key}." );
                updatedConfigs.Add( plugin.Key, plugin.Value.Config );
            }

            configs = updatedConfigs;
        }

        if ( configs == null )
        {
            Logger.LogError( "Configurations was empty!" );
        }        
    }

    public static void ModTabClicked()
    {
        var mainPopup = GameObject.Find( "Canvas/MainMenuUI/PopupHolder/SettingsPopup(Clone)" );
        var settingsComp = mainPopup.GetComponent<SettingsPopup>();

        settingsComp.OnTabClick( holderIndex );
    }

    private static class Patches
    {
        [HarmonyPatch( typeof( SettingsPopup ), nameof( SettingsPopup.Setup ) ), HarmonyPostfix]
        public static void SettingsPopup_Postfix( ref SettingsPopup __instance, bool isMainMenu )
        {
            if ( configs == null )
            {
                Logger.LogWarning( "No configurations were found to create a settings menu!" );
                return;
            }
            
            var mainPopup = GameObject.Find( "Canvas/MainMenuUI/PopupHolder/SettingsPopup(Clone)" );
            var settingsComp = mainPopup.GetComponent<SettingsPopup>();
            var background = mainPopup.transform.Find( "Background" ).gameObject;
            var generalHolder = background.transform.Find( "GeneralHolder" ).gameObject;
            var tabs = background.transform.Find( "Tabs" ).gameObject;
            var controlTab = tabs.transform.Find( "ControlsTab" ).gameObject;

            Logger.LogDebug( "Creating mod page..." );

            // Create a new page
            var modPage = InterfaceUtils.CreateUiObject( "ModsHolder", background.transform );
            // modPage.SetActive( false );
            
            RectTransform objRectTransform = modPage.GetComponent<RectTransform>();
            objRectTransform.anchoredPosition = new Vector2( 0f, 150f ); 
            objRectTransform.offsetMax = new Vector2( 250f, 425f );
            objRectTransform.offsetMin = new Vector2( -250f, 150f );
            objRectTransform.localScale = Vector3.one;
            objRectTransform.localPosition = new Vector3( 0f, 10f, 0f ); 

            var modPageContent = InterfaceUtils.CreateUiObject( "ModContent", modPage.transform );
            modPageContent.AddComponent<VerticalLayoutGroup>();

            RectTransform contentTransform = modPageContent.GetComponent<RectTransform>();
            contentTransform.anchoredPosition = new Vector2( 0f, 0f ); 
            contentTransform.anchorMin = Vector2.zero;
            contentTransform.anchorMax = Vector2.one;
            contentTransform.offsetMax = new Vector2( 0f, 0f );
            contentTransform.offsetMin = new Vector2( 0f, 0f );
            contentTransform.localScale = Vector3.one; 
            contentTransform.localPosition = new Vector3( 0f, 10f, 0f ); 

            var scrollRect = modPage.AddComponent<ScrollRect>();
            scrollRect.content = modPageContent.GetComponent<RectTransform>();

            var holders = Traverse.Create( __instance ).Field( "holders" ).GetValue<RectTransform[]>();
            holderIndex = holders.Length;
            holders.AddItem( objRectTransform );
            Traverse.Create( __instance ).Field( "holders" ).SetValue( holders );

            // Create settings for each mod
            foreach ( var pluginConfig in configs )
            {
                var modBox = InterfaceUtils.CreateUiObject( $"{pluginConfig.Key}Box", modPageContent.transform );
                modBox.AddComponent<VerticalLayoutGroup>();

                var boxLabel = InterfaceUtils.CreateUiObject( pluginConfig.Key, modBox.transform );
                InterfaceUtils.AddLocalizedLabel( boxLabel, pluginConfig.Key );

                Logger.LogDebug( $"Creating '{pluginConfig.Key}' configuration menu with {pluginConfig.Value.Count} entries." );

                var sections = pluginConfig.Value.GroupBy( x => x.Key.Section ).ToList();
                Logger.LogDebug( $"'{pluginConfig.Key}' has {sections.Count} sections." );
                foreach( var section in sections )
                {
                    var sectionLabel = InterfaceUtils.CreateUiObject( $"{section}Label", modBox.transform );
                    sectionLabel.AddComponent<VerticalLayoutGroup>();
                    InterfaceUtils.AddLocalizedLabel( sectionLabel, section.Key ); // TODO: make sure this doesn't break without localization

                    foreach ( var configKvp in section )
                    {
                        var configName = configKvp.Key.Key;
                        var configObject = InterfaceUtils.CreateUiObject( $"{configName}", modBox.transform );

                        var valueLabel = InterfaceUtils.CreateUiObject( $"{configName}Label", configObject.transform );
                        InterfaceUtils.AddLocalizedLabel( valueLabel, configName ); // TODO: make sure this doesn't break without localization

                        // Int/Float/Double: UnityEngine.UI.Slider
                        // TMPro.TMP_Dropdown
                        Logger.LogDebug( $"Config value '{configName}' has a default type of {configKvp.Value.DefaultValue.GetType()} and a current type of {configKvp.Value.BoxedValue.GetType()}." );
                    }
                }
            }

            Logger.LogDebug( "Creating Mods tab..." );

            // Create "Mods" tab
            var modTab = GameObject.Instantiate( controlTab, tabs.transform );
            modTab.name = "ModsTab";

            var modTabText = modTab.transform.GetChild( 0 ).gameObject;
            var label = modTabText.GetComponent<LocalizedLabel>();
            Traverse.Create( label ).Field( "key" ).SetValue( "ModTab" );

            var modTabTextComp = modTabText.GetComponent<TMPro.TextMeshProUGUI>();
            var tabTexts = Traverse.Create( __instance ).Field( "tabTexts" ).GetValue<TMPro.TMP_Text[]>();
            tabTexts.AddItem( (TMPro.TMP_Text)modTabTextComp );
            Traverse.Create( __instance ).Field( "tabTexts" ).SetValue( tabTexts );

            var button = modTab.GetComponent<UnityEngine.UI.Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ModTabClicked);
            return;
        }
    }
}