# Ultimate Tavern Master

A mod and library for modding Tavern Master using BepinEx 5.x

## Features
- Clock Mod
    - Adds a new fast-forward button with a customizable speed. (Default is 5x, vanilla fast speed is 3x.)
    - Digital clock style: shows exact minutes

- UI Tweaks
    - Hide top bar

## Planned Features
- Fight Club:
    - Patrons now have an internal rowdiness level.
    - Drunk patrons spend more, but may find themselves getting into fights.
    - Fights cause those with a low rowdiness to flee.
    - Guards can now be assigned to day shift to act as bouncers.
    - Having a bouncer stops fights from occurring, relative to the patron's rowdiness level.
    - This should be affected by difficulty.
    - Early game, you should be able to use bar-keeps to address fights. 

- Better Patrons:
    - Name Generation
        - Rowdiness level
    - Famous Adventurers
        - Very high ability
        - Adds extra cost to a journey
        - Chance of extra rewards

- Choose Your Flavor:
    - Existing bar purchases are now generic barrels
    - Drinks can be assigned to barrels in a bar
    - Empty existing barrel into storage? (So it can be reassigned.)
    - New drinks!

- Better Research:
    - You should be able to trade cash for increased research.
    - More diverse tech tree. (Tabs?)

- General Tweaks:
    - Forbid guests from using certain doors/stairs
    - Color pallette is now 4x3 (ADD YELLOW DAMMIT)
    - Special Events window is now as big as the research window
    - Clone selection in props shop

- Really, REALLY farfetched ideas:
    - Grain mill on the river, will automatically generate flour in return for hiring a miller

## Known Issues:
- Space will not start time again from super-speed.







# Ultimate Library

## Running locally
Change the game directory in the .csproj for each plugin.
Build.

## Creating a plugin
`dotnet new bepinex5plugin -n <PluginName> -T netstandard2.0 -U 2020.3.19`
`dotnet new bep6plugin_unitymono -n <PluginName> -T netstandard2.0 -U 2020.3.19`