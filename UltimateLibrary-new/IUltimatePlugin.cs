using System;

namespace UltimateLibrary.Interfaces;

public interface IUltimatePlugin
{
    bool LoadPlugin( IUltimateLibrary library );

    bool IsPluginActive();

    bool UnloadPlugin( IUltimateLibrary library );
}