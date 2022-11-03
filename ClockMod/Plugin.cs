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
using System.Reflection.Emit;

namespace ClockMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class ClockModPlugin : BaseUnityPlugin
{
    private ConfigEntry<bool> modEnabled;
    private ConfigEntry<bool> hideHudBar;
    private ClockMod clockMod;

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        
        try
        {
            LoadConfig();

            if ( !modEnabled.Value )
                return;

            // Apply Harmony Patches
            var harmony = Harmony.CreateAndPatchAll( typeof( ClockPatch ) );
            Logger.LogInfo( $"Successfully patched {harmony.GetPatchedMethods().Count()} methods." );

            SceneManager.activeSceneChanged += OnSceneChange;
        }
        catch ( Exception ex )
        {
            Logger.LogInfo( $"Failed to start. {ex.Message}" );
        }
    }

    private void LoadConfig()
    {
        modEnabled = Config.Bind
        (
            new ConfigDefinition( "General", "modEnabled" ),
            true,
            new ConfigDescription( "Enable/Disable the mod here if desired." )
        );

        hideHudBar = Config.Bind
        (
            new ConfigDefinition( "Interface", "hideHudBar" ),
            true,
            new ConfigDescription( "Hides the top bar in game." )
        );
        
        ClockPatch.showMinutes = Config.Bind
        (
            new ConfigDefinition( "Interface", "showMinutes" ),
            true,
            new ConfigDescription( "Adds minutes to the time display." )
        );

        ClockPatch.superSpeedModifier = Config.Bind
        (
            new ConfigDefinition( "Interface", "superFastSpeedModifier" ),
            5,
            new ConfigDescription( "Super-speed time modifier. The default fast-forward is 5x speed, vanilla is 3x. Set below " )
        );

        ClockPatch.uppercasePeriod = Config.Bind
        (
            new ConfigDefinition( "Interface", "uppercasePeriod" ),
            true,
            new ConfigDescription( "Uppercases the AM/PM for 12-hr clock. Requires showing minutes." )
        );
    }

    void OnSceneChange( Scene previousActiveScene, Scene newActiveScene )
    {
        clockMod = null;
        if ( newActiveScene.name == "Tavern0" )
        {
            Logger.LogDebug( "Patching game menu..." );
            var ui = GameObject.Find( "GameUI" );
            for ( int i = 0; i < ui.transform.childCount; i++ )
            {
                var gameObj = ui.transform.GetChild( i ).gameObject;
                if ( gameObj.name == "TopHeader" )
                {
                    HeaderTweaks( gameObj );
                    break;
                }
            }
        }
    }

    void HeaderTweaks( GameObject topHeader )
    {
        for ( int i = 0; i < topHeader.transform.childCount; i++ )
        {
            var gameObj = topHeader.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "Bg":
                    if ( hideHudBar.Value )
                    {
                        gameObj.SetActive( false );
                    }
                    break;
                case "TimeControlButtons":
                    clockMod = new ClockMod( gameObj );
                    break;
                case "StatsButton": // missing notifications mostly
                case "IndicatorsButton": // bottom menu
                case "MoneyLabel":
                case "PrestigeLabel":
                case "GuestsCounter":
                case "FloorControlPanel": // move up/down floors
                case "FireTooltip":
                case "PauseMenuButton":
                    break;
                default:
                    Logger.LogDebug( $"Menu header object has no hook: {gameObj.name}" );
                    break;
            }
        }
    }
}

public class ClockMod
{
    private readonly GameObject _timeControlButtons;
    private GameObject Clock;
    private GameObject PauseButton;
    private GameObject PlayButton;
    private GameObject FastForwardButton;
    public static GameObject TripleSpeedButton;
    
    public ClockMod( GameObject timeControlButtons )
    {
        _timeControlButtons = timeControlButtons;

        for ( int i = 0; i < _timeControlButtons.transform.childCount; i++ )
        {
            var gameObj = _timeControlButtons.transform.GetChild( i ).gameObject;
            switch ( gameObj.name )
            {
                case "dayHour":
                    Clock = gameObj;
                    break;
                case "PauseButton":
                    PauseButton = gameObj;
                    break;
                case "PlayButton":
                    PlayButton = gameObj;
                    break;
                case "FastForwardButton":
                    FastForwardButton = gameObj;
                    break;
                case "TripleSpeedButton":
                    TripleSpeedButton = gameObj;
                    break;
                default:
                    Debug.LogWarning( $"ClockMod found unexpected object: {gameObj.name}" );
                    break;
            }
        }

        PostConstruct();
    }

