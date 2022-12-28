using System;

namespace UltimateLibrary.Interfaces;


public enum PluginLoadTime
{
    Immediately,
    MainMenu,
    GameOnly
}

public interface IUltimatePlugin
{
    bool LoadPlugin( IUltimateLibrary library );

    bool IsPluginActive();

    bool UnloadPlugin( IUltimateLibrary library );

    PluginLoadTime GetPluginLoadTime();
    
    int GetPluginLoadPriority();
}