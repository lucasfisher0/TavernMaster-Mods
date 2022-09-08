using System;
using HarmonyLib;
using System.Reflection;

namespace UltimateLibrary;

class Achievements
{
    // Todo: custom achievements not related to Steam
    /*
    [HarmonyPatch( typeof( AchievementManager ), "RollDice" ) ] // Specify target method with HarmonyPatch attribute
    [HarmonyPrefix]
    static bool RollRealDice(ref int __result)
    {
        __result = 4; // The special __result variable allows you to read or change the return value
        return false; // Returning false in prefix patches skips running the original code
    }
    */
}
