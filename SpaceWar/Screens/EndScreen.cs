using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class EndScreen : Screen {
        private SpriteFont font;
        private string winner;

        public EndScreen(Game1 game, Player p1, Player p2) : base(game) {
            winner = p1.Health > 0 ? "Joueur 1 GAGNE !" : "Joueur 2 GAGNE !";
        }

        public override void LoadContent(ContentManager content) {
            font = content.Load<SpriteFont>("DefaultFont");
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                game.ChangeScreen(new MenuScreen(game));
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, "FIN DE PARTIE", new Vector2(100, 100), Color.Yellow);
            spriteBatch.DrawString(font, winner, new Vector2(100, 150), Color.White);
            spriteBatch.DrawString(font, "[ENTRÉE] : Retour au menu", new Vector2(100, 200), Color.White);
        }
    }
}
