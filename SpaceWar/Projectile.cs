using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace SpaceWar {
    public class Projectile {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool Active = true;
        public float Speed = 10f;
        private Texture2D texture;
        
        private float rotation; 

        public Projectile(Vector2 position, float angle) {
            Position = position;
            rotation = angle;
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Speed;
        }

        public void LoadContent(ContentManager content) {
            texture = content.Load<Texture2D>("projectile_1");
        }

        public void Update(GameTime gameTime) {
            Position += Velocity;
            if (Position.X < 0 || Position.X > 1280 || Position.Y < 0 || Position.Y > 720)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (Active)
                spriteBatch.Draw(texture, Position, null, Color.White, rotation,
                    new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public Rectangle GetBounds() {
            return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }
    }
}
