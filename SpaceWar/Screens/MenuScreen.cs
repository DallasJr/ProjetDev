using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class MenuScreen : Screen {

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture, trophyTexture;
        private float trophyTimer = 0f;
        private float blinkTimer = 0f;
        private float shineOffset = 0f;

        private bool showArrow = true;
        private int selectedIndex = -1;
        private string[] options = { "Jouer", "Instructions", "Options", "Quitter" };

        private List<Texture2D> spaceshipTextures;
        private List<Spaceship> activeSpaceships = new();
        private Random random = new();
        private float spawnTimer = 0f;

        private List<ScoreEntry> topScores;

        public MenuScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            trophyTexture = content.Load<Texture2D>("trophy");
            spaceshipTextures = new List<Texture2D> {
                content.Load<Texture2D>("blue_01"),
                content.Load<Texture2D>("darkgrey_02"),
                content.Load<Texture2D>("green_06"),
                content.Load<Texture2D>("metalic_02"),
                content.Load<Texture2D>("red_01"),
                content.Load<Texture2D>("purple_03"),
                content.Load<Texture2D>("orange_03"),
                content.Load<Texture2D>("large_enemy"),
                content.Load<Texture2D>("enemy_2_3"),
                content.Load<Texture2D>("enemy_1_3")
            };
            topScores = ScoreManager.LoadScores();
        }

        private KeyboardState previousKeyboard;

        public override void Update(GameTime gameTime) {
            KeyboardState currentKeyboard  = Keyboard.GetState();
            if (selectedIndex == -1) {
                if (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Z) || currentKeyboard.IsKeyDown(Keys.Up)) {
                    selectedIndex = 0;
                    setCursorVisibility(true);
                }
            } else {
                if (IsKeyPressed(currentKeyboard, Keys.Down) || IsKeyPressed(currentKeyboard, Keys.S)) {
                    selectedIndex = (selectedIndex + 1) % options.Length;
                    setCursorVisibility(true);
                }
                if (IsKeyPressed(currentKeyboard, Keys.Up) || IsKeyPressed(currentKeyboard, Keys.Z)) {
                    selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
                    setCursorVisibility(true);
                }
                if (IsKeyPressed(currentKeyboard, Keys.Space) || IsKeyPressed(currentKeyboard, Keys.RightAlt)) {
                    switch (selectedIndex) {
                        case 0:
                            game.ChangeScreen(new UsernameScreen(game));
                            break;
                        case 1:
                            game.ChangeScreen(new InstructionsScreen(game));
                            break;
                        case 2:
                            game.ChangeScreen(new OptionsScreen(game));
                            break;
                        default:
                            game.Exit();
                            break;
                    }
                }
            }

            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (blinkTimer >= 0.5f) {
                setCursorVisibility(!showArrow);
            }
            trophyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            shineOffset += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
            if (shineOffset > "SpaceWar".Length) shineOffset -= "SpaceWar".Length;
            previousKeyboard = currentKeyboard;
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnTimer >= 2f) {
                spawnTimer = 0f;

                Texture2D texture = spaceshipTextures[random.Next(spaceshipTextures.Count)];
                int directionType = random.Next(3);

                Vector2 position;
                Vector2 velocity;
                float rotation;

                float speed = random.Next(50, 150);
                float scale = (float)random.NextDouble() * 1f + 0.3f;

                switch (directionType) {
                    case 0:
                        bool fromLeft = random.Next(2) == 0;
                        float yH = random.Next(50, 600);
                        position = fromLeft ? new Vector2(-texture.Width, yH) : new Vector2(1300 + texture.Width, yH);
                        velocity = fromLeft ? new Vector2(speed, 0) : new Vector2(-speed, 0);
                        rotation = fromLeft ? 0f : MathF.PI;
                        break;

                    case 1:
                        bool fromTop = random.Next(2) == 0;
                        float xV = random.Next(100, 1180);
                        position = fromTop ? new Vector2(xV, -texture.Height) : new Vector2(xV, 800 + texture.Height);
                        velocity = fromTop ? new Vector2(0, speed) : new Vector2(0, -speed);
                        rotation = fromTop ? MathF.PI / 2f : -MathF.PI / 2f;
                        break;

                    default:
                        bool fromLeftSide = random.Next(2) == 0;
                        float x = random.Next(-100, 1380);
                        float y = random.Next(2) == 0 ? -texture.Height : 800 + texture.Height;
                        float dx = random.Next(-60, 60);
                        float dy = y < 0 ? random.Next(30, 100) : -random.Next(30, 100);
                        position = new Vector2(x, y);
                        velocity = new Vector2(dx, dy);
                        rotation = MathF.Atan2(dy, dx);
                        break;
                }

                activeSpaceships.Add(new Spaceship(texture, position, velocity, scale, rotation));
            }

            for (int i = activeSpaceships.Count - 1; i >= 0; i--) {
                activeSpaceships[i].Update(gameTime);
                if (activeSpaceships[i].IsOffScreen())
                    activeSpaceships.RemoveAt(i);
            }

        }

        private void setCursorVisibility(bool show) {
            showArrow = show;
            blinkTimer = 0f;
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);
            foreach (var ship in activeSpaceships)
                ship.Draw(spriteBatch);
            Vector2 leaderboardTitlePos = new Vector2((game.ScreenResolution.X / 3 * 2) - 100, 150);
            Vector2 menuPos = new Vector2(100, 300);
            Vector2 titlePos = new Vector2(100, 100);
            string title = "SpaceWar";
            Vector2 currentPos = titlePos;
            for (int i = 0; i < title.Length; i++) {
                float intensity = MathF.Max(0f, 1f - MathF.Abs(i - shineOffset));
                Color letterColor = Color.Lerp(Color.Teal, Color.White, intensity);

                string letter = title[i].ToString();
                spriteBatch.DrawString(game.TitleFont, letter, currentPos, letterColor);

                Vector2 letterSize = game.TitleFont.MeasureString(letter);
                currentPos.X += letterSize.X;
            }
            for (int i = 0; i < options.Length; i++) {
                Color color = (i == selectedIndex) ? Color.Yellow : Color.LightGray;
                string prefix = (i == selectedIndex && showArrow) ? "> " : "  ";
                spriteBatch.DrawString(game.TextFont, prefix + options[i], menuPos + new Vector2(0, i * 30), color);
            }

            string leaderboardTitleString = "Leaderboard";
            spriteBatch.DrawString(game.MidFont, leaderboardTitleString, leaderboardTitlePos, Color.White);
            float pulse = (float)Math.Sin(trophyTimer * 5f) * 0.35f + 0.65f;
            Color trophyColor = Color.White * pulse;
            Vector2 leaderboardTextSize = game.MidFont.MeasureString(leaderboardTitleString);
            Vector2 trophyPos = leaderboardTitlePos + new Vector2(leaderboardTextSize.X + 20, -5);
            spriteBatch.Draw(trophyTexture, trophyPos, null, trophyColor, 0f, Vector2.Zero, 0.035f, SpriteEffects.None, 0f);

            Vector2 scoreStartPos = leaderboardTitlePos + new Vector2(0, 80);
            for (int i = 0; i < Math.Min(topScores.Count, 10); i++) {
                var entry = topScores[i];
                string scoreText = $"{i + 1}. {entry.Name} - {entry.Score} pts";

                Color scoreColor = i switch {
                    0 => Color.Gold,
                    1 => new Color(192, 192, 192),
                    2 => new Color(205, 127, 50),
                    _ => Color.LightGray,
                };

                spriteBatch.DrawString(game.TextFont, scoreText, scoreStartPos + new Vector2(0, i * 25), scoreColor);
            }

        }

        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.ScreenResolution.X, game.ScreenResolution.Y), Color.White);
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, game.ScreenResolution.X, game.ScreenResolution.Y), Color.White);
            for (int x = 0; x < game.ScreenResolution.X; x += stars1Texture.Width)
                for (int y = 0; y < game.ScreenResolution.Y; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y), Color.White);
            for (int x = 0; x < game.ScreenResolution.X; x += stars2Texture.Width)
                for (int y = 0; y < game.ScreenResolution.Y; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y), Color.White);
        }
    }
}
