using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Spaceship {
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Velocity;
    public float Scale;
    public float Rotation;

    public Spaceship(Texture2D texture, Vector2 startPos, Vector2 velocity, float scale, float rotation) {
        Texture = texture;
        Position = startPos;
        Velocity = velocity;
        Scale = scale;
        Rotation = rotation;
    }

    public void Update(GameTime gameTime) {
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch) {
        float transparency = MathHelper.Clamp(Scale, 0.2f, 1f);
        spriteBatch.Draw(Texture, Position, null, Color.White * transparency, Rotation, 
            new Vector2(Texture.Width / 2f, Texture.Height / 2f), Scale, SpriteEffects.None, 0f);
    }

    public bool IsOffScreen() {
        return Position.X < -200 || Position.X > 1500 || Position.Y < -200 || Position.Y > 900;
    }
}
