using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class GameScreen : Screen {
        private Player player1, player2;
        private float gameDuration = 0f;
        private Texture2D pixel;

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture;
        private SoundEffect countdownSound, explode1, explode2;

        private List<Explosion> explosions = new();
        private ContentManager contentManager;
        private Rectangle arenaBounds;
        
        private int countdown = 3;
        private float countdownTimer = 0f;
        private bool countdownStarted = false;
        private bool gameStarted = false;
        private bool showGo = false;
        private float goDisplayTime = 0f;
        private const float goDuration = 1f;

        public GameScreen(Game1 game, string player1Username, string player2Username) : base(game) {
            arenaBounds = new Rectangle(-10, -10, 1290, 730);
            Vector2 arenaCenter = new Vector2(
                arenaBounds.Left + arenaBounds.Width / 2f,
                arenaBounds.Top + arenaBounds.Height / 2f
            );
            Vector2 leftHalfCenter = new Vector2(
                arenaBounds.Left + arenaBounds.Width / 4f,
                arenaCenter.Y
            );
            Vector2 rightHalfCenter = new Vector2(
                arenaBounds.Left + 3 * arenaBounds.Width / 4f,
                arenaCenter.Y
            );
            player1 = new Player(player1Username, leftHalfCenter, Keys.Z, Keys.Q, Keys.D, Keys.Space, Keys.LeftShift, 0, arenaBounds, game.GameOptions);
            player2 = new Player(player2Username, rightHalfCenter, Keys.Up, Keys.Left, Keys.Right, Keys.RightControl, Keys.RightAlt, 1, arenaBounds, game.GameOptions);
        }

        public override void LoadContent(ContentManager content) {
            contentManager = content;
            player1.LoadContent(content);
            player2.LoadContent(content);

            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            
            countdownSound = content.Load<SoundEffect>("countdown");
            explode1 = content.Load<SoundEffect>("explode-1");
            explode2 = content.Load<SoundEffect>("explode-2");

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime) {
            if (!gameStarted) {
                if (!countdownStarted) {
                    countdownSound.Play(volume: 0.2f, pitch: 0f, pan: 0f);
                    countdownStarted = true;
                    countdownTimer = 0f;
                }
                countdownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countdown > 0 && countdownTimer >= 1f) {
                    countdown--;
                    countdownTimer = 0f;

                    if (countdown == 0) {
                        gameStarted = true;
                        showGo = true;
                        goDisplayTime = 0f;
                    }
                }
            } else {
                if (showGo) {
                    goDisplayTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (goDisplayTime >= goDuration) {
                        showGo = false;
                    }
                }
                gameDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                player1.Update(gameTime);
                player2.Update(gameTime);
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player1.UpdateDodging(player2.GetProjectiles(), elapsed);
                player2.UpdateDodging(player1.GetProjectiles(), elapsed);

                if (player1.Health <= 0 || player2.Health <= 0) {
                    explode1.Play(volume: 0.2f, pitch: 0f, pan: 0f);
                    game.ChangeScreen(new EndScreen(game, player1, player2, gameDuration));
                }
                for (int i = explosions.Count - 1; i >= 0; i--) {
                    explosions[i].Update(gameTime);
                    if (explosions[i].IsExpired()) explosions.RemoveAt(i);
                }
                CheckProjectileCollisions();
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);
            foreach (var explosion in explosions) explosion.Draw(spriteBatch);
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            if (!gameStarted || showGo) {
                string countdownText = countdown > 0 ? $"{countdown}" : "GO";
                Vector2 textSize = game.MidFont.MeasureString(countdownText);
                Vector2 textPosition = new Vector2(game.ScreenResolution.X / 2, game.ScreenResolution.Y / 2);
                spriteBatch.DrawString(game.MidFont, countdownText, textPosition, Color.Teal, 0f,
                    textSize / 2f, 1f, SpriteEffects.None, 0f);
            }
            if (gameStarted) {
                DrawUI(spriteBatch);
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

        private void DrawUI(SpriteBatch spriteBatch) {
            const int barWidth = 235, healthBarHeight = 20, boostBarHeight = 5, maxHealth = 100;

            spriteBatch.DrawString(game.TextFont, player1.Username, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(game.TextFont, player2.Username, new Vector2(game.ScreenResolution.X - barWidth - 10, 10), Color.White);

            float healthRatio1 = MathHelper.Clamp(player1.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar1 = new Rectangle(10, 35, barWidth, healthBarHeight);
            Rectangle fgBar1 = new Rectangle(10, 35, (int)(barWidth * healthRatio1), healthBarHeight);
            spriteBatch.Draw(pixel, bgBar1, Color.DarkRed);
            spriteBatch.Draw(pixel, fgBar1, Color.Red); 
            Color barColor1 = player1.IsFlashing ? Color.White : Color.Red;
            spriteBatch.Draw(pixel, fgBar1, barColor1);

            float healthRatio2 = MathHelper.Clamp(player2.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar2 = new Rectangle(game.ScreenResolution.X - barWidth - 10, 35, barWidth, healthBarHeight);
            Rectangle fgBar2 = new Rectangle(game.ScreenResolution.X - barWidth - 10, 35, (int)(barWidth * healthRatio2), healthBarHeight);
            spriteBatch.Draw(pixel, bgBar2, Color.DarkRed);
            spriteBatch.Draw(pixel, fgBar2, Color.Red);
            Color barColor2 = player2.IsFlashing ? Color.White : Color.Red;
            spriteBatch.Draw(pixel, fgBar2, barColor2);

            float boostRatio1 = MathHelper.Clamp(player1.GetBoost() / player1.GetMaxBoost(), 0, 1);
            Rectangle bgBoostBar1 = new Rectangle(10, healthBarHeight + 38, barWidth, boostBarHeight);
            Rectangle fgBoostBar1 = new Rectangle(10, healthBarHeight + 38, (int)(barWidth * boostRatio1), boostBarHeight);
            spriteBatch.Draw(pixel, bgBoostBar1, Color.DarkOrange);
            spriteBatch.Draw(pixel, fgBoostBar1, Color.Yellow);

            float boostRatio2 = MathHelper.Clamp(player2.GetBoost() / player2.GetMaxBoost(), 0, 1);
            Rectangle bgBoostBar2 = new Rectangle(game.ScreenResolution.X - barWidth - 10, healthBarHeight + 38, barWidth, boostBarHeight);
            Rectangle fgBoostBar2 = new Rectangle(game.ScreenResolution.X - barWidth - 10, healthBarHeight + 38, (int)(barWidth * boostRatio2), boostBarHeight);
            spriteBatch.Draw(pixel, bgBoostBar2, Color.DarkOrange);
            spriteBatch.Draw(pixel, fgBoostBar2, Color.Yellow);

            Color bulletTextColor1 = player1.Bullets == 0 ? Color.Red : Color.White;
            Color bulletTextColor2 = player2.Bullets == 0 ? Color.Red : Color.White;
            spriteBatch.DrawString(game.TextFont, $"Munitions: {player1.Bullets}", new Vector2(10, 65), bulletTextColor1);
            spriteBatch.DrawString(game.TextFont, $"Munitions: {player2.Bullets}", new Vector2(game.ScreenResolution.X - barWidth - 10, 65), bulletTextColor2);

            spriteBatch.Draw(player1.GetTexture(), new Vector2(barWidth + 50, 40), null, Color.White, player1.Rotation,
                new Vector2(player1.GetTexture().Width / 2, player1.GetTexture().Height / 2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(player2.GetTexture(), new Vector2(game.ScreenResolution.X - barWidth - 50, 40), null, Color.White, player2.Rotation,
                new Vector2(player2.GetTexture().Width / 2, player2.GetTexture().Height / 2), 1f, SpriteEffects.None, 0f);

        }

        private void CheckProjectileCollisions() {
            List<Projectile> projectiles1 = player1.GetProjectiles();
            List<Projectile> projectiles2 = player2.GetProjectiles();

            foreach (var projectile1 in projectiles1) {
                foreach (var projectile2 in projectiles2) {
                    if (projectile1.Active && projectile2.Active && CirclesIntersect(projectile1.GetBounds(), projectile2.GetBounds())) {
                        AddExplosion(projectile1.Position);
                        AddExplosion(projectile2.Position);
                    
                        projectile1.Active = false;
                        projectile2.Active = false;
                    }
                }
            }

            foreach (var projectile in projectiles1) {
                if (projectile.Active && projectile.OwnerIndex != 1 && CirclesIntersect(projectile.GetBounds(), player2.GetBounds())) {
                    AddExplosion(projectile.Position);
                    projectile.Active = false;
                    float distance = Vector2.Distance(projectile.StartPosition, projectile.Position);
                    player1.TotalHitDistance += distance;
                    player2.Health -= 10;
                    player1.TotalDamageDealt += 10;
                }
            }

            foreach (var projectile in projectiles2) {
                if (projectile.Active && projectile.OwnerIndex != 0 && CirclesIntersect(projectile.GetBounds(), player1.GetBounds())) {
                    AddExplosion(projectile.Position);
                    projectile.Active = false;
                    float distance = Vector2.Distance(projectile.StartPosition, projectile.Position);
                    player2.TotalHitDistance += distance;
                    player1.Health -= 10;
                    player2.TotalDamageDealt += 10;
                }
            }
        }

        private void AddExplosion(Vector2 position) {
            var explosion = new Explosion(position);
            explosion.LoadContent(contentManager);
            explosions.Add(explosion);
            explode2.Play(volume: 0.2f, pitch: 0f, pan: 0f);
        }

        public static bool CirclesIntersect(Circle circle1, Circle circle2) =>
            Vector2.Distance(circle1.Center, circle2.Center) < (circle1.Radius + circle2.Radius);

    }
    
}
