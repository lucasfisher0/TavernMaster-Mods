using System;
using System.Collections.Generic;

namespace UltimateLibrary;

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

	public static string GetText( string key, string defaultText = null )
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
        }

        if ( desiredDict.ContainsKey( key ) )
        {
            return desiredDict[ key ];
        }
        else
        {
            return EnglishKeys.ContainsKey( key ) ? EnglishKeys[ key ] : defaultText ?? key;
        }
	}

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
        }

        desiredDict ??= EnglishKeys;

        if ( desiredDict.ContainsKey( key ) )
            desiredDict.Remove( key );

        desiredDict.Add( key, text );
	}
}