    public void PostConstruct()
    {
        if ( Clock == null )
            Debug.LogWarning( $"ClockMod: Clock is null." );
        else
        {
            Clock.transform.localPosition = new Vector3( 13f, 1.5f, 0f );
            Clock.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( PauseButton == null )
            Debug.LogWarning( $"ClockMod: PauseButton is null." );
        else
        {
            PauseButton.transform.localPosition = new Vector3( 10f, -16f, 0f );
            PauseButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( PlayButton == null )
            Debug.LogWarning( $"ClockMod: PlayButton is null." );
        else
        {
            PlayButton.transform.localPosition = new Vector3( 30f, -16f, 0f );
            PlayButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( FastForwardButton == null )
            Debug.LogWarning( $"ClockMod: FastForwardButton is null." );
        else
        {
            FastForwardButton.transform.localPosition = new Vector3( 50f, -16f, 0f );
            FastForwardButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }
        
        if ( TripleSpeedButton == null )
        {
            TripleSpeedButton = GameObject.Instantiate( FastForwardButton );
            TripleSpeedButton.transform.SetParent( FastForwardButton.transform.parent, false );
            
            ClockPatch.fastForwardButton = TripleSpeedButton.GetComponent<Button>();
            ClockPatch.fastForwardButton.onClick.RemoveAllListeners();
            ClockPatch.fastForwardButton.onClick.AddListener( OnTripleSpeedButtonClicked );
        }

        TripleSpeedButton.transform.localPosition = new Vector3( 70f, -16f, 0f );
        TripleSpeedButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
    }

    public void OnTripleSpeedButtonClicked()
	{
        ClockPatch.OnCustomSpeedGame( ClockPatch.superSpeedModifier.Value, true );
	}
}

[HarmonyPatch]
public static class ClockPatch
{
    public static ConfigEntry<int> superSpeedModifier;
    public static ConfigEntry<bool> showMinutes;
    public static ConfigEntry<bool> uppercasePeriod;
    public static Button fastForwardButton;


    [HarmonyPatch( typeof( UiController ), "RefreshTimeButtons" ) ]
    [HarmonyPostfix]
    public static void UI_RefreshTimeButtons()
	{
        if ( fastForwardButton != null )
        {
            fastForwardButton.image.color = fastForwardButton.image.color.GetWithAlpha(
                TemporaryModel.I.GameSpeed > 1 && TemporaryModel.I.GameSpeed < superSpeedModifier.Value ? 1f : 0.2f
            );
        }

        if ( ClockMod.TripleSpeedButton != null && ClockMod.TripleSpeedButton.GetComponent<Button>() is Button tripleSpeedButton )
        {
            tripleSpeedButton.image.color = tripleSpeedButton.image.color.GetWithAlpha(
                TemporaryModel.I.GameSpeed == superSpeedModifier.Value ? 1f : 0.2f
            );
        }
        else
        {
            Debug.LogWarning( "ClockMod: REFRESHBUTTON could not find button component for Triple Speed Button!" );
        }
	}

    [HarmonyPatch( typeof( UiController ), "SetAllTimeControlButtonsActive" ) ]
    [HarmonyPostfix]
    public static void UI_SetAllTimeControlButtonsActive( ref bool isActive )
	{
        if ( ClockMod.TripleSpeedButton != null && ClockMod.TripleSpeedButton.GetComponent<Button>() is Button tripleSpeedButton )
        {
            tripleSpeedButton.image.color = tripleSpeedButton.image.color.GetWithAlpha(0.2f);
            tripleSpeedButton.interactable = isActive;
        }
        else
        {
            Debug.LogWarning( "ClockMod: SETALL could not find button component for Triple Speed Button!" );
        }
	}

    // [HarmonyPatch( typeof( UiController ), "PauseGame" ) ]
    // [HarmonyPostfix]
    // public static void PauseGame_Patch( ref bool isActive )
	// {
    //     // This should set gameSpeedBeforeSettingsOpened here so it can be 
    //     if ( ClockMod.TripleSpeedButton != null && ClockMod.TripleSpeedButton.GetComponent<Button>() is Button tripleSpeedButton )
    //     {
    //         tripleSpeedButton.image.color = tripleSpeedButton.image.color.GetWithAlpha(0.2f);
    //         tripleSpeedButton.interactable = isActive;
    //     }
	// }

    // static FieldInfo f_someField = AccessTools.Field( typeof(SomeType), nameof(SomeType.someField));
    // static MethodInfo m_MyExtraMethod = SymbolExtensions.GetMethodInfo(() => Tools.MyExtraMethod());

    /*
    [HarmonyPatch( typeof( UiController ), "LateTick" ) ]
    [HarmonyTranspiler]
    // Arguments are identified by their type and can have any name:
    // IEnumerable<CodeInstruction> instructions // [REQUIRED]
    // ILGenerator generator // [OPTIONAL]
    // MethodBase original
    public static IEnumerable<CodeInstruction> LateTick_Transpiler( IEnumerable<CodeInstruction> instructions )
    {
        var codes = new List<CodeInstruction>( instructions );
        int insertionIndex = -1;

        for (int i = 0; i < codes.Count - 2; i++) // -1 since we will be checking i + 1
        {


            if (codes[i].opcode == OpCodes.Call
            && (MethodInfo)codes[i].operand == AccessTools.Method( typeof(UiController), nameof(UiController.OnFastForwardGame ) )// instance void UiController::OnFastForwardGame(bool) 
            && codes[i + 1].opcode == OpCodes.Br_S)
            {
                Debug.Log( $"{codes[i].operand}, INSERTION AT {codes[i].opcode.Name}" );
                insertionIndex = i+1;
                break;
            }
            
            if (codes[i].opcode == OpCodes.Call )//&& codes[i + 1].opcode == OpCodes.Br_S )
            {
                Debug.Log( $"{codes[i].operand}" );
            }
        }
        
        if ( insertionIndex != -1 )
		{
            var instructionsToInsert = new List<CodeInstruction>();

            instructionsToInsert.Add( new( OpCodes.Ldarg_0 ) );
            instructionsToInsert.Add( new( OpCodes.Ldfld, AccessTools.Field( typeof(UiController), "gameSpeedBeforeSettingsOpened" ) ) );       //int32 UiController::gameSpeedBeforeSettingsOpened
            instructionsToInsert.Add( new( OpCodes.Ldsfld, AccessTools.Field( typeof(ClockPatch), nameof( ClockPatch.superSpeedModifier ) ) )  );      //class [BepInEx]BepInEx.Configuration.ConfigEntry`1<int32> [ClockMod]ClockMod.ClockPatch::superSpeedModifier
            instructionsToInsert.Add( new( OpCodes.Callvirt, AccessTools.Method(typeof(BepInEx.Configuration.ConfigEntry<int>), "get_Value()" ) ) );    //instance !0 class [BepInEx]BepInEx.Configuration.ConfigEntry`1<int32>::get_Value()
            instructionsToInsert.Add( new( OpCodes.Bne_Un_S, codes[ insertionIndex ].operand) );    // IL_0163

            instructionsToInsert.Add( new( OpCodes.Ldsfld, AccessTools.Field( typeof(ClockPatch), nameof( ClockPatch.superSpeedModifier ) ) ) );      // class [BepInEx]BepInEx.Configuration.ConfigEntry`1<int32> [ClockMod]ClockMod.ClockPatch::superSpeedModifier
            instructionsToInsert.Add( new( OpCodes.Callvirt, AccessTools.Method(typeof(BepInEx.Configuration.ConfigEntry<int>), "get_Value()" ) ) );    // instance !0 class [BepInEx]BepInEx.Configuration.ConfigEntry`1<int32>::get_Value()
            instructionsToInsert.Add( new( OpCodes.Ldc_I4_0 ) );
            instructionsToInsert.Add( new( OpCodes.Call, AccessTools.Method( typeof(ClockPatch), nameof(ClockPatch.OnCustomSpeedGame), new Type[] { typeof(int), typeof(bool)} ) ) );        // void [ClockMod]ClockMod.ClockPatch::OnCustomSpeedGame(int32, bool)
            instructionsToInsert.Add( codes[ insertionIndex ] );

			codes.InsertRange( insertionIndex, instructionsToInsert );
		}

        return codes.AsEnumerable();
    } */

    [HarmonyPatch( typeof( UiController ), "RefreshTime" ) ]
    [HarmonyPrefix]
    public static bool UI_RefreshTime()
	{
        if ( !showMinutes.Value )
            return true;

        var normalizedTime = Traverse.Create(UiController.I)
            .Field("timeOfTheDay")
            .Method("Evaluate", Mathf.Lerp(0f, 1f, TimeModel.I.DayTimerNormalized))
            .GetValue<float>();

        var hour = Mathf.FloorToInt(normalizedTime);
        var minutes = (normalizedTime - hour) * 60;
        string timeText;

		if (SettingsModel.I.TimeFormat == SettingsModel.TimeFormatType.American)
		{
            if ( hour > 11 && hour != 24)
            {
                if ( hour > 12 )
                    hour -= 12;
                
                timeText = String.Format("{0:00}:{1:00}{2}", hour, Mathf.FloorToInt(minutes), uppercasePeriod.Value ? "PM" : "pm" );
            }
            else
            {
                if ( hour == 0 || hour == 24 )
                {
                    hour = 12;
                }
                timeText = String.Format("{0:00}:{1:00}{2}", hour, Mathf.FloorToInt(minutes), uppercasePeriod.Value ? "AM" : "PM");
            }
        } 
        else
        {
            timeText = String.Format("{0:00}:{1:00}", hour, Mathf.FloorToInt(minutes));
        }

        Traverse.Create( UiController.I )
            .Field( "dayTime" )
            .Method( "SetText", timeText, true )
            .GetValue();

        return false;
    }

    public static void OnCustomSpeedGame( int gameSpeed, bool shouldShowWarningMessage = false )
	{
        Traverse.Create( UiController.I )
            .Method( "ChangeGameSpeed", gameSpeed, shouldShowWarningMessage )
            .GetValue();
	}
}