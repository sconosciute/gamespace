using SQLite;

namespace gamespace.Managers.Database;

[Table("Statistic")]
public class Statistic
{
    [PrimaryKey]
    [Column("statName")]
    public string StatName { get; set; }
    
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