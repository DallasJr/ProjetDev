using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class MenuScreen : Screen {

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture, trophyTexture;
        private float trophyTimer = 0f;
        private float blinkTimer = 0f;
        private bool showArrow = true;
        private int selectedIndex = -1;
        private string[] options = { "Jouer", "Options", "Instructions", "Quitter" };

        public MenuScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            trophyTexture = content.Load<Texture2D>("trophy");
        }

        private KeyboardState previousKeyboard;

        public override void Update(GameTime gameTime) {
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
                            game.ChangeScreen(new GameScreen(game));
                            break;
                        default:
                            game.Exit();
                            break;
                    }
                }
            }

            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (blinkTimer >= 0.5f) {
                showArrow = !showArrow;
                blinkTimer = 0f;
            }
            trophyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            previousKeyboard = currentKeyboard;
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);
            Vector2 leaderboardTitlePos = new Vector2(800, 150);
            spriteBatch.DrawString(game.TitleFont, "SpaceWar", new Vector2(100, 100), Color.Teal);
            Vector2 menuPos = new Vector2(100, 300);
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

        }

        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            for (int x = 0; x < 1280; x += stars1Texture.Width)
                for (int y = 0; y < 720; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y), Color.White);
            for (int x = 0; x < 1280; x += stars2Texture.Width)
                for (int y = 0; y < 720; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y), Color.White);
        }
    }
}
