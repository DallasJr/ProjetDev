using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceWar {
    public class Explosion {
        public Vector2 Position;
        private Texture2D[] frames;
        private float frameTimer = 0f;
        private int currentFrame = 0;
        private float frameDuration = 0.05f;
        private bool isExpired = false;

        public Explosion(Vector2 position) {
            Position = position;
        }

        public void LoadContent(ContentManager content) {
            // Load the 8 frames of the explosion (explosion-1, explosion-2, ...)
            frames = new Texture2D[8];
            for (int i = 0; i < 8; i++) {
                frames[i] = content.Load<Texture2D>($"explosion-{i + 1}");
            }
        }

        public void Update(GameTime gameTime) {
            if (isExpired) return;
            // Scroll frames
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTimer >= frameDuration) {
                frameTimer = 0f;
                currentFrame++;
                if (currentFrame >= frames.Length) {
                    isExpired = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (!isExpired && currentFrame < frames.Length) {
                // Display current frame
                Texture2D currentTexture = frames[currentFrame];
                spriteBatch.Draw(currentTexture, Position, null, Color.White, 0f,
                    new Vector2(currentTexture.Width / 2, currentTexture.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }

        public bool IsExpired() {
            return isExpired;
        }
    }
}
