using SQLite;

namespace gamespace.Managers.Database;

[Table("Statistic")]
public class Statistic
{
    /// <summary>
    /// The primary key in the table.
    /// </summary>
    [PrimaryKey]
    [Column("statName")]
    public string StatName { get; set; }
    
    /// <summary>
    /// The value that will be set in the database.
    /// </summary>
    [Column("value")]
    public int Value { get; set; }

    /// <summary>
    /// IDs for stat columns.
    /// </summary>
    public enum Stats
    {
        TimeInDungeon,
        DeathCount,
        WinCount,
        MobKillCount,
        ItemsFound
    }
    
}