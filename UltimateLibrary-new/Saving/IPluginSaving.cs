using System;

namespace UltimateLibrary.Interfaces;

public interface ISavablePlugin
{
    void CreateData();

    void LoadData( string json );

    void SaveData( string json );
}