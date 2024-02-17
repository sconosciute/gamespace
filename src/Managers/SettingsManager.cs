using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public static class SettingsManager
{
    private const int DefaultResHeight = 800;
    private const int DefaultResWidth = 800;
    private const bool FullScreen = false;
    private const bool DynamicRes = false;

    public static GraphicsDeviceManager GenerateGraphics(Game1 game)
    {
        var graphics = new GraphicsDeviceManager(game);
        var adapter = new GraphicsAdapter();
        
        const string fileName = "launchConfig.json";
        var settings = LoadSettings(fileName);
        if (settings != null)
        {
            if (settings.IsDynamic)
            {
                graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = settings.DefaultResWidth;
                graphics.PreferredBackBufferHeight = settings.DefaultResHeight;
            }

            graphics.IsFullScreen = settings.IsFullScreened;
        }
        graphics.ApplyChanges();
        return graphics;
    }
    private static LaunchSettings LoadSettings(string fileName)
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
         baseDir = Path.Combine(baseDir, "Configs");
        var path = Path.Combine(baseDir, fileName);
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
            Directory.CreateDirectory(baseDir); 
            UpdateSettings(path, defaultSettings);
            return defaultSettings;
        }
    }
    private static void UpdateSettings(string path, object defaultSettings)
    {
        var json = JsonSerializer.Serialize(defaultSettings);
        File.WriteAllText(path, json);
    }
}