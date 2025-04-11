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
            player1 = new Player(new Vector2(100, 300), Keys.Z, Keys.Q, Keys.D, Keys.Space, 0);
            player2 = new Player(new Vector2(700, 300), Keys.Up, Keys.Left, Keys.Right, Keys.RightControl, 1);
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
            DrawNebula(spriteBatch);
            DrawStars(spriteBatch);

            foreach (var explosion in explosions) {
                explosion.Draw(spriteBatch);
            }

            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            DrawHealthBars(spriteBatch);
        }

        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
        }

        private void DrawNebula(SpriteBatch spriteBatch) {
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, 1280, 720), Color.White);
        }

        private void DrawStars(SpriteBatch spriteBatch) {
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

        private void DrawHealthBars(SpriteBatch spriteBatch) {
            int barWidth = 200;
            int barHeight = 20;
            int maxHealth = 100;

            spriteBatch.DrawString(game.Font, $"HP: {player1.Health}", new Vector2(10, 35), Color.White);
            spriteBatch.DrawString(game.Font, $"HP: {player2.Health}", new Vector2(1280 - 100, 35), Color.White);

            float ratio1 = MathHelper.Clamp(player1.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar1 = new Rectangle(10, 10, barWidth, barHeight);
            Rectangle fgBar1 = new Rectangle(10, 10, (int)(barWidth * ratio1), barHeight);
            
            spriteBatch.Draw(pixel, bgBar1, Color.DarkRed);
            spriteBatch.Draw(pixel, fgBar1, Color.Red); 

            float ratio2 = MathHelper.Clamp(player2.Health / (float)maxHealth, 0, 1);
            Rectangle bgBar2 = new Rectangle(1280 - barWidth - 10, 10, barWidth, barHeight);
            Rectangle fgBar2 = new Rectangle(1280 - barWidth - 10, 10, (int)(barWidth * ratio2), barHeight);
            
            spriteBatch.Draw(pixel, bgBar2, Color.DarkBlue);
            spriteBatch.Draw(pixel, fgBar2, Color.Blue);

            Color barColor1 = player1.IsFlashing ? Color.White : Color.Red;
            Color barColor2 = player2.IsFlashing ? Color.White : Color.Blue;

            spriteBatch.Draw(pixel, fgBar1, barColor1);
            spriteBatch.Draw(pixel, fgBar2, barColor2);

        }

        private void CheckProjectileCollisions() {
            List<Projectile> projectiles1 = player1.GetProjectiles();
            List<Projectile> projectiles2 = player2.GetProjectiles();

            foreach (var projectile1 in projectiles1) {
                foreach (var projectile2 in projectiles2) {
                    if (projectile1.Active && projectile2.Active && projectile1.GetBounds().Intersects(projectile2.GetBounds())) {
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
                    projectile.GetBounds().Intersects(player2.GetBounds())) {

                    Explosion explosion = new Explosion(projectile.Position);
                    explosion.LoadContent(contentManager);
                    explosions.Add(explosion);
                    projectile.Active = false;
                    player2.Health -= 10;
                }
            }

            foreach (var projectile in projectiles2) {
                if (projectile.Active && projectile.OwnerIndex != 0 &&
                    projectile.GetBounds().Intersects(player1.GetBounds())) {
                
                    Explosion explosion = new Explosion(projectile.Position);
                    explosion.LoadContent(contentManager);
                    explosions.Add(explosion);
                    projectile.Active = false;
                    player1.Health -= 10;
                }
            }
        }

    }
    
}
