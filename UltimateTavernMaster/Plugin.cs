using BepInEx;

namespace UltimateTavernMaster;


[BepInPlugin( PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION )]
[BepInDependency( "org.bepinex.tavernmaster.UltimateLibrary",  "~1.0" )]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "org.bepinex.tavernmaster.UltimateTavernMaster";
    public const string PLUGIN_NAME = "Ultimate Tavern Master";
    public const string PLUGIN_VERSION = "1.0.0.0";

    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo( $"Plugin {PluginInfo.PLUGIN_GUID} is loaded!" );
    }
}