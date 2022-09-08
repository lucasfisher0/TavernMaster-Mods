using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UltimateTavernMaster;

public class ClockMod
{
    private readonly GameObject _timeControlButtons;
    private GameObject Clock;
    private GameObject PauseButton;
    private GameObject PlayButton;
    private GameObject FastForwardButton;
    private GameObject TripleSpeedButton;
    
    public ClockMod( GameObject timeControlButtons )
    {
        _timeControlButtons = timeControlButtons;

        for ( int i = 0; i < _timeControlButtons.transform.childCount; i++ )
        {
            Debug.Log( "ClockMod: constructed" );
            var gameObj = _timeControlButtons.transform.GetChild( i ).gameObject;
            Debug.LogWarning( $"ClockMod: evaluating object {gameObj.name}" );
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

        Debug.Log( "ClockMod: running post construct" );
        PostConstruct();
    }

    public void PostConstruct()
    {
        if ( Clock == null )
        {
            Debug.LogWarning( $"ClockMod: Clock is null." );
        }
        else
        {
            Clock.transform.localPosition = new Vector3( 13f, 1.5f, 0f );
            Clock.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( PauseButton == null )
        {
            Debug.LogWarning( $"ClockMod: PauseButton is null." );
        }
        else
        {
            PauseButton.transform.localPosition = new Vector3( 10f, -16f, 0f );
            PauseButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( PlayButton == null )
        {
            Debug.LogWarning( $"ClockMod: PlayButton is null." );
        }
        else
        {
            PlayButton.transform.localPosition = new Vector3( 30f, -16f, 0f );
            PlayButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }

        if ( FastForwardButton == null )
        {
            Debug.LogWarning( $"ClockMod: FastForwardButton is null." );
        }
        else
        {
            FastForwardButton.transform.localPosition = new Vector3( 50f, -16f, 0f );
            FastForwardButton.transform.localScale    = new Vector3( 0.7f, 0.7f, 0.7f );
        }
        
        Debug.Log( "ClockMod: creating new speed button" );
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

        Debug.Log( "ClockMod: post-construct completed" );
    }

    public void OnTripleSpeedButtonClicked()
	{
        Debug.Log( "ClockMod: super-fast button clicked" );
        ClockPatch.OnCustomSpeedGame( ClockPatch.superFastSpeed, true );
	}
}

[HarmonyPatch]
static class ClockPatch
{
    public static int superFastSpeed;
    public static Button fastForwardButton;
    public static bool showMinutes;

    [HarmonyPatch( typeof( UiController ), "RefreshTimeButtons" ) ]
    [HarmonyPostfix]
    public static void UI_RefreshTimeButtons()
	{
        Debug.Log( "ClockPatch: UI_RefreshTimeButtons" );
        if ( fastForwardButton != null )
        {
            fastForwardButton.image.color = fastForwardButton.image.color.GetWithAlpha(
                TemporaryModel.I.GameSpeed == superFastSpeed ? 1f : 0.2f
            );
        }
	}

    [HarmonyPatch( typeof( UiController ), "SetAllTimeControlButtonsActive" ) ]
    [HarmonyPostfix]
    public static void UI_SetAllTimeControlButtonsActive()
	{
        Debug.Log( "ClockPatch: UI_SetAllTimeControlButtonsActive" );
        if ( fastForwardButton != null )
        {
            fastForwardButton.image.color = fastForwardButton.image.color.GetWithAlpha( 0.2f );
        }
	}

    [HarmonyPatch( typeof( UiController ), "RefreshTime" ) ]
    [HarmonyPrefix]
    public static bool UI_RefreshTime()
	{
        // Debug.Log( "ClockPatch: UI_RefreshTime" );

        var normalizedTime = Traverse.Create(UiController.I)
            .Field("timeOfTheDay")
            .Method("Evaluate", Mathf.Lerp(0f, 1f, TimeModel.I.DayTimerNormalized))
            .GetValue<float>();

        // Debug.Log( $"UI_RefreshTime: {Mathf.Lerp( 0f, 1f, TimeModel.I.DayTimerNormalized)} => {normalizedTime}" );

        var hour = Mathf.FloorToInt(normalizedTime);
        var minutes = (normalizedTime - hour) * 60;
        string timeText;

        // Debug.Log( $"UI_RefreshTime: {hour} : {minutes}" );

		if (SettingsModel.I.TimeFormat == SettingsModel.TimeFormatType.American)
		{
            if ( hour > 12 && hour != 24)
            {
                hour -= 12;
                timeText = String.Format("{0:00}:{1:00}PM", hour, Mathf.FloorToInt(minutes));
            }
            else
            {
                if ( hour == 0 || hour == 24 )
                {
                    hour = 12;
                }
                timeText = String.Format("{0:00}:{1:00}AM", hour, Mathf.FloorToInt(minutes));
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
        Debug.Log( $"ClockPatch: OnCustomSpeedGame to x{gameSpeed}" );
        
        Traverse.Create( UiController.I )
            .Method( "ChangeGameSpeed", gameSpeed, shouldShowWarningMessage )
            .GetValue();
	}
}