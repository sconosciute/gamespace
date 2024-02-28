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

        var settings = LoadLaunchSettings(ConfNames.LaunchConfig);
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
        if (TryReadConfig(fileName, out var data))
        {
            var settings = JsonSerializer.Deserialize<LaunchSettings>(data);
            return settings;
        }

        else
        {
            var settings = new LaunchSettings()
            {
                DefaultResHeight = DefaultResHeight,
                DefaultResWidth = DefaultResWidth,
                IsFullScreened = FullScreen,
                IsDynamic = DynamicRes
            };
            var jsonData = JsonSerializer.Serialize(settings);
            TryWriteConfig(fileName, jsonData);
            return settings;
        }
    }

    /// <summary>
    /// Tries to write the specified config file into the users AppData/Local folder.
    /// </summary>
    /// <param name="fileName">Name of the file from ConfNames.</param>
    /// <param name="toWrite">The string to write into the file.</param>
    /// <returns>True if the file has been successfully written, else false.</returns>
    public static bool TryWriteConfig(in string fileName, in string toWrite)
    {
        var confDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ConfNames.ConfigDir);

        if (!Directory.Exists(confDir))
        {
            Directory.CreateDirectory(confDir);
        }

        var filePath = Path.Combine(confDir, fileName);
        try
        {
            File.WriteAllText(filePath, toWrite);
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            _log.LogInformation("Failed to write {file} to disc due to {ex}", fileName, e);
            return false;
        }

        return true;
    }

    public static bool TryReadConfig(in string fileName, out string data)
    {
        var confDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ConfNames.ConfigDir);
        var filePath = Path.Combine(confDir, fileName);
        try
        {
            data = File.ReadAllText(filePath);
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            _log.LogInformation("Failed to read {file} to disc due to {ex}", fileName, e);
            data = null;
            return false;
        }

        return true;
    }
}

public static class ConfNames
{
    public const string ConfigDir = "Gamespace\\configs";
    public const string LaunchConfig = "launchConfig.json";
    public const string KeyBinds = "keyBindings.json";
}