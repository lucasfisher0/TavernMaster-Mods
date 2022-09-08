using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateLibrary;

[BepInPlugin( PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION )]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "org.bepinex.tavernmaster.UltimateLibrary";
    public const string PLUGIN_NAME = "Ultimate Library";
    public const string PLUGIN_VERSION = "1.0.0.0";

    // Inherits from UnityEngine.MonoBehavior and can use any of those methods
    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo( $"Plugin {Info.Metadata.Name} is loaded!" );

        // https://github.com/BepInEx/HarmonyX/wiki/
        // MethodInfo original = AccessTools.Method(typeof( StaffApp ), "UpdateButton");
        // MethodInfo patch = AccessTools.Method( typeof( StaffApp_Patch ), "UpdateButton_Postfix");
        // harmony.Patch(original, postfix: new HarmonyMethod ( patch ));

        var harmony = Harmony.CreateAndPatchAll( Assembly.GetExecutingAssembly() );
        Logger.LogInfo( $"Successfully patched {harmony.GetPatchedMethods().Count()} methods." );
    }
}

/*
public static class StaffDataExtensions
{

    public static void ResetSkills( this StaffData data )
    {
        try {
            var skills = ( data.Item as StaffItem ).skillz;
            foreach ( StatValue skill in skills )
            {
               data.Data.GetSkill( skill.type ).value = 0;
            }
        }
        catch ( System.Exception ex ) {
            Debug.LogError( $"Exception in CalculateResetPoints:\n{ex}" );
        }
    }
}
*/

/*
[HarmonyPatch ( typeof( StaffApp ) )]
public static class StaffApp_Patch
{
    public static InventoryItemActionButton resetSkillButton;

    
    [HarmonyPostfix]
    [HarmonyPatch( "UpdateButton" )]
    static void UpdateButton_Postfix( this StaffApp app )
    {
        try {
            var panel = GameObject.Find( "MasterUI/CompleteUI/HighOSv2/Desktop/Apps/StaffApp/RightPanel/StaffDetails/Scroll View/Viewport/Content/Skillz/Panel/" );
            if ( !panel )
            {
                Debug.LogError( "UpdateButton_Postfix: Could not find panel object!" );
                return;
            }

            var button = panel.transform.Find( "ResetSkillsButton" ).gameObject;
            if ( !button )
            {
                Debug.Log( "UpdateButton_Postfix: Creating Reset Skills Button..." );
                var hireButton = GameObject.Find( "MasterUI/CompleteUI/HighOSv2/Desktop/Apps/StaffApp/BottomPanel/ButtonPanel/ItemActionButtonV2 (1)" );
                button = GameObject.Instantiate( hireButton, panel.transform );
            }

            InventoryItemActionButton actionButton;
            actionButton = button.GetComponent<InventoryItemActionButton>();
            if (!actionButton)
            {
                Debug.LogError( "UpdateButton_Postfix: Failed to retrieve actionButton component!" );
                return;
            }

            if ( app.selectedStaff.CalculateResetPoints() > 0 ) // TODO: AND HAS ENOUGH CASH
            {
                Debug.Log( "UpdateButton_Postfix: Can reset!" );
                actionButton.Setup(
                "Reallocate Skills",
                delegate
                {
                    app.ResetSkillPopup();
                } );
            }
            else
            {
                Debug.Log( "UpdateButton_Postfix: Cannot reset." );
                actionButton.Setup(
                    "UPGRADE YA HOMIE FIRST",
                    null
                );
            }
        }
        catch ( System.Exception ex ) {
            Debug.LogError( $"Exception in patch of void almost.StaffApp::UpdateButton():\n{ex}" );
        }
    }

    public static void ResetSkillPopup( this StaffApp app )
    {
        try {
            Debug.Log( "Showing reset skill popup..." );
            app.popup.ShowPopup(
                "RESET " + app.selectedStaff.Name.ToUpper() + "'s SKILLS?",
                "THIS WILL COST A SHIT TON",
                "PAY UP BITCH",
                "HAHA JK...",
                new Action( app.ResetSkills ),
                null );
        }
        catch ( System.Exception ex ) {
            Debug.LogError( $"Exception in ResetSkillPopup():\n{ex}" );
        }
    }  

    public static void ResetSkills( this StaffApp app )
    {
        try {
            Debug.Log( "RESETTING SKILLS..." );
            app.selectedStaff.Data.points = app.selectedStaff.CalculateResetPoints();
            app.selectedStaff.ResetSkills();
            app.selectedStaff.SetPayload<StaffStats>(app.selectedStaff.Data);

            MethodInfo dynMethod = app.GetType().GetMethod( "UpdateStaffDetails",  BindingFlags.NonPublic | BindingFlags.Instance );
            dynMethod.Invoke( app, new object[] { });

			// app.shop.staffManager.UpgradeSkill(this.selectedStaff);
        }
        catch (System.Exception ex) {
            Debug.LogError( $"Exception in ResetSkills():\n{ex}" );
        }
    }            
}
*/