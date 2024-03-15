using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamespace.Managers;

public static class SettingsManager
{
    /// <summary>
    /// Default resolution height.
    /// </summary>
    private const int DefaultResHeight = 1080;
    
    /// <summary>
    /// Default resolution width.
    /// </summary>
    private const int DefaultResWidth = 1920;
    
    /// <summary>
    /// Variable that determines if the application should be full screened or not.
    /// </summary>
    private const bool FullScreen = false;
    
    /// <summary>
    /// Dynamic resolution to fit to the users monitor.
    /// </summary>
    private const bool DynamicRes = false;
    
    /// <summary>
    /// Debug log.
    /// </summary>
    private static ILogger _log;

    /// <summary>
    /// Initializes log debugger.
    /// </summary>
    /// <param name="logger">The debug log.</param>
    public static void Init(in ILogger logger)
    {
        _log ??= logger;
    }

    /// <summary>
    /// Generates all graphics for the game.
    /// </summary>
    /// <param name="game">The game to generate graphics in.</param>
    public static GraphicsDeviceManager GenerateGraphics(in Game1 game)
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

    /// <summary>
    /// Loads the launch settings.
    /// </summary>
    /// <param name="fileName">The file name to read.</param>
    private static LaunchSettings LoadLaunchSettings(in string fileName)
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

    /// <summary>
    /// Tries to read the specified config file from the users AppData/Local folder.
    /// </summary>
    /// <param name="fileName">The ConfNames file to attempt to read.</param>
    /// <param name="data">The read in contents of the file or null if failed to read.</param>
    /// <returns>True if the file was successfully read, else false.</returns>
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
    /// <summary>
    /// The directory where all game Config files are stored relative to the OS storage location.
    /// </summary>
    public const string ConfigDir = "Gamespace\\configs";
    
    /// <summary>
    /// Name of the Graphics configuration file inside the config directory.
    /// </summary>
    public const string LaunchConfig = "launchConfig.json";
    
    /// <summary>
    /// Name of the key bindings config file 
    /// </summary>
    public const string KeyBinds = "keyBindings.json";
}