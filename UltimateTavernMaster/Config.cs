using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

namespace UltimateTavernMaster;

class UltimateConfigManager
{
    private readonly BaseUnityPlugin _plugin;

    public ConfigEntry<bool> modEnabled;
    public ConfigEntry<bool> transparentHeader;
    public ConfigEntry<bool> changeIndicatorButton;
    public ConfigEntry<bool> useEnhancedClock;
    public ConfigEntry<bool> showMinutes;
    public ConfigEntry<int> superSpeedModifier;

    public UltimateConfigManager( BaseUnityPlugin plugin )
    {
        _plugin = plugin;
        BindConfig();
    }

    private void BindConfig()
    {
        // General
        modEnabled = _plugin.Config.Bind
        (
            new ConfigDefinition( "General", "ultimateModEnabled" ),
            true,
            new ConfigDescription( "Disable the mod here if desired." )
        );

        // Interface
        transparentHeader = _plugin.Config.Bind
        (
            new ConfigDefinition( "Interface", "transparentHeaderBar" ),
            true,
            new ConfigDescription( "Disables the background image at the top of the in-game menu." )
        );
        changeIndicatorButton = _plugin.Config.Bind
        (
            new ConfigDefinition( "Interface", "changeIndicatorButton" ),
            true,
            new ConfigDescription( "Changes the indicator button from a hamburger to an eye icon." )
        );
        useEnhancedClock = _plugin.Config.Bind
        (
            new ConfigDefinition( "Interface", "useEnhancedClock" ),
            true,
            new ConfigDescription( "Replaces the clock interface with a new one allowing super fast-forward." )
        );
        showMinutes = _plugin.Config.Bind
        (
            new ConfigDefinition( "Interface", "showMinutes" ),
            true,
            new ConfigDescription( "Adds minutes to the time display." )
        );
        superSpeedModifier = _plugin.Config.Bind
        (
            new ConfigDefinition( "Interface", "superFastSpeedModifier" ),
            5,
            new ConfigDescription( "Super-speed time modifier. The default fast-forward is 3x speed." )
        );
    }
}
