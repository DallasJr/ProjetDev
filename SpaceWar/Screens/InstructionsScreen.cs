using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpaceWar {
    public class InstructionsScreen : Screen {

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture;
        private Texture2D leftArrow, rightArrow;

        private List<Texture2D> instructionSlides;

        private int currentSlide = 0;
        private int selectedIndex = 0;
        private string returnText = "Retour menu";

        private KeyboardState previousKeyboard;
        private float blinkTimer = 0f;
        private bool showArrow = true;

        private const float ArrowScale = 4f;

        public InstructionsScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");

            instructionSlides = new List<Texture2D> {
                content.Load<Texture2D>("red_01"),
                content.Load<Texture2D>("orange_03"),
                content.Load<Texture2D>("purple_03")
            };

            leftArrow = content.Load<Texture2D>("left_arrow");
            rightArrow = content.Load<Texture2D>("right_arrow");
        }

        public override void Update(GameTime gameTime) {
            { // Navigation
            KeyboardState current = Keyboard.GetState();
            int max = instructionSlides.Count;
            if (IsKeyPressed(current, Keys.Down) || IsKeyPressed(current, Keys.S)) {
                if (selectedIndex == 0 || selectedIndex == 1) {
                    selectedIndex = 2;
                } else {
                    if (currentSlide == max - 1) {
                        selectedIndex = 0;
                    } else if (currentSlide == 0) {
                        selectedIndex = 1;
                    } else {
                        selectedIndex = 1;
                    }
                }
                setCursorVisibility(true);
            }
            if (IsKeyPressed(current, Keys.Up) || IsKeyPressed(current, Keys.Z)) {
                if (selectedIndex == 2) {
                    if (currentSlide == max - 1) {
                        selectedIndex = 0;
                    } else if (currentSlide == 0) {
                        selectedIndex = 1;
                    } else {
                        selectedIndex = 1;
                    }
                } else {
                    selectedIndex = 2;
                }
                setCursorVisibility(true);
            }
            if (IsKeyPressed(current, Keys.Left) || IsKeyPressed(current, Keys.Q)) {
                if ((selectedIndex == 1 || selectedIndex == 2) && currentSlide > 0) {
                    selectedIndex = 0;
                    setCursorVisibility(true);
                }
            }
            if (IsKeyPressed(current, Keys.Right) || IsKeyPressed(current, Keys.D)) {
                if ((selectedIndex == 0 || selectedIndex == 2) && currentSlide != max - 1) {
                    selectedIndex = 1;
                    setCursorVisibility(true);
                }
            }
            if (IsKeyPressed(current, Keys.Space) || IsKeyPressed(current, Keys.RightAlt)) {
                switch (selectedIndex) {
                    case 0:
                        if (currentSlide > 0)
                            currentSlide--;
                        break;
                    case 1:
                        if (currentSlide < max - 1)
                            currentSlide++;
                        break;
                    case 2:
                        game.ChangeScreen(new MenuScreen(game));
                        break;
                }
            }
            previousKeyboard = current;
            }

            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (blinkTimer >= 0.3f) {
                setCursorVisibility(!showArrow);
            }

        }

        private void setCursorVisibility(bool show) {
            showArrow = show;
            blinkTimer = 0f;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);

            Texture2D slide = instructionSlides[currentSlide];
            Vector2 slidePos = new Vector2((1280 - slide.Width) / 2, (720 - slide.Height) / 2 - 50);
            spriteBatch.Draw(slide, slidePos, Color.White);

            Vector2 leftPos = new Vector2(100, 320);
            Vector2 rightPos = new Vector2(1280 - 100 - rightArrow.Width, 320);
            if (currentSlide > 0) {
                DrawArrow(spriteBatch, leftArrow, leftPos, selectedIndex == 0);
            }
            if (currentSlide < instructionSlides.Count - 1) {
                DrawArrow(spriteBatch, rightArrow, rightPos, selectedIndex == 1);
            }
            Vector2 returnPos = new Vector2((1280 - game.TextFont.MeasureString(returnText).X) / 2, 650);
            Color returnColor = (selectedIndex == 2) ? Color.Yellow : Color.White;
            spriteBatch.DrawString(game.TextFont, (selectedIndex == 2 && showArrow) ? "> " + returnText : "  " + returnText, returnPos, returnColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void DrawArrow(SpriteBatch spriteBatch, Texture2D arrow, Vector2 position, bool isSelected) {
            Color color = isSelected ? (showArrow ? Color.White : Color.Gray) : Color.Gray;
            spriteBatch.Draw(arrow, position, null, color, 0f, Vector2.Zero, ArrowScale, SpriteEffects.None, 0f);
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            for (int x = 0; x < 1280; x += stars1Texture.Width)
                for (int y = 0; y < 720; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y), Color.White);
            for (int x = 0; x < 1280; x += stars2Texture.Width)
                for (int y = 0; y < 720; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y), Color.White);
        }
    }
}
