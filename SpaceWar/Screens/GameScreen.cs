using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class GameScreen : Screen {
        private Player player1, player2;

        public GameScreen(Game1 game) : base(game) {
            player1 = new Player(new Vector2(100, 300), Keys.Z, Keys.Q, Keys.D, Keys.Space, 0);
            player2 = new Player(new Vector2(700, 300), Keys.Up, Keys.Left, Keys.Right, Keys.RightControl, 1);
        }

        public override void LoadContent(ContentManager content) {
            player1.LoadContent(content);
            player2.LoadContent(content);
        }

        public override void Update(GameTime gameTime) {
            player1.Update(gameTime);
            player2.Update(gameTime);

            if (player1.Health <= 0 || player2.Health <= 0) {
                game.ChangeScreen(new EndScreen(game, player1, player2));
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
        }
    }
}
