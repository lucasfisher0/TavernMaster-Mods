
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UltimateLibrary.Managers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UltimateLibrary.Extensions;

/// <summary>
/// Extensions for JSON operations.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Returns a list from a JSON file containing either a list or a singlular object.
    /// </summary>
    /// <param name="json">JSON text</param>
    /// <typeparam name="T">Return type</typeparam>
    /// <returns></returns>
    public static List<T> DeserializeSingleOrList<T>( string json )
    {
        Logger.LogWarning( $"JSON:\n\n{json}" );
        bool? isArray = null;
        {
            char ObjectStartToken = '{';
            char ArrayStartToken = '[';
            
            foreach ( char c in json )
            {
                if ( c == ObjectStartToken )
                {
                    isArray = false;
                    break;
                }
                else if ( c == ArrayStartToken )
                {
                    isArray = true;
                    break;
                }
            }
        }

        if ( isArray == null )
            Logger.LogWarning( "Could not determine whether JSON was an object or array!" );

        // Parse as array
        if ( isArray ?? true )
        {
            try
            {
                return JsonUtility.FromJson<List<T>>( json );
            }
            catch ( Exception ) { }
        }

        // Parse as object
        if ( !(isArray ?? false ) )
        {
            try
            {
                return new List<T>()
                {
                    JsonUtility.FromJson<T>( json )
                };
            }
            catch ( Exception ) { }
        }

        Logger.LogError( "Could not deserialize JSON as a single or list!" );
        return new List<T>();
    }
}