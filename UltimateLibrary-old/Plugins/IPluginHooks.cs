using System;
using System.Collections.Generic;

using UltimateLibrary.Hooks;

namespace UltimateLibrary.Plugins;

public interface IPluginHooks
{
    List<ILibraryHook> GetAllHooks();

    List<IInterfaceHooks> GetInterfaceHooks();
}