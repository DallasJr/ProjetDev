using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceWar {
    public class Explosion {
        public Vector2 Position;
        private Texture2D texture;
        private float timer;
        private const float Duration = 0.5f;  // Durée de l'explosion en secondes

        public Explosion(Vector2 position) {
            Position = position;
            timer = Duration;
        }

        public void LoadContent(ContentManager content) {
            texture = content.Load<Texture2D>("explosion-5"); // Assurez-vous que l'image "explosion.png" est bien dans vos ressources
        }

        public void Update(GameTime gameTime) {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (timer > 0) {
                spriteBatch.Draw(texture, Position, null, Color.White, 0f,
                    new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }

        public bool IsExpired() {
            return timer <= 0;
        }
    }
}
