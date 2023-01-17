using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;

// Copyright (c) 2021 JotunnLib Team

namespace UltimateLibrary.Utility;

/// <summary>
/// Helper methods to access BepInEx plugin information
/// </summary>
public static class BepInExUtils
{
    /// <summary>
    /// Cached plugin list
    /// </summary>
    private static BaseUnityPlugin[] Plugins;

    private static Dictionary<BepInEx.PluginInfo, string> PluginInfoTypeNameCache { get; } = new();
    private static Dictionary<Assembly, BepInEx.PluginInfo> AssemblyToPluginInfoCache { get; } = new();

    /// <summary>
    /// Cache loaded plugins which depend on Ultimate Library.
    /// </summary>
    private static BaseUnityPlugin[] CacheDependentPlugins()
    {
        List<BaseUnityPlugin> dependent = new();

        foreach ( var plugin in GetLoadedPlugins() )
        {
            if ( plugin.Info == null )
            {
                Logger.LogWarning( $"Plugin without Info found: {plugin.GetType().Assembly.FullName}" );
                continue;
            }
            if ( plugin.Info.Metadata == null )
            {
                Logger.LogWarning( $"Plugin without Metadata found: {plugin.GetType().Assembly.FullName}" );
                continue;
            }

            if ( plugin.Info.Metadata.GUID == PluginInfo.PLUGIN_GUID )
            {
                dependent.Add( plugin );
                continue;
            }

            foreach ( var dependencyAttribute in plugin.GetType().GetCustomAttributes( typeof( BepInDependency ), false ).Cast<BepInDependency>() )
            {
                if ( dependencyAttribute.DependencyGUID == PluginInfo.PLUGIN_GUID )
                {
                    dependent.Add( plugin );
                }
            }
        }

        return dependent.ToArray();
    }

    /// <summary>
    /// Get a dictionary of loaded plugins which depend on Ultimate Library.
    /// </summary>
    /// <returns>Dictionary of plugin GUID and <see cref="BaseUnityPlugin"/></returns>
    public static Dictionary<string, BaseUnityPlugin> GetDependentPlugins( bool includeUltimateLib = false )
    {
        if ( Plugins == null )
        {
            if ( ReflectionHelper.GetPrivateField<bool>( typeof( BepInEx.Bootstrap.Chainloader ), "_loaded") )
            {
                Plugins = CacheDependentPlugins();
            }
            else
            {
                return new();
            }
        }

        return Plugins
                .Where( plugin => includeUltimateLib || plugin.Info.Metadata.GUID != PluginInfo.PLUGIN_GUID )
                .ToDictionary( plugin => plugin.Info.Metadata.GUID );
    }

    /// <summary>
    /// Get a dictionary of all plugins loaded by BepInEx
    /// </summary>
    /// <returns>Dictionary of plugin GUID and <see cref="BaseUnityPlugin"/></returns>
    public static Dictionary<string, BaseUnityPlugin> GetPlugins( bool includeUltimateLib = false )
    {
        return GetLoadedPlugins()
                .Where( plugin => includeUltimateLib || plugin.Info.Metadata.GUID != PluginInfo.PLUGIN_GUID )
                .ToDictionary( plugin => plugin.Info.Metadata.GUID );
    }

    /// <summary>
    /// Get <see cref="PluginInfo"/> from a <see cref="Type"/>
    /// </summary>
    /// <param name="type"><see cref="Type"/> of the plugin main class</param>
    /// <returns></returns>
    public static BepInEx.PluginInfo GetPluginInfoFromType( Type type )
    {
        foreach (var info in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            var typeName = ReflectionHelper.GetPrivateProperty<string>(info, "TypeName");
            if ( typeName.Equals( type.FullName ) )
            {
                return info;
            }
        }

        return null;
    }

    private static string GetPluginInfoTypeName( BepInEx.PluginInfo info )
    {
        if ( PluginInfoTypeNameCache.ContainsKey( info ) )
        {
            return PluginInfoTypeNameCache[ info ];
        }

        var typeName = ReflectionHelper.GetPrivateProperty<string>( info, "TypeName" );
        PluginInfoTypeNameCache.Add( info, typeName );
        return typeName;
    }

    /// <summary>
    ///     Get <see cref="PluginInfo"/> from an <see cref="Assembly"/>
    /// </summary>
    /// <param name="assembly"><see cref="Assembly"/> of the plugin</param>
    /// <returns></returns>
    public static BepInEx.PluginInfo GetPluginInfoFromAssembly(Assembly assembly)
    {
        if (AssemblyToPluginInfoCache.ContainsKey(assembly))
        {
            return AssemblyToPluginInfoCache[assembly];
        }

        foreach (var info in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
        {
            if (assembly.GetType(GetPluginInfoTypeName(info)) != null)
            {
                AssemblyToPluginInfoCache.Add(assembly, info);
                return info;
            }
        }

        return null;
    }

    /// <summary>
    ///     Get <see cref="PluginInfo"/> from a path, also matches subfolder paths
    /// </summary>
    /// <param name="fileInfo"><see cref="FileInfo"/> object of the plugin path</param>
    /// <returns></returns>
    public static BepInEx.PluginInfo GetPluginInfoFromPath(FileInfo fileInfo) =>
        BepInEx.Bootstrap.Chainloader.PluginInfos.Values
            .Where(pi => pi.Location != null)
            .FirstOrDefault(pi =>
                fileInfo.DirectoryName != null &&
                fileInfo.DirectoryName.Contains(new FileInfo(pi.Location).DirectoryName) &&
                new FileInfo(pi.Location).DirectoryName != BepInEx.Paths.PluginPath);

    /// <summary>
    ///     Get metadata information from the current calling mod
    /// </summary>
    /// <returns></returns>
    public static BepInPlugin GetSourceModMetadata()
    {
        Type callingType = ReflectionHelper.GetCallingType();

        return GetPluginInfoFromType(callingType)?.Metadata ??
                GetPluginInfoFromAssembly(callingType.Assembly)?.Metadata ??
                UltimateLibraryPlugin.Instance.Info.Metadata;
    }

    private static IEnumerable<BaseUnityPlugin> GetLoadedPlugins()
    {
        return BepInEx.Bootstrap.Chainloader.PluginInfos
                        .Where(x => x.Value != null && x.Value.Instance != null)
                        .Select(x => x.Value.Instance);
    }
}

