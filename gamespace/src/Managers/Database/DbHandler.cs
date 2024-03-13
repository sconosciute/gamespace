using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SQLite;

namespace gamespace.Managers.Database;

public static class DbHandler
{
    private static readonly string ConnPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gamespace", FilePath);

    private const string FilePath = "stats.db";
    private static readonly ILogger Log = Globals.LogFactory.CreateLogger<Game1>();

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

    public static void WriteStat(Statistic.Stats statName, int value)
    {
        if (!TryOpen(out var db)) return;

        var stat = new Statistic()
        {
            StatName = statName.ToString(),
            Value = value
        };

        db.Update(stat);
        db.Close();
    }
}