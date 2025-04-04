using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class Player {
        
        private const float Speed = 4f;
        private const float FireCooldown = 0.5f;
        private const float SlowRotationSpeed = 0.01f;
        private const float FastRotationSpeed = 0.05f;

        public Vector2 Position;
        public float Rotation;
        public int Health = 100;

        private Texture2D idle, moving, left, right, slow_left, slow_right;
        private Texture2D texture;
        private Keys forwardKey, leftKey, rightKey, fireKey;
        
        private List<Projectile> projectiles = new List<Projectile>();
        private float lastFireTime = 0f;
        
        private ContentManager contentManager;

        public Player(Vector2 startPosition, Keys up, Keys left, Keys right, Keys fire) {
            Position = startPosition;
            forwardKey = up;
            leftKey = left;
            rightKey = right;
            fireKey = fire;
        }

        public void LoadContent(ContentManager content) {
            contentManager = content;
            idle = content.Load<Texture2D>("main_idle");
            moving = content.Load<Texture2D>("main_moving");
            left = content.Load<Texture2D>("main_left");
            right = content.Load<Texture2D>("main_right");
            slow_left = content.Load<Texture2D>("main_slow_left");
            slow_right = content.Load<Texture2D>("main_slow_right");
            texture = idle;
        }

        public void Update(GameTime gameTime) {
            KeyboardState  keyboard = Keyboard.GetState();
            Texture2D finalTexture = idle;

            float cosRotation = (float)Math.Cos(Rotation);
            float sinRotation = (float)Math.Sin(Rotation);

            if (keyboard.IsKeyDown(forwardKey)) {
                Position += new Vector2(cosRotation, sinRotation) * Speed;
                finalTexture = moving;
            }
            if (keyboard.IsKeyDown(leftKey) && !keyboard.IsKeyDown(rightKey)) {
                Rotation -= keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                finalTexture = keyboard.IsKeyDown(forwardKey) ? left : slow_left;
            }
            if (!keyboard.IsKeyDown(leftKey) && keyboard.IsKeyDown(rightKey)) {
                Rotation += keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                finalTexture = keyboard.IsKeyDown(forwardKey) ? right : slow_right;
            }
            texture = finalTexture;

            if (keyboard.IsKeyDown(fireKey) && gameTime.TotalGameTime.TotalSeconds - lastFireTime > FireCooldown) {
                FireProjectile();
                lastFireTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }
            foreach (var projectile in projectiles) {
                projectile.Update(gameTime);
            }
            projectiles.RemoveAll(p => !p.Active);
        }

        public void Draw(SpriteBatch spriteBatch) {
            foreach (var projectile in projectiles) {
                projectile.Draw(spriteBatch);
            }
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation,
                new Vector2(texture.Width / 2, texture.Height / 2), 1.3f, SpriteEffects.None, 0f);
        }

        private void FireProjectile() {
            float offsetDistance = 20f;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * offsetDistance;
            Vector2 projectilePosition = Position + offset;

            Projectile projectile = new Projectile(projectilePosition, Rotation);
            projectile.LoadContent(contentManager);
            projectiles.Add(projectile);
        }
    }
}
