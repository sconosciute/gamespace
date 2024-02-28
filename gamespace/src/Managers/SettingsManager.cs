using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public static class SettingsManager
{
    private const int DefaultResHeight = 1080;
    private const int DefaultResWidth = 1920;
    private const bool FullScreen = false;
    private const bool DynamicRes = false;
    private static ILogger _log;

    public static void Init(ILogger logger)
    {
        _log ??= logger;
    }

    public static GraphicsDeviceManager GenerateGraphics(Game1 game)
    {
        var graphics = new GraphicsDeviceManager(game);
        var adapter = new GraphicsAdapter();
        
        const string fileName = "launchConfig.json";
        var settings = LoadLaunchSettings(fileName);
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
    private static LaunchSettings LoadLaunchSettings(string fileName)
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
         baseDir = Path.Combine(baseDir, "GameSpace");
        var launchConfigDir = Path.Combine(baseDir, "LaunchSettings");
        var path = Path.Combine(launchConfigDir, fileName);
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
            Directory.CreateDirectory(launchConfigDir); 
            UpdateSettings(path, defaultSettings);
            return defaultSettings;
        }
    }
    private static void UpdateSettings(string path, object defaultSettings)
    {
        var json = JsonSerializer.Serialize(defaultSettings);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Tries to write the specified config file into the users AppData/Local folder.
    /// </summary>
    /// <param name="fileName">Name of the file from ConfNames.</param>
    /// <param name="toWrite">The string to write into the file.</param>
    /// <returns>True if the file has been successfully written, else false.</returns>
    public static bool TryWriteConfig(in string fileName, in string toWrite)
    {
        var confDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        confDir = Path.Combine(confDir, ConfNames.ConfigDir);

        if (!Directory.Exists(confDir))
        {
            Directory.CreateDirectory(confDir);
        }

        var filePath = Path.Combine(confDir, fileName);
        try
        {
            File.WriteAllText(fileName, toWrite);
        }
        catch (Exception e)
        {
            _log.LogInformation("Failed to write file to disc due to {ex}", e);
            return false;
        }

        return true;
    }
}

public static class ConfNames
{
    public const string ConfigDir = "configs";
    public const string LaunchConfig = "launchConfig.json";
    public const string KeyBinds = "keyBindings.json";
}