using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class MenuScreen : Screen {
        private SpriteFont font;

        public MenuScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
            font = content.Load<SpriteFont>("DefaultFont");
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                game.ChangeScreen(new GameScreen(game));
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, "BORNE D'ARCADE", new Vector2(100, 100), Color.White);
            spriteBatch.DrawString(font, "Appuyez sur [ENTRÉE] pour commencer", new Vector2(100, 150), Color.White);
        }
    }
}
