using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using SQLite;

namespace gamespace.Managers.Database;

public static class DbHandler
{
    /// <summary>
    /// The file path for connection.
    /// </summary>
    private static readonly string ConnPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gamespace", FilePath);

    /// <summary>
    /// The file name that the database will be stored in.
    /// </summary>
    private const string FilePath = "stats.db";
    
    /// <summary>
    /// Debug logger.
    /// </summary>
    private static readonly ILogger Log = Globals.LogFactory.CreateLogger<Game1>();

    /// <summary>
    /// Initializes database.
    /// </summary>
    public static void InitDatabase()
    {
        if (TryOpen(out var db))
        {
            var query = "INSERT OR IGNORE INTO Statistic (statName, value) VALUES (?, ?)";
            foreach (var stat in Enum.GetValues(typeof(Statistic.Stats)))
            {
                db.Execute(query, stat.ToString(), 0);
            }
            Log.LogInformation("Initialized Database");
        }
    }
    
    /// <summary>
    /// Tries to open the database and create a statistic table.
    /// </summary>
    /// <param name="db">The database to connect to.</param>
    private static bool TryOpen(out SQLiteConnection db)
    {
        try
        {
            db = new SQLiteConnection(ConnPath);
            db.CreateTable<Statistic>();
            return true;
        }
        catch (Exception e)
        {
            db = null;
            Log.LogError("Failed to establish database connection due to {e}", e);
            return false;
        }
    }

    /// <summary>
    /// Writes/updates the stats associated with the primary key.
    /// </summary>
    /// <param name="statName">The database primary key.</param>
    /// <param name="value">The value to update.</param>
    public static void WriteStat(Statistic.Stats statName, int value)
    {
        if (!TryOpen(out var db)) return;

        var stat = new Statistic()
        {
            StatName = statName.ToString(),
            Value = value
        };

        var query = "UPDATE Statistic SET value = value + ? WHERE statName = ?";
        db.Execute(query, value, statName.ToString());
        db.Close();
    }

    /// <summary>
    /// Retrieves the stat associated with the primary key.
    /// </summary>
    /// <param name="statName">The database primary key.</param>
    public static int GetStat(Statistic.Stats statName)
    {
        if (!TryOpen(out var db)) return 0;
        var res = db.Table<Statistic>().First(t => t.StatName == statName.ToString());
        db.Close();
        return res.Value;

    }

    /// <summary>
    /// Gets all stats in the database.
    /// </summary>
    public static List<Statistic> GetAllStats()
    {
        if (!TryOpen(out var db)) return null;
        var res = db.Table<Statistic>().ToList();
        db.Close();
        return res;
    }
}