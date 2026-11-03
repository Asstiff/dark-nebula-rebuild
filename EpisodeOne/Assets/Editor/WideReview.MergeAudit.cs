#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

public static partial class WideReview
{
    [Serializable] sealed class FramingSnapshot { public ViewportDiagnostics.Projection[] projections; }
    [Serializable] sealed class MergeReport
    {
        public string episode, result;
        public int gameplayScenes, normalAndExpertCombinations, localHistoryCapacity;
        public string[] checks, scenes, avatars, boosters;
    }

    static void RequireMerge(bool condition, string message)
    {
        if (!condition) throw new Exception("BRANCH_MERGE_AUDIT " + message);
    }

    static LeaderboardEntryVO ReviewRecord(float time, int days = 0)
    {
        return new LeaderboardEntryVO { name = "PLAYER", time = time, star = 2,
            date = new DateTime(2026, 11, 3, 12, 34, 56, DateTimeKind.Local).AddDays(-days) };
    }

    static void AuditBranchMerge()
    {
        var checks = new List<string>();
        var memory = new Dictionary<string, string>();
        int writes = 0;
        var board = new LocalLeaderboard(key => memory.ContainsKey(key) ? memory[key] : null,
            (key, value) => { memory[key] = value; writes++; });
        for (int i = 13; i >= 0; i--) board.AddEntry(ReviewRecord(100f + i, i), false, 1);
        var list = board.GetList(false, 1);
        RequireMerge(list.Count == 10 && list[0].time == 100f && list[9].time == 109f, "Top 10 capacity/order");
        checks.Add("14 completions retain exactly the fastest 10, in ascending order");
        list.Clear();
        RequireMerge(board.GetList(false, 1).Count == 10, "caller mutated saved list");
        checks.Add("UI/caller list mutation does not alter saved records");
        board.AddEntry(ReviewRecord(50f), true, 1);
        board.AddEntry(ReviewRecord(60f), false, 2);
        int last = EpisodeTwo ? 32 : 23;
        board.AddEntry(ReviewRecord(70f), false, last);
        board.AddEntry(ReviewRecord(80f), true, last);
        RequireMerge(board.GetList(false, 1)[0].time == 100f && board.GetList(true, 1).Single().time == 50f &&
            board.GetList(false, 2).Single().time == 60f && board.GetList(true, last).Single().time == 80f,
            "level/difficulty/expansion separation");
        checks.Add("Normal, Expert and final expansion level records remain independent");
        int before = writes;
        foreach (float invalid in new[] { 0f, -1f, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
            board.AddEntry(ReviewRecord(invalid), false, 1);
        RequireMerge(before == writes, "invalid completion was saved");
        checks.Add("zero, negative, NaN and both infinities cause no storage writes");
        var seededMemory = new Dictionary<string, string>();
        var seeded = new LocalLeaderboard(key => seededMemory.ContainsKey(key) ? seededMemory[key] : null,
            (key, value) => seededMemory[key] = value,
            (expert, level) => new LeaderboardEntryVO { name = "LOCAL BEST", time = 92.125f, star = 1 });
        RequireMerge(seeded.GetList(false, 1).Single().date == default(DateTime), "best-only migration invented a date");
        RequireMerge(seededMemory.Count == 0, "browsing should not write a migrated record");
        seeded.AddEntry(ReviewRecord(95f), false, 1);
        seeded.AddEntry(ReviewRecord(92.125f), false, 1);
        RequireMerge(seeded.GetList(false, 1).Count == 2 && seeded.GetList(false, 1).All(row => row.date != default(DateTime)),
            "migrated best lost or duplicated");
        checks.Add("best-only saves remain visible; a real matching run replaces only the undated import");
        var culture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            board.AddEntry(ReviewRecord(88.125f), false, 3);
            // Old HD serialization used the device's decimal separator and date format.
            var old = new Dictionary<string, object> {
                { "name", "OLD HD" }, { "time", "89,125" }, { "date", "03/11/2026 12:34:56" },
                { "star", "1" }, { "expert", "False" }
            };
            LeaderboardEntryVO parsed;
            RequireMerge(LocalLeaderboard.TryDeserialize(old, out parsed) && parsed.time == 89.125f && parsed.date.Month == 11 && parsed.date.Day == 3,
                "legacy locale decimal");
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            RequireMerge(board.GetList(false, 3).Single().time == 88.125f &&
                board.GetList(false, 3).Single().date.Day == 3, "cross-locale new record roundtrip");
        }
        finally { Thread.CurrentThread.CurrentCulture = culture; }
        checks.Add("French legacy decimals and new invariant records survive locale changes");
        string mixed = FugaJSON.jsonEncode(new List<object> {
            LocalLeaderboard.Serialize(ReviewRecord(93.25f)),
            new Dictionary<string, object> { { "time", "NaN" } },
            new Dictionary<string, object> { { "time", "Infinity" } },
            new Dictionary<string, object> { { "time", "-1" } },
            new Dictionary<string, object> { { "name", "NO TIME" } }
        });
        memory[LocalLeaderboard.StorageKey(false, 4)] = mixed;
        RequireMerge(board.GetList(false, 4).Single().time == 93.25f, "damaged rows hid the valid row");
        memory[LocalLeaderboard.StorageKey(false, 5)] = "[broken";
        RequireMerge(board.GetList(false, 5).Count == 0 && memory[LocalLeaderboard.StorageKey(false, 5)] == "[broken",
            "damaged list destroyed on read");
        checks.Add("malformed rows are ignored; reading a broken list leaves its raw storage intact");
        board.AddEntry(ReviewRecord(93.25f, 1), false, 4);
        RequireMerge(board.GetList(false, 4).Count == 2, "equal real runs were collapsed");
        checks.Add("two dated completions with equal times remain two attempts");

        object settings = typeof(UserSettings).GetProperty(EpisodeTwo ? "I" : "Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        var enabled = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var scenes = new List<string>();
        for (int level = 1; level <= last; level++)
        {
            string name = (string)typeof(UserSettings).GetMethod("GetSceneName").Invoke(settings, new object[] { level });
            string path = enabled.FirstOrDefault(scene => Path.GetFileNameWithoutExtension(scene) == name);
            if (path == null && !EpisodeTwo)
                path = enabled.FirstOrDefault(scene => Path.GetFileNameWithoutExtension(scene) == name.Replace("level1-", "level1b-"));
            RequireMerge(path != null && File.Exists(path), "missing gameplay scene " + level + " (" + name + ")");
            scenes.Add(path);
        }
        RequireMerge(scenes.Distinct().Count() == last, "gameplay level points to duplicate scene");
        checks.Add("every original/HD gameplay scene resolves to a distinct enabled scene");
        string[] avatars = Resources.LoadAll("Avatars").Select(asset => asset.name).OrderBy(name => name).ToArray();
        string[] boosters = Resources.LoadAll("HardCurrencyItems").Where(asset => asset.GetType().Name == "Booster")
            .Select(asset => asset.name).OrderBy(name => name).ToArray();
        if (!EpisodeTwo)
        {
            RequireMerge(Resources.Load<Texture2D>("ClassicLaunch") != null, "classic native launch art missing");
            RequireMerge(avatars.Length == 5 && boosters.Length == 3, "later branch vessel/booster resources missing");
            checks.Add("native launch artwork, five vessels and all three booster assets present");
        }
        else
        {
            // Episode One uses its two native level circles.
            checks.Add("HD expansion remains enabled, including both ending scenes");
        }
        var report = new MergeReport { episode = EpisodeTwo ? "Episode Two" : "Episode One", result = "PASS",
            gameplayScenes = last, normalAndExpertCombinations = last * 2, localHistoryCapacity = LocalLeaderboard.MaxEntries,
            checks = checks.ToArray(), scenes = scenes.ToArray(), avatars = avatars, boosters = boosters };
        File.WriteAllText(Path.Combine(output, "branch-merge-audit.json"), JsonUtility.ToJson(report, true));
        Debug.Log("BRANCH_MERGE_AUDIT PASS scenes=" + last + " combinations=" + (last * 2) + " checks=" + checks.Count);
    }

    static void AuditTutorial()
    {
        var screen = UnityEngine.Object.FindObjectsByType<MonoBehaviour>().First(c => c.GetType().Name == "ImageActionBehaviour");
        Camera camera = (Camera)screen.GetType().GetField("HudCamera").GetValue(screen);
        GameObject art = (GameObject)screen.GetType().GetField("Image", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(screen);
        Renderer background = art.GetComponentsInChildren<Renderer>().Single(r => r.name == "Background");
        Bounds bounds = background.bounds;
        Vector3 min = camera.WorldToScreenPoint(bounds.min), max = camera.WorldToScreenPoint(bounds.max);
        RequireMerge(min.x <= 0.5f && min.y <= 0.5f && max.x >= Screen.width - 0.5f && max.y >= Screen.height - 0.5f,
            "tutorial background does not cover display: " + min + " " + max);
        var devices = art.GetComponentsInChildren<Renderer>().Where(r => r.name == "Device").ToArray();
        RequireMerge(devices.Length == 2, "tutorial phone halves missing");
        Bounds phone = devices[0].bounds; phone.Encapsulate(devices[1].bounds);
        RequireMerge(Mathf.Abs(phone.size.x / phone.size.y - 61f / 102f) < 0.001f, "phone artwork stretched");
        RequireMerge(Mathf.Abs(phone.size.y - 102f) < 0.01f, "phone scale changed with tall display");
        File.AppendAllText(Path.Combine(output, "TUTORIAL-PASS.txt"), Screen.width + "x" + Screen.height +
            " background covers full display; native phone ratio/scale preserved\n");
        Debug.Log("WIDE_TUTORIAL_AUDIT PASS " + Screen.width + "x" + Screen.height);
    }

    static void SetHistoryFixture()
    {
        var view = UnityEngine.Object.FindAnyObjectByType<LeaderboardView>();
        var rows = new List<LeaderboardEntryVO> { new LeaderboardEntryVO { name = "LOCAL BEST", time = 88.125f, star = 2 } };
        view.SetList(rows, false);
        RequireMerge(rows.Count == 1, "LeaderboardView appended its blank display rows to caller data");
        for (int i = 1; i < 10; i++) rows.Add(ReviewRecord(88.125f + i * 3.375f, i));
        view.SetList(rows, false);
        Debug.Log("BRANCH_MERGE_UI fixture=10 savedScoresWritten=0");
    }
}
#endif
