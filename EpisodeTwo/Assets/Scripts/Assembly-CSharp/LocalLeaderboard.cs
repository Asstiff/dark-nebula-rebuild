using System;
using System.Collections.Generic;
using System.Globalization;

// The HD branch's local Top 10, retained alongside the later release's best times.
// Lists belong to callers: adding blank UI rows must never modify saved records.
public sealed class LocalLeaderboard
{
    public const int MaxEntries = 10;
    private readonly Func<string, string> read;
    private readonly Action<string, string> write;
    private readonly Func<bool, int, LeaderboardEntryVO?> legacyBest;

    public LocalLeaderboard() : this(ReadStored, WriteStored, GetLegacyBest) { }

    public LocalLeaderboard(Func<string, string> read, Action<string, string> write,
        Func<bool, int, LeaderboardEntryVO?> legacyBest = null)
    {
        this.read = read;
        this.write = write;
        this.legacyBest = legacyBest;
    }

    public static bool IsValidTime(float time)
    {
        return time > 0f && !float.IsNaN(time) && !float.IsInfinity(time);
    }

    public static string StorageKey(bool expert, int level)
    {
        return string.Format(CultureInfo.InvariantCulture, "Leaderboard({0},{1})", expert, level);
    }

    public List<LeaderboardEntryVO> GetList(bool expert, int level)
    {
        var result = new List<LeaderboardEntryVO>();
        if (level < 1) return result;
        string raw = read(StorageKey(expert, level));
        if (!string.IsNullOrEmpty(raw))
        {
            try
            {
                var rows = FugaJSON.jsonDecode(raw) as List<object>;
                if (rows != null)
                {
                    foreach (object row in rows)
                    {
                        LeaderboardEntryVO entry;
                        if (TryDeserialize(row as Dictionary<string, object>, out entry))
                        {
                            entry.expert = expert;
                            result.Add(entry);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // A damaged/old list must not prevent browsing or erase the best time.
                // Reading does not overwrite the original stored string.
            }
        }
        LeaderboardEntryVO? best = legacyBest == null ? null : legacyBest(expert, level);
        if (best.HasValue && IsValidTime(best.Value.time) &&
            !result.Exists(item => SameTime(item.time, best.Value.time)))
        {
            var entry = best.Value;
            entry.expert = expert;
            result.Add(entry);
        }
        SortAndTrim(result);
        return result;
    }

    public void AddEntry(LeaderboardEntryVO entry, bool expert, int level)
    {
        if (level < 1 || !IsValidTime(entry.time)) return;
        var entries = GetList(expert, level);
        // Replace a best-only imported row with its real dated completion record.
        // Equal times from different real runs remain separate records.
        entries.RemoveAll(item => item.date == default(DateTime) && SameTime(item.time, entry.time));
        entry.expert = expert;
        entry.star = Math.Max(-1, Math.Min(2, entry.star));
        if (string.IsNullOrEmpty(entry.name)) entry.name = "PLAYER";
        if (entry.date == default(DateTime)) entry.date = DateTime.Now;
        entries.Add(entry);
        SortAndTrim(entries);
        var serialized = new List<object>();
        foreach (var item in entries) serialized.Add(Serialize(item));
        write(StorageKey(expert, level), FugaJSON.jsonEncode(serialized));
    }

    private static bool SameTime(float a, float b)
    {
        return Math.Abs(a - b) < 0.0005f;
    }

    private static void SortAndTrim(List<LeaderboardEntryVO> entries)
    {
        entries.Sort((a, b) => {
            int comparison = a.time.CompareTo(b.time);
            return comparison != 0 ? comparison : b.date.CompareTo(a.date);
        });
        if (entries.Count > MaxEntries) entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
    }

    private static string Field(Dictionary<string, object> data, string key)
    {
        object value;
        return data.TryGetValue(key, out value) ? Convert.ToString(value, CultureInfo.InvariantCulture) : string.Empty;
    }

    public static bool TryDeserialize(Dictionary<string, object> data, out LeaderboardEntryVO entry)
    {
        entry = default(LeaderboardEntryVO);
        if (data == null) return false;
        float time;
        string text = Field(data, "time");
        if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out time) &&
            !float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out time)) return false;
        if (!IsValidTime(time)) return false;
        int star;
        if (!int.TryParse(Field(data, "star"), NumberStyles.Integer, CultureInfo.InvariantCulture, out star)) star = -1;
        bool expert;
        bool.TryParse(Field(data, "expert"), out expert);
        DateTime date;
        string dateText = Field(data, "date");
        if (!DateTime.TryParseExact(dateText, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date) &&
            !DateTime.TryParse(dateText, CultureInfo.CurrentCulture, DateTimeStyles.None, out date) &&
            !DateTime.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) date = default(DateTime);
        entry = new LeaderboardEntryVO {
            name = Field(data, "name"), time = time, date = date,
            star = Math.Max(-1, Math.Min(2, star)), expert = expert
        };
        if (string.IsNullOrEmpty(entry.name)) entry.name = "PLAYER";
        return true;
    }

    public static Dictionary<string, object> Serialize(LeaderboardEntryVO entry)
    {
        return new Dictionary<string, object> {
            { "date", entry.date.ToString("O", CultureInfo.InvariantCulture) },
            { "name", entry.name ?? "PLAYER" },
            { "time", entry.time.ToString("R", CultureInfo.InvariantCulture) },
            { "star", entry.star.ToString(CultureInfo.InvariantCulture) },
            { "expert", entry.expert.ToString() }
        };
    }

    private static string ReadStored(string key)
    {
        return UserSettings.I.GetString(key, string.Empty);
    }

    private static void WriteStored(string key, string value)
    {
        UserSettings.I.SetString(key, value);
    }

    private static LeaderboardEntryVO? GetLegacyBest(bool expert, int level)
    {
        float time = UserSettings.I.GetBestTime(level, expert);
        if (!IsValidTime(time)) return null;
        return new LeaderboardEntryVO {
            name = "LOCAL BEST", time = time, expert = expert,
            star = UserSettings.I.GetLevelStar(level, expert) - 1
        };
    }

    public static void ResetLeaderboards()
    {
        // The HD expansion includes levels 21..32, including its final Core.
        for (int level = 1; level <= UserSettings.I.GetMaxLevel(); level++)
            foreach (bool expert in new[] { false, true })
                UserSettings.I.SetString(StorageKey(expert, level), string.Empty);
        UnityEngine.PlayerPrefs.Save();
    }
}
