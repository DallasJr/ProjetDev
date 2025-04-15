using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public static class ScoreManager {
    private static readonly string FilePath = "highscores.json";

    public static List<ScoreEntry> LoadScores() {
        if (!File.Exists(FilePath)) return new List<ScoreEntry>();
        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
    }

    public static void SaveScores(List<ScoreEntry> scores) {
        var sorted = scores.OrderByDescending(s => s.Score).Take(10).ToList();
        string json = JsonSerializer.Serialize(sorted, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public static void AddScore(string name, int score) {
        var scores = LoadScores();
        scores.Add(new ScoreEntry { Name = name, Score = score });
        SaveScores(scores);
    }
}
