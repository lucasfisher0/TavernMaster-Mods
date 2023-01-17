using System;
using System.Collections.Generic;

namespace UltimateLibrary.Utility;

/// <summary>
/// Localization wrapper for custom strings
/// </summary>
public static class Localization
{
    private static Dictionary<string,string> EnglishKeys            = new Dictionary<string,string>();
    private static Dictionary<string,string> RussianKeys            = new Dictionary<string,string>();
    private static Dictionary<string,string> SpanishKeys            = new Dictionary<string,string>();
    private static Dictionary<string,string> FrenchKeys             = new Dictionary<string,string>();
    private static Dictionary<string,string> GermanKeys             = new Dictionary<string,string>();
    private static Dictionary<string,string> ItalianKeys            = new Dictionary<string,string>();
    private static Dictionary<string,string> PortugueseKeys         = new Dictionary<string,string>();
    private static Dictionary<string,string> TurkishKeys            = new Dictionary<string,string>();
    private static Dictionary<string,string> ChineseSimplifiedKeys  = new Dictionary<string,string>();
    private static Dictionary<string,string> ChineseTraditionalKeys = new Dictionary<string,string>();
    private static Dictionary<string,string> JapaneseKeys           = new Dictionary<string,string>();
    private static Dictionary<string,string> KoreanKeys             = new Dictionary<string,string>();
    private static Dictionary<string,string> PolishKeys             = new Dictionary<string,string>();

    /// <summary>
    /// Retrieves the localized text from a string. Defaults to the given key, unless a default is specified.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultText"></param>
	public static string GetText( string key, string defaultText = null )
	{
        Dictionary<string,string> desiredDict = null;
        switch ( LocalizationModel.I.Language )
        {
            case LocalizationModel.LanguageType.Russian:
                desiredDict = RussianKeys;
                break;
            case LocalizationModel.LanguageType.Spanish:
                desiredDict = SpanishKeys;
                break;
            case LocalizationModel.LanguageType.French:
                desiredDict = FrenchKeys;
                break;
            case LocalizationModel.LanguageType.German:
                desiredDict = GermanKeys;
                break;
            case LocalizationModel.LanguageType.Italian:
                desiredDict = ItalianKeys;
                break;
            case LocalizationModel.LanguageType.Portuguese:
                desiredDict = PortugueseKeys;
                break;
            case LocalizationModel.LanguageType.Turkish:
                desiredDict = TurkishKeys;
                break;
            case LocalizationModel.LanguageType.ChineseSimplified:
                desiredDict = ChineseSimplifiedKeys;
                break;
            case LocalizationModel.LanguageType.ChineseTraditional:
                desiredDict = ChineseTraditionalKeys;
                break;
            case LocalizationModel.LanguageType.Japanese:
                desiredDict = JapaneseKeys;
                break;
            case LocalizationModel.LanguageType.Korean:
                desiredDict = KoreanKeys;
                break;
            case LocalizationModel.LanguageType.Polish:
                desiredDict = PolishKeys;
                break;
            case LocalizationModel.LanguageType.English:
            default:
                desiredDict = EnglishKeys;
                break;
        }

        return desiredDict.ContainsKey( key )
            ? desiredDict[ key ]
            : EnglishKeys.ContainsKey( key )
                ? EnglishKeys[ key ]
                : defaultText ?? key;
	}

    /// <summary>
    /// Updates the localization for a string key for the specified language.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="key"></param>
    /// <param name="text"></param>
    public static void UpsertText( LocalizationModel.LanguageType language, string key, string text )
	{
        Dictionary<string,string> desiredDict = null;
        switch ( LocalizationModel.I.Language )
        {
            case LocalizationModel.LanguageType.English:
                desiredDict = EnglishKeys;
                break;
            case LocalizationModel.LanguageType.Russian:
                desiredDict = RussianKeys;
                break;
            case LocalizationModel.LanguageType.Spanish:
                desiredDict = SpanishKeys;
                break;
            case LocalizationModel.LanguageType.French:
                desiredDict = FrenchKeys;
                break;
            case LocalizationModel.LanguageType.German:
                desiredDict = GermanKeys;
                break;
            case LocalizationModel.LanguageType.Italian:
                desiredDict = ItalianKeys;
                break;
            case LocalizationModel.LanguageType.Portuguese:
                desiredDict = PortugueseKeys;
                break;
            case LocalizationModel.LanguageType.Turkish:
                desiredDict = TurkishKeys;
                break;
            case LocalizationModel.LanguageType.ChineseSimplified:
                desiredDict = ChineseSimplifiedKeys;
                break;
            case LocalizationModel.LanguageType.ChineseTraditional:
                desiredDict = ChineseTraditionalKeys;
                break;
            case LocalizationModel.LanguageType.Japanese:
                desiredDict = JapaneseKeys;
                break;
            case LocalizationModel.LanguageType.Korean:
                desiredDict = KoreanKeys;
                break;
            case LocalizationModel.LanguageType.Polish:
                desiredDict = PolishKeys;
                break;
            default:
                Logger.LogWarning( $"Could not insert key {key} for language: {language}" );
                break;
        }

        desiredDict ??= EnglishKeys;

        if ( desiredDict.ContainsKey( key ) )
            desiredDict.Remove( key );

        desiredDict.Add( key, text );
	}

    /// <summary>
    /// Adds the content of the CSV file containing localization entries.
    /// DebugName should optimally be PluginName/FileName.
    /// </summary>
    /// <param name="fileContent"></param>
    /// <param name="debugName"></param>
    public static void AddCsvText( string fileContent, string debugName = "Unknown" )
    {
        var lines = fileContent.Split( "\n"[0] ); //.ToList();
        Logger.LogDebug( $"Adding localization CSV '{debugName}' with {lines.Length} keys." );

        // Currently, localization CSV files are hardcoded:
        // Key,Comment,English,Russian,Spanish,French,German,Italian,Portuguese,Turkish,ChineseSimplified,ChineseTraditional,Japanese,Korean,Polish

        // For each line...
        int currentLine = 0;
        foreach( var line in lines[1..] )
        {
            currentLine++;
            var lineData = ( line.Trim() ).Split( ","[0] );
            var key = lineData[0].Trim();

            if ( string.IsNullOrWhiteSpace( key ) )
            {
                Logger.LogError( $"Localization CSV '{debugName}' has a blank key on line: {currentLine}." );
                continue;
            }

            // For each language, try getting a value
            for ( int i = 0; i < 12; i++ )
            {
                try
                {
                    var locText = lineData[i+2].Trim();
                    if ( string.IsNullOrWhiteSpace( locText ) )
                        continue;

                    Logger.LogDebug( $"Adding localization: {key} in {(LocalizationModel.LanguageType)i} is {locText}." );
                    UpsertText( (LocalizationModel.LanguageType)i, key, locText );
                }
                catch ( Exception ex )
                {
                    Logger.LogError( $"Loc CSV '{debugName}' encounter an error parsing line {currentLine}:\n{ex.Message}" );
                }
            }
        }
    }
}