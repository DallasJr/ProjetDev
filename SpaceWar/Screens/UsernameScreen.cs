using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class UsernameScreen : Screen {

        private Texture2D backgroundTexture, nebulaTexture, stars1Texture, stars2Texture, checkMarkTexture;
        private bool[] playerReady = { false, false };
        private string[] letters = {
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
        };
        private string[] specialButtons = { "Retour au menu", "ESPACE", "SUPPR", "PRET" };

        private string[] playerNames = { "", "" };
        private Point[] playerCursors = { new Point(0, 0), new Point(0, 0) };

        private int maxNameLength = 16;
        private int itemsPerRow = 13;
        private int letterRows => (int)Math.Ceiling(letters.Length / (float)itemsPerRow);

        private bool justOpened = true;

        private KeyboardState prevKeyboard;

        public UsernameScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            nebulaTexture = content.Load<Texture2D>("nebula_2");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            checkMarkTexture = content.Load<Texture2D>("check-mark");
        }

        public override void Update(GameTime gameTime) {
            KeyboardState keyboard = Keyboard.GetState();

            if (justOpened) {
                if (keyboard.GetPressedKeys().Length == 0) {
                    justOpened = false;
                }
                prevKeyboard = keyboard;
                return;
            }
            for (int player = 0; player < 2; player++) {
                Keys left = (player == 0) ? Keys.Q : Keys.Left;
                Keys right = (player == 0) ? Keys.D : Keys.Right;
                Keys up = (player == 0) ? Keys.Z : Keys.Up;
                Keys down = (player == 0) ? Keys.S : Keys.Down;
                Keys select = (player == 0) ? Keys.Space : Keys.RightAlt;

                int totalRows = letterRows + 1;
                if (!playerReady[player]) {
                    if (IsKeyPressed(keyboard, left)) {
                        int maxCols = (playerCursors[player].Y < letterRows) ? itemsPerRow : specialButtons.Length;
                        playerCursors[player].X = (playerCursors[player].X - 1 + maxCols) % maxCols;
                    }

                    if (IsKeyPressed(keyboard, right)) {
                        int maxCols = (playerCursors[player].Y < letterRows) ? itemsPerRow : specialButtons.Length;
                        playerCursors[player].X = (playerCursors[player].X + 1) % maxCols;
                    }

                    if (IsKeyPressed(keyboard, up)) {
                        playerCursors[player].Y = (playerCursors[player].Y - 1 + totalRows) % totalRows;

                        int maxCols = (playerCursors[player].Y < letterRows)
                            ? GetLetterRowLength(playerCursors[player].Y)
                            : specialButtons.Length;

                        playerCursors[player].X = Math.Min(playerCursors[player].X, maxCols - 1);
                    }

                    if (IsKeyPressed(keyboard, down)) {
                        playerCursors[player].Y = (playerCursors[player].Y + 1) % totalRows;

                        int maxCols = (playerCursors[player].Y < letterRows)
                            ? GetLetterRowLength(playerCursors[player].Y)
                            : specialButtons.Length;

                        playerCursors[player].X = Math.Min(playerCursors[player].X, maxCols - 1);
                    }
                }

                if (IsKeyPressed(keyboard, select)) {
                    string choice = null;
                    if (playerCursors[player].Y < letterRows) {
                        int index = playerCursors[player].Y * itemsPerRow + playerCursors[player].X;
                        if (index < letters.Length)
                            choice = letters[index];
                    } else if (playerCursors[player].Y == letterRows) {
                        if (playerCursors[player].X < specialButtons.Length)
                            choice = specialButtons[playerCursors[player].X];
                    }

                    if (choice != null) {
                        if (choice == "SUPPR" && playerNames[player].Length > 0) {
                            playerNames[player] = playerNames[player].Substring(0, playerNames[player].Length - 1);
                        } else if (choice == "ESPACE" && playerNames[player].Length < maxNameLength) {
                            playerNames[player] += " ";
                        } else if (choice == "PRET") {
                            if (playerReady[player]) playerReady[player] = false;
                             else if (playerNames[player].Length > 0)  {
                                playerReady[player] = true;
                            }
                            if (playerReady[0] && playerReady[1]) {
                                game.ChangeScreen(new GameScreen(game, playerNames[0], playerNames[1]));
                            }
                        } else if (choice == "Retour au menu") {
                            game.ChangeScreen(new MenuScreen(game));
                        } else if (playerNames[player].Length < maxNameLength) {
                            playerNames[player] += choice;
                        }
                    }
                }
            }

            prevKeyboard = keyboard;
        }

        private int GetLetterRowLength(int row) {
            int fullRows = letters.Length / itemsPerRow;
            if (row < fullRows) return itemsPerRow;
            return letters.Length % itemsPerRow;
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && prevKeyboard.IsKeyUp(key);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            DrawBackground(spriteBatch);
            spriteBatch.DrawString(game.TitleFont, "Entrez vos pseudos:", new Vector2(100, 100), Color.White);
            DrawPlayerNames(spriteBatch);
            DrawLetterGrid(spriteBatch);
            DrawSpecialButtons(spriteBatch);
        }

        private void DrawPlayerNames(SpriteBatch spriteBatch) {
            Vector2 labelPos1 = new Vector2(game.ScreenResolution.X / 6, 220);
            Vector2 checkMarkOffset = new Vector2(190, 0);
            spriteBatch.DrawString(game.MidFont, $"Joueur 1:", labelPos1, Color.White);
            if (playerReady[0]) {
                Vector2 checkPos = labelPos1 + checkMarkOffset;
                spriteBatch.Draw(checkMarkTexture, checkPos, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);

            }
            Vector2 namePos1 = new Vector2(game.ScreenResolution.X / 6, 300);
            string nameDisplay1 = playerNames[0].PadRight(maxNameLength, '_');
            spriteBatch.DrawString(game.TextFont, $"{nameDisplay1}", namePos1, Color.White);

            Vector2 labelPos2 = new Vector2(game.ScreenResolution.X / 6 * 4 - 50, 220);
            spriteBatch.DrawString(game.MidFont, $"Joueur 2:", labelPos2, Color.White);
            if (playerReady[1]) {
                Vector2 checkPos = labelPos2 + checkMarkOffset;
                spriteBatch.Draw(checkMarkTexture, checkPos, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            }
            Vector2 namePos2 = new Vector2(game.ScreenResolution.X / 6 * 4 - 50, 300);
            string nameDisplay2 = playerNames[1].PadRight(maxNameLength, '_');
            spriteBatch.DrawString(game.TextFont, $"{nameDisplay2}", namePos2, Color.White);

        }

        private void DrawLetterGrid(SpriteBatch spriteBatch) {
            Vector2 gridOrigin = new Vector2(game.ScreenResolution.X / 4, game.ScreenResolution.Y / 5 * 3);
            int cellSize = 50;

            for (int i = 0; i < letters.Length; i++) {
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
                Vector2 pos = gridOrigin + new Vector2(col * cellSize, row * cellSize);

                spriteBatch.DrawString(game.TextFont, letters[i], pos, Color.Gray);

                for (int player = 0; player < 2; player++) {
                    if (playerCursors[player].X == col && playerCursors[player].Y == row) {
                        DrawCursor(spriteBatch, pos, player);
                    }
                }
            }
        }

        private void DrawSpecialButtons(SpriteBatch spriteBatch) {
            Vector2 gridOrigin = new Vector2(game.ScreenResolution.X / 4, game.ScreenResolution.Y / 5 * 3);
            int cellSize = 50;
            float spacing = 60f;

            Vector2 specialStart = gridOrigin + new Vector2(0, letterRows * cellSize + 40);
            float currentX = specialStart.X;

            for (int i = 0; i < specialButtons.Length; i++) {
                string label = specialButtons[i];
                Vector2 textSize = game.TextFont.MeasureString(label);
                Vector2 pos = new Vector2(currentX, specialStart.Y);

                Rectangle backgroundRect = new Rectangle((int)pos.X - 10, (int)pos.Y - 5, (int)textSize.X + 20, (int)textSize.Y + 10);
                spriteBatch.Draw(CreateRectTexture((int)textSize.X + 20, (int)textSize.Y + 10), backgroundRect, Color.Black * 0.5f);

                spriteBatch.DrawString(game.TextFont, label, pos, Color.White);

                for (int player = 0; player < 2; player++) {
                    if (playerCursors[player].Y == letterRows && playerCursors[player].X == i) {
                        DrawCursor(spriteBatch, pos, player, textSize);
                    }
                }

                currentX += textSize.X + spacing;
            }
        }
        
        private void DrawCursor(SpriteBatch spriteBatch, Vector2 pos, int player, Vector2? textSize = null) {
            Color color = (player == 0) ? Color.Red : Color.Cyan;
            string bracket = (player == 0) ? "[  " : "  ]";
            Vector2 cursorPos = (player == 0)
                ? pos + new Vector2(-10, 0)
                : pos + new Vector2(textSize?.X - 14 ?? 0, 0);

            spriteBatch.DrawString(game.TextFont, bracket, cursorPos, color);
        }

        private Texture2D CreateRectTexture(int width, int height) {
            Texture2D texture = new Texture2D(game.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            texture.SetData(data);
            return texture;
        }



        private void DrawBackground(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, game.ScreenResolution.X, game.ScreenResolution.Y), Color.White);
            spriteBatch.Draw(nebulaTexture, new Rectangle(0, 0, game.ScreenResolution.X, game.ScreenResolution.Y), Color.White);
            for (int x = 0; x < game.ScreenResolution.X; x += stars1Texture.Width)
                for (int y = 0; y < game.ScreenResolution.Y; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y), Color.White);
            for (int x = 0; x < game.ScreenResolution.X; x += stars2Texture.Width)
                for (int y = 0; y < game.ScreenResolution.Y; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y), Color.White);
        }
    }
}
