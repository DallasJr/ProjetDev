using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Data;
using Microsoft.Data.Sqlite;

public static class ScoreManager {
    private static readonly string DbPath = "data.db";
    static ScoreManager() {
        InitializeDatabase();
    }

    private static void InitializeDatabase() {
        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Scores (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Score INTEGER NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    }

    public static List<ScoreEntry> LoadScores() {
        var scores = new List<ScoreEntry>();
        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Name, Score
            FROM Scores
            ORDER BY Score DESC
            LIMIT 10;
        ";
        using var reader = command.ExecuteReader();
        while (reader.Read()) {
            scores.Add(new ScoreEntry {
                Name = reader.GetString(0),
                Score = reader.GetInt32(1)
            });
        }
        return scores;
    }

    public static void AddScore(string name, int score) {
        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();
        var insert = connection.CreateCommand();
        insert.CommandText = @"
            INSERT INTO Scores (Name, Score)
            VALUES ($name, $score);
        ";
        insert.Parameters.AddWithValue("$name", name);
        insert.Parameters.AddWithValue("$score", score);
        insert.ExecuteNonQuery();
        var delete = connection.CreateCommand();
        delete.CommandText = @"
            DELETE FROM Scores
            WHERE Id NOT IN (
                SELECT Id FROM Scores
                ORDER BY Score DESC
                LIMIT 10
            );
        ";
        delete.ExecuteNonQuery();
    }
}
