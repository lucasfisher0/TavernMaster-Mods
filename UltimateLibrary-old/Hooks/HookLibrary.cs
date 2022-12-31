using System;

namespace UltimateLibrary.Hooks;

public enum HookRegisterTime
{
    Load,
    MainMenu,
    Game
}

public enum HookRegisterPeriod
{
    Forever,
    Rehook
}

public enum HookType
{
    Interface
}

public interface ILibraryHook
{
    HookRegisterTime GetRegisterTime { get; }

    HookRegisterPeriod GetRegisterPeriod { get; }

    void RegisterHooks();

    void UnregisterHooks();
}