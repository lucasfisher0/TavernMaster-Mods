using BepInEx;

namespace MyFirstPlugin;


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
        Logger.LogInfo($"Plugin {Info.Metadata.Name} is loaded!");
    }
}