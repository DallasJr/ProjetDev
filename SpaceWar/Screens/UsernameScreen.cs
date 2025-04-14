using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class UsernameScreen : Screen {

        private string[] alphabet = {
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "ESPACE", "SUPPR", "READY"
        };

        private string[] playerNames = { "", "" };
        private Point[] playerCursors = { new Point(0, 0), new Point(0, 0) };

        private int maxNameLength = 16;
        private int itemsPerRow = 13;
        private int alphabetRows => (int)Math.Ceiling(alphabet.Length / (float)itemsPerRow);

        private KeyboardState prevKeyboard;

        public UsernameScreen(Game1 game) : base(game) { }

        public override void LoadContent(ContentManager content) {
        }

        public override void Update(GameTime gameTime) {
            KeyboardState keyboard = Keyboard.GetState();

            for (int player = 0; player < 2; player++) {
                Keys left = (player == 0) ? Keys.Q : Keys.Left;
                Keys right = (player == 0) ? Keys.D : Keys.Right;
                Keys up = (player == 0) ? Keys.Z : Keys.Up;
                Keys down = (player == 0) ? Keys.S : Keys.Down;
                Keys select = (player == 0) ? Keys.Space : Keys.RightAlt;

                if (IsKeyPressed(keyboard, left)) {
                    playerCursors[player].X = (playerCursors[player].X - 1 + itemsPerRow) % itemsPerRow;
                }
                if (IsKeyPressed(keyboard, right)) {
                    playerCursors[player].X = (playerCursors[player].X + 1) % itemsPerRow;
                }
                if (IsKeyPressed(keyboard, up)) {
                    playerCursors[player].Y = (playerCursors[player].Y - 1 + alphabetRows) % alphabetRows;
                }
                if (IsKeyPressed(keyboard, down)) {
                    playerCursors[player].Y = (playerCursors[player].Y + 1) % alphabetRows;
                }

                if (IsKeyPressed(keyboard, select)) {
                    int index = playerCursors[player].Y * itemsPerRow + playerCursors[player].X;
                    if (index < alphabet.Length) {
                        string choice = alphabet[index];
                        if (choice == "SUPPR" && playerNames[player].Length > 0) {
                            playerNames[player] = playerNames[player].Substring(0, playerNames[player].Length - 1);
                        } else if (choice == "ESPACE" && playerNames[player].Length < maxNameLength) {
                            playerNames[player] += " ";
                        } else if (choice == "READY") {
                            if (player == 1 && playerNames[0].Length > 0 && playerNames[1].Length > 0) {
                                game.ChangeScreen(new GameScreen(game, playerNames[0], playerNames[1]));
                            }
                        } else if (playerNames[player].Length < maxNameLength) {
                            playerNames[player] += choice;
                        }
                    }
                }
            }

            prevKeyboard = keyboard;
        }

        private bool IsKeyPressed(KeyboardState current, Keys key) {
            return current.IsKeyDown(key) && prevKeyboard.IsKeyUp(key);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            for (int i = 0; i < 2; i++) {
                string underline = new string('_', maxNameLength);
                string nameDisplay = playerNames[i].PadRight(maxNameLength, '_');
                Vector2 namePos = new Vector2(100, 100 + i * 60);
                spriteBatch.DrawString(game.TextFont, $"Joueur {i + 1}: {nameDisplay}", namePos, Color.White);
            }

            Vector2 gridOrigin = new Vector2(100, 250);
            int cellSize = 50;

            for (int i = 0; i < alphabet.Length; i++) {
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
                Vector2 pos = gridOrigin + new Vector2(col * cellSize, row * cellSize);
                spriteBatch.DrawString(game.TextFont, alphabet[i], pos, Color.Gray);

                for (int player = 0; player < 2; player++) {
                    if (playerCursors[player].X == col && playerCursors[player].Y == row) {
                        Color color = (player == 0) ? Color.Red : Color.Cyan;
                        spriteBatch.DrawString(game.TextFont, $"[{alphabet[i]}]", pos, color);
                    }
                }
            }
        }
    }
}
