using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class GameScreen : Screen {
        private Player player1, player2;
        private Texture2D pixel;

        private Texture2D backgroundTexture;
        private Texture2D nebulaTexture;
        private Texture2D stars1Texture;
        private Texture2D stars2Texture;

        private List<Explosion> explosions;
        private ContentManager contentManager;

        public GameScreen(Game1 game) : base(game) {
            player1 = new Player(new Vector2(100, 300), Keys.Z, Keys.Q, Keys.D, Keys.Space, Keys.LeftShift, 0);
            player2 = new Player(new Vector2(700, 300), Keys.Up, Keys.Left, Keys.Right, Keys.RightControl, Keys.RightAlt, 1);
            explosions = new List<Explosion>();
        }

        public override void LoadContent(ContentManager content) {
            contentManager = content;
            player1.LoadContent(content);
            player2.LoadContent(content);

            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime) {
            player1.Update(gameTime);
            player2.Update(gameTime);

            if (player1.Health <= 0 || player2.Health <= 0) {
                game.ChangeScreen(new EndScreen(game, player1, player2));
            }

            foreach (var explosion in explosions.ToArray()) {
                explosion.Update(gameTime);
                if (explosion.IsExpired()) {
                    explosions.Remove(explosion);
                }
            }
            
            CheckProjectileCollisions();
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);

            foreach (var explosion in explosions) {
                explosion.Draw(spriteBatch);
            }

            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            // HITBOX DEBUG
            // DrawCircle(spriteBatch, player1.GetBounds(), Color.Blue);
            // DrawCircle(spriteBatch, player2.GetBounds(), Color.Red);
            // foreach (var projectile in player1.GetProjectiles()) {
            //     if (projectile.Active) {
            //         DrawCircle(spriteBatch, projectile.GetBounds(), Color.Green);
            //     }
            // }

            // foreach (var projectile in player2.GetProjectiles()) {
            //     if (projectile.Active) {
            //         DrawCircle(spriteBatch, projectile.GetBounds(), Color.Yellow);
            //     }
            // }

            DrawUI(spriteBatch);
        }

        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            for (int x = 0; x < 1280; x += stars1Texture.Width) {
                for (int y = 0; y < 720; y += stars1Texture.Height) {
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y), Color.White);
                }
            }
            for (int x = 0; x < 1280; x += stars2Texture.Width) {
                for (int y = 0; y < 720; y += stars2Texture.Height) {
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y), Color.White);
                }
            }
        }

        private void DrawUI(SpriteBatch spriteBatch) {
            int barWidth = 200;
            int healthBarHeight = 20;
            int maxHealth = 100;
            int boostBarHeight = 5;

            Color bulletTextColor1 = player1.GetBulletCount() == 0 ? Color.Red : Color.White;
            Color bulletTextColor2 = player2.GetBulletCount() == 0 ? Color.Red : Color.White;
            spriteBatch.DrawString(game.Font, $"Bullets: {player1.GetBulletCount()}", new Vector2(10, 45), bulletTextColor1);
            spriteBatch.DrawString(game.Font, $"Bullets: {player2.GetBulletCount()}", new Vector2(1280 - 208, 45), bulletTextColor2);

            float boostRatio1 = MathHelper.Clamp(player1.GetBoost() / player1.GetMaxBoost(), 0, 1);
            Rectangle bgBoostBar1 = new Rectangle(10, healthBarHeight + 15, barWidth, boostBarHeight);
            Rectangle fgBoostBar1 = new Rectangle(10, healthBarHeight + 15, (int)(barWidth * boostRatio1), boostBarHeight);
            spriteBatch.Draw(pixel, bgBoostBar1, Color.DarkOrange);
            spriteBatch.Draw(pixel, fgBoostBar1, Color.Yellow);

            float boostRatio2 = MathHelper.Clamp(player2.GetBoost() / player2.GetMaxBoost(), 0, 1);
            Rectangle bgBoostBar2 = new Rectangle(1280 - barWidth - 10, healthBarHeight + 15, barWidth, boostBarHeight);
            Rectangle fgBoostBar2 = new Rectangle(1280 - barWidth - 10, healthBarHeight + 15, (int)(barWidth * boostRatio2), boostBarHeight);
            spriteBatch.Draw(pixel, bgBoostBar2, Color.DarkOrange);
            spriteBatch.Draw(pixel, fgBoostBar2, Color.Yellow);

            float healthRatio1 = MathHelper.Clamp(player1.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar1 = new Rectangle(10, 10, barWidth, healthBarHeight);
            Rectangle fgBar1 = new Rectangle(10, 10, (int)(barWidth * healthRatio1), healthBarHeight);
            spriteBatch.Draw(pixel, bgBar1, Color.DarkRed);
            spriteBatch.Draw(pixel, fgBar1, Color.Red); 

            float healthRatio2 = MathHelper.Clamp(player2.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar2 = new Rectangle(1280 - barWidth - 10, 10, barWidth, healthBarHeight);
            Rectangle fgBar2 = new Rectangle(1280 - barWidth - 10, 10, (int)(barWidth * healthRatio2), healthBarHeight);
            spriteBatch.Draw(pixel, bgBar2, Color.DarkRed);
            spriteBatch.Draw(pixel, fgBar2, Color.Red);

            Color barColor1 = player1.IsFlashing ? Color.White : Color.Red;
            Color barColor2 = player2.IsFlashing ? Color.White : Color.Red;
            spriteBatch.Draw(pixel, fgBar1, barColor1);
            spriteBatch.Draw(pixel, fgBar2, barColor2);

            spriteBatch.Draw(player1.GetTexture(), new Vector2(barWidth + 50, 40), null, Color.White, player1.Rotation,
                new Vector2(player1.GetTexture().Width / 2, player1.GetTexture().Height / 2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(player2.GetTexture(), new Vector2(1280 - barWidth - 50, 40), null, Color.White, player2.Rotation,
                new Vector2(player2.GetTexture().Width / 2, player2.GetTexture().Height / 2), 1f, SpriteEffects.None, 0f);

        }

        private void CheckProjectileCollisions() {
            List<Projectile> projectiles1 = player1.GetProjectiles();
            List<Projectile> projectiles2 = player2.GetProjectiles();

            foreach (var projectile1 in projectiles1) {
                foreach (var projectile2 in projectiles2) {
                    if (projectile1.Active && projectile2.Active && CirclesIntersect(projectile1.GetBounds(), projectile2.GetBounds())) {
                        Explosion explosion1 = new Explosion(projectile1.Position);
                        explosion1.LoadContent(contentManager);
                        explosions.Add(explosion1);
                        Explosion explosion2 = new Explosion(projectile2.Position);
                        explosion2.LoadContent(contentManager);
                        explosions.Add(explosion2);
                    
                        projectile1.Active = false;
                        projectile2.Active = false;
                    }
                }
            }

            foreach (var projectile in projectiles1) {
                if (projectile.Active && projectile.OwnerIndex != 1 &&
                    CirclesIntersect(projectile.GetBounds(), player2.GetBounds())) {

                    Explosion explosion = new Explosion(projectile.Position);
                    explosion.LoadContent(contentManager);
                    explosions.Add(explosion);
                    projectile.Active = false;
                    player2.Health -= 10;
                }
            }

            foreach (var projectile in projectiles2) {
                if (projectile.Active && projectile.OwnerIndex != 0 &&
                    CirclesIntersect(projectile.GetBounds(), player1.GetBounds())) {
                
                    Explosion explosion = new Explosion(projectile.Position);
                    explosion.LoadContent(contentManager);
                    explosions.Add(explosion);
                    projectile.Active = false;
                    player1.Health -= 10;
                }
            }
        }

        public static bool CirclesIntersect(Circle circle1, Circle circle2) {
            float distance = Vector2.Distance(circle1.Center, circle2.Center);
            return distance < (circle1.Radius + circle2.Radius);
        }

        public void DrawCircle(SpriteBatch spriteBatch, Circle circle, Color color) {
            Texture2D circleTexture = contentManager.Load<Texture2D>("circle_texture");

            float scale = circle.Radius / (circleTexture.Width / 2f);

            spriteBatch.Draw(circleTexture, circle.Center, null, color, 0f,
                new Vector2(circleTexture.Width / 2, circleTexture.Height / 2), scale, SpriteEffects.None, 0f);
        }

    }
    
}
