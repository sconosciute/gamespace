using System;
using System.IO;
using System.Text.Json;

namespace gamespace.Managers;

public static class SettingsManager
{
    private const int DefaultResHeight = 800;
    private const int DefaultResWidth = 800;
    private const bool FullScreen = false;
    private const bool DynamicRes = false;
    public static LaunchSettings LoadSettings(string fileName)
    {
        var appData = AppDomain.CurrentDomain.BaseDirectory;
        appData = Path.Combine(appData, "Configs");
        var path = Path.Combine(appData, fileName);
        
        Console.Write(path);
        try
        {
            var jsonString = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<LaunchSettings>(jsonString);
            return settings;
        }
        catch (Exception ex) when(ex is FileNotFoundException or DirectoryNotFoundException)
        {
            var defaultSettings = new LaunchSettings
            {
                DefaultResHeight = DefaultResHeight,
                DefaultResWidth = DefaultResWidth,
                IsFullScreened = FullScreen,
                IsDynamic = DynamicRes
            };
            Directory.CreateDirectory(path); 
            UpdateSettings(path, defaultSettings);
            return defaultSettings;
        }
    }
    private static void UpdateSettings(string path, Object defaultSettings)
    {
        string json = JsonSerializer.Serialize(defaultSettings);
        File.WriteAllText(path, json);
    }
}