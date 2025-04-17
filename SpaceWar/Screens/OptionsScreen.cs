using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class OptionsScreen : Screen {

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture;

        private string[] labels = {
            "Vitesse",
            "Vitesse Boost",
            "Vitesse Projectile",
            "Munitions Max",
            "Boost Infini",
            "Remettre par defaut",
            "Sauvegarder",
            "Retour"
        };

        private int selectedIndex = 0;
        private bool infiniteBoost = false;

        private float speed;
        private float boostedSpeed;
        private float bulletSpeed;
        private int maxBullets;

        private KeyboardState previousKeyboard;

        public OptionsScreen(Game1 game) : base(game) {}

        public override void LoadContent(ContentManager content) {
            GameOptions current = game.GameOptions;
            speed = current.Speed;
            boostedSpeed = current.BoostedSpeed;
            maxBullets = current.MaxBullets;
            infiniteBoost = current.InfiniteBoost;
            bulletSpeed = current.BulletSpeed;
            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
        }

        public override void Update(GameTime gameTime) {
            KeyboardState current = Keyboard.GetState();

            if (IsKeyPressed(current, Keys.Down) || IsKeyPressed(current, Keys.S))
                selectedIndex = (selectedIndex + 1) % labels.Length;
            if (IsKeyPressed(current, Keys.Up) || IsKeyPressed(current, Keys.Z))
                selectedIndex = (selectedIndex - 1 + labels.Length) % labels.Length;

            if (IsKeyPressed(current, Keys.Left) || IsKeyPressed(current, Keys.Q))
                ModifyValue(-1);
            if (IsKeyPressed(current, Keys.Right) || IsKeyPressed(current, Keys.D))
                ModifyValue(1);

            if (IsKeyPressed(current, Keys.Space)) {
                if (selectedIndex == labels.Length - 1) {
                    game.ChangeScreen(new MenuScreen(game));
                } else if (selectedIndex == labels.Length - 2) {
                    SaveOptions();
                    game.ChangeScreen(new MenuScreen(game));
                } else if (selectedIndex == labels.Length - 3) {
                    ResetToDefault();
                }
            }

            previousKeyboard = current;
        }

        private void ModifyValue(int direction) {
            switch (selectedIndex) {
                case 0:
                    speed = MathHelper.Clamp(speed + direction, 1f, 100f);
                    break;
                case 1:
                    boostedSpeed = MathHelper.Clamp(boostedSpeed + direction, 1f, 200f);
                    break;
                case 2:
                    bulletSpeed = MathHelper.Clamp(bulletSpeed + direction, 1f, 200f);
                    break;
                case 3:
                    maxBullets = MathHelper.Clamp(maxBullets + direction, 1, 20);
                    break;
                case 4:
                    infiniteBoost = !infiniteBoost;
                    break;
            }
        }

        private void ResetToDefault() {
            GameOptions defaultOptions = game.DefaultOptions;
            speed = defaultOptions.Speed;
            boostedSpeed = defaultOptions.BoostedSpeed;
            maxBullets = defaultOptions.MaxBullets;
            bulletSpeed = defaultOptions.BulletSpeed;
            infiniteBoost = defaultOptions.InfiniteBoost;
        }

        private void SaveOptions() {
            GameOptions newOptions = new GameOptions {
                Speed = speed,
                BoostedSpeed = boostedSpeed,
                MaxBullets = maxBullets,
                BulletSpeed = bulletSpeed,
                InfiniteBoost = infiniteBoost
            };
            game.GameOptions = newOptions;
            game.SaveOptions(newOptions);
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) =>
            current.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);
            spriteBatch.DrawString(game.TitleFont, "Options:", new Vector2(100, 100), Color.White);
            Vector2 position = new Vector2(100, 200);

            for (int i = 0; i < labels.Length; i++) {
                string label = labels[i];
                string value = i switch {
                    0 => speed.ToString("0.0"),
                    1 => boostedSpeed.ToString("0.0"),
                    2 => bulletSpeed.ToString("0.0"),
                    3 => maxBullets.ToString(),
                    4 => infiniteBoost ? "Oui" : "Non",
                    _ => ""
                };

                Color color = i == selectedIndex ? Color.Yellow : Color.White;
                string text = (i == selectedIndex && i < 5) ? $"> {label} : {value}" :
                              (i == selectedIndex ? $"> {label}" : label + (value != "" ? $" : {value}" : ""));
                spriteBatch.DrawString(game.TextFont, text, position + new Vector2(0, i * 40), color);
            }

            if (AreOptionsModified()) {
                string warning = "Les scores ne seront pas enregistres dans le leaderboard.";
                Vector2 warningPos = new Vector2(100, game.GraphicsDevice.Viewport.Height - 60);
                spriteBatch.DrawString(game.TextFont, warning, warningPos, Color.OrangeRed);
            }
        }

        public bool AreOptionsModified() {
            GameOptions defaultOptions = game.DefaultOptions;
            return
                speed != defaultOptions.Speed ||
                boostedSpeed != defaultOptions.BoostedSpeed ||
                maxBullets != defaultOptions.MaxBullets ||
                bulletSpeed != defaultOptions.BulletSpeed ||
                infiniteBoost != defaultOptions.InfiniteBoost;
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
