using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class EndScreen : Screen {
        private Texture2D backgroundTexture, stars1Texture, stars2Texture, trophyTexture;
        private float trophyTimer = 0f;

        private int winnerIndex;
        private Player winner {
            get => winnerIndex == 1 ? player1 : player2;
        }
        private Player loser {
            get => winnerIndex == 1 ? player2 : player1;
        }

        private Player player1;
        private Player player2;
        private float gameDuration;
        private Vector2 starsOffset = Vector2.Zero;

        public EndScreen(Game1 game, Player p1, Player p2, float gameDuration) : base(game) {
            player1 = p1;
            player2 = p2;
            winnerIndex = player1.Health > 0 ? 1 : 2;
            winner.Winner = true;
            this.gameDuration = gameDuration;
        }

        public override void LoadContent(ContentManager content) {
            backgroundTexture = content.Load<Texture2D>("background");
            stars1Texture = content.Load<Texture2D>("stars_1");
            stars2Texture = content.Load<Texture2D>("stars_2");
            trophyTexture = content.Load<Texture2D>("trophy");

        }

        public override void Update(GameTime gameTime) {
            winner.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                game.ChangeScreen(new MenuScreen(game));

            starsOffset -= winner.Velocity * 1f;
            starsOffset.X %= stars1Texture.Width;
            starsOffset.Y %= stars1Texture.Height;
            
            trophyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);
            for (int x = -stars1Texture.Width; x < 1280 + stars1Texture.Width; x += stars1Texture.Width)
                for (int y = -stars1Texture.Height; y < 720 + stars1Texture.Height; y += stars1Texture.Height)
                    spriteBatch.Draw(stars1Texture, new Vector2(x, y) + starsOffset, Color.White);

            for (int x = -stars2Texture.Width; x < 1280 + stars2Texture.Width; x += stars2Texture.Width)
                for (int y = -stars2Texture.Height; y < 720 + stars2Texture.Height; y += stars2Texture.Height)
                    spriteBatch.Draw(stars2Texture, new Vector2(x, y) + starsOffset * 1.5f, Color.White);

            Vector2 titlePos = new Vector2(100, 50);
            Vector2 winTextPos = new Vector2(100, 150);
            Vector2 scorePos = new Vector2(100, 250);
            Vector2 winnerScorePos = new Vector2(100, 320);
            Vector2 loserScorePos = new Vector2(100, 370);
            Vector2 hintPos = new Vector2(100, 600);

            float pulse = (float)Math.Sin(trophyTimer * 5f) * 0.35f + 0.65f;
            Color trophyColor = Color.White * pulse;

            spriteBatch.DrawString(game.TitleFont, "FIN DE PARTIE", titlePos, Color.OrangeRed);
            spriteBatch.DrawString(game.MidFont, $"{winner.Username} GAGNE !", winTextPos, Color.LightGreen);

            spriteBatch.DrawString(game.MidFont, "SCORES :", scorePos, Color.White);

            string winnerString = $"{winner.Username} : {CalculateScore(winner)} pts";
            spriteBatch.DrawString(game.MidFont, winnerString, winnerScorePos, Color.Orange);
            Vector2 winTextSize = game.MidFont.MeasureString(winnerString);
            Vector2 trophyPos = winnerScorePos + new Vector2(winTextSize.X + 20, 0);
            spriteBatch.Draw(trophyTexture, trophyPos, null, trophyColor, 0f, Vector2.Zero, 0.035f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(game.TextFont, $"{loser.Username} : {CalculateScore(loser)} pts", loserScorePos, Color.Gray);

            spriteBatch.DrawString(game.TextFont, "[ENTREE] : Retour au menu", hintPos, Color.LightGray);

            Texture2D winTexture = winner.GetTexture();
            spriteBatch.Draw(winTexture, new Vector2(900, 350), null, Color.White, winner.Rotation,
                new Vector2(winTexture.Width / 2, winTexture.Height / 2), 4f, SpriteEffects.None, 0f);
        }

        private int CalculateScore(Player player)
        {
            float baseScore = player.Winner ? 1000f : 500f;
            float healthBonus = player.Health * 5f;
            float aggressionBonus = Math.Min(player.TotalDamageDealt, 100f);
            float bulletEfficiency = (float)player.TotalDamageDealt / Math.Max(1, player.TotalBulletsFired);
            float timeFactor = MathHelper.Clamp(gameDuration / 60f, 0.5f, 1.5f);
            float movementRatio = MathHelper.Clamp(player.TimeInMotion / gameDuration, 0.2f, 1f);
            float farmingPenalty = 0.5f + (movementRatio * 0.5f);
            float finalScore = (baseScore + healthBonus + aggressionBonus + bulletEfficiency * 100f) * timeFactor * farmingPenalty;
            return (int)finalScore;
        }
    }
}
