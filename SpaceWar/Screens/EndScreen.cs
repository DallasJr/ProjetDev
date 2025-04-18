using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class EndScreen : Screen {
        private Texture2D backgroundTexture, stars1Texture, stars2Texture, trophyTexture;
        private float trophyTimer = 0f;

        private int winnerIndex;
        private Player winner {
            get => winnerIndex == 1 ? player1 : player2;
        }
        private Player loser {
            get => winnerIndex == 1 ? player2 : player1;
        }

        private Player player1;
        private Player player2;
        private float gameDuration;
        private Vector2 starsOffset = Vector2.Zero;

        private float blinkTimer = 0f;
        private bool showArrow = true;
        private int selectedIndex = -1;
        private string[] options = { "Rejouer", "Retour au menu" };
        private bool scoreSaved = false;
        private List<ScoreEntry> leaderboardScores;

        public EndScreen(Game1 game, Player p1, Player p2, float gameDuration) : base(game) {
            player1 = p1;
            player2 = p2;
            winnerIndex = player1.Health > 0 ? 1 : 2;
            winner.Winner = true;
            this.gameDuration = gameDuration;
        }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            trophyTexture = content.Load<Texture2D>("trophy");
        }

        private KeyboardState previousKeyboard;

        public override void Update(GameTime gameTime) {
            winner.Update(gameTime);
            KeyboardState currentKeyboard  = Keyboard.GetState();
            if (selectedIndex == -1) {
                if (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Z) || currentKeyboard.IsKeyDown(Keys.Up)) {
                    selectedIndex = 0;
                }
            } else {
                if (IsKeyPressed(currentKeyboard, Keys.Down) || IsKeyPressed(currentKeyboard, Keys.S)) {
                    selectedIndex = (selectedIndex + 1) % options.Length;
                }
                if (IsKeyPressed(currentKeyboard, Keys.Up) || IsKeyPressed(currentKeyboard, Keys.Z)) {
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                }
                if (IsKeyPressed(currentKeyboard, Keys.Space) || IsKeyPressed(currentKeyboard, Keys.RightAlt)) {
                    switch (selectedIndex) {
                        case 0:
                            game.ChangeScreen(new GameScreen(game, player1.Username, player2.Username));
                            break;
                        case 1:
                            game.ChangeScreen(new MenuScreen(game));
                            break;
                    }
                }
            }

            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (blinkTimer >= 0.5f) {
                showArrow = !showArrow;
                blinkTimer = 0f;
            }

            previousKeyboard = currentKeyboard;

            starsOffset -= winner.Velocity * 1f;
            starsOffset.X %= stars1Texture.Width;
            starsOffset.Y %= stars1Texture.Height;
            
            trophyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        }
        
        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.ScreenResolution.X, game.ScreenResolution.Y), Color.White);
            for (int x = -stars1Texture.Width; x < game.ScreenResolution.X + stars1Texture.Width; x += stars1Texture.Width)
                for (int y = -stars1Texture.Height; y < game.ScreenResolution.Y + stars1Texture.Height; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y) + starsOffset, Color.White);
            for (int x = -stars2Texture.Width; x < game.ScreenResolution.X + stars2Texture.Width; x += stars2Texture.Width)
                for (int y = -stars2Texture.Height; y < game.ScreenResolution.Y + stars2Texture.Height; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y) + starsOffset * 1.5f, Color.White);

            Vector2 titlePos = new Vector2(100, 50);
            Vector2 winTextPos = new Vector2(200, 160);
            Vector2 scorePos = new Vector2(100, 250);
            Vector2 winnerScorePos = new Vector2(100, 320);
            Vector2 loserScorePos = new Vector2(100, 400);
            Vector2 menuPos = new Vector2(100, game.ScreenResolution.Y - 120);

            float pulse = (float)Math.Sin(trophyTimer * 5f) * 0.35f + 0.65f;
            Color trophyColor = Color.White * pulse;

            spriteBatch.DrawString(game.TitleFont, "FIN DE PARTIE", titlePos, Color.OrangeRed);
            spriteBatch.DrawString(game.MidFont, $"{winner.Username} GAGNE !", winTextPos, Color.LightGreen);

            spriteBatch.DrawString(game.MidFont, "SCORES :", scorePos, Color.White);

            int winnerScore = CalculateScore(winner);
            int loserScore = CalculateScore(loser);
            string winnerString = $"{winner.Username} : {winnerScore} pts";
            spriteBatch.DrawString(game.MidFont, winnerString, winnerScorePos, Color.Orange);
            Vector2 winTextSize = game.MidFont.MeasureString(winnerString);
            Vector2 trophyPos = winnerScorePos + new Vector2(winTextSize.X + 20, -5);
            spriteBatch.Draw(trophyTexture, trophyPos, null, trophyColor, 0f, Vector2.Zero, 0.035f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(game.TextFont, $"{loser.Username} : {loserScore} pts", loserScorePos, Color.Gray);
            if (!scoreSaved) {
                if (game.AreDefaultOptions()) {
                    ScoreManager.AddScore(winner.Username, winnerScore);
                    ScoreManager.AddScore(loser.Username, loserScore);
                    leaderboardScores = ScoreManager.LoadScores();
                }
                scoreSaved = true;
            }
            if (game.AreDefaultOptions()) {
                int? winnerRank = GetLeaderboardRank(winner.Username, winnerScore);
                if (winnerRank.HasValue) {
                    Vector2 winnerRankPos = winnerScorePos + new Vector2(0, 30);
                    Color rankColor = GetRankColor(winnerRank.Value);
                    spriteBatch.DrawString(game.TextFont, $"#{winnerRank.Value} du leaderboard !", winnerRankPos, rankColor);
                }
                int? loserRank = GetLeaderboardRank(loser.Username, loserScore);
                if (loserRank.HasValue) {
                    Vector2 loserRankPos = loserScorePos + new Vector2(0, 30);
                    Color rankColor = GetRankColor(loserRank.Value);
                    spriteBatch.DrawString(game.TextFont, $"#{loserRank.Value} du leaderboard !", loserRankPos, rankColor);
                }
            }
            Texture2D winTexture = winner.GetTexture();
            spriteBatch.Draw(winTexture, new Vector2(game.ScreenResolution.X / 3 * 2, game.ScreenResolution.Y / 2), null, Color.White, winner.Rotation,
                new Vector2(winTexture.Width / 2, winTexture.Height / 2), 4f, SpriteEffects.None, 0f);

            for (int i = 0; i < options.Length; i++) {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.LightGray;
                string prefix = (i == selectedIndex && showArrow) ? "> " : "  ";
                spriteBatch.DrawString(game.TextFont, prefix + options[i], menuPos + new Vector2(0, i * 30), color);
            }
        }

        private Color GetRankColor(int rank) {
            if (rank == 1) return Color.Gold;
            if (rank == 2) return new Color(192, 192, 192);
            if (rank == 3) return new Color(205, 127, 50);
            return Color.LightGray;
        }

        private int CalculateScore(Player player) { // Calcule de score compétitive avec soft cap
            float precision = (player.TotalBulletsFired > 0) ? player.TotalDamageDealt / (float)player.TotalBulletsFired : 0f;
            float precisionScore = 300f * MathHelper.Clamp(precision, 0f, 1f);

            float aggressionScore = 150f * (1f - (float)Math.Exp(-player.TotalDamageDealt / 50f));
            float dodgeScore = 20f * (float)Math.Sqrt(player.Dodges);
            float healthScore = (float)Math.Pow(player.Health, 0.8f);
            float mobilityRatio = MathHelper.Clamp(player.TimeInMotion / gameDuration, 0.1f, 1f);
            float mobilityScore = 120f * mobilityRatio;
            
            float averageHitDistance = (player.TotalDamageDealt > 0) ? player.TotalHitDistance / player.TotalDamageDealt : 0f;
            float rangeScore = 80f * MathHelper.Clamp(averageHitDistance / 300f, 0f, 1f);

            float winBonus = player.Winner ? 200f : 0f;

            float timeFactor = MathHelper.Clamp(gameDuration / 60f, 0.75f, 1.25f);

            float finalScore = (
                precisionScore +
                aggressionScore +
                dodgeScore +
                healthScore +
                rangeScore +
                mobilityScore +
                winBonus
            ) * timeFactor;

            return (int)finalScore;
        }

        private int? GetLeaderboardRank(string name, int score) {
            for (int i = 0; i < leaderboardScores.Count && i < 10; i++) {
                if (leaderboardScores[i].Name == name && leaderboardScores[i].Score == score) {
                    return i + 1;
                }
            }
            return null;
        }
    }
}
