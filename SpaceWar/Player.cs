using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class Player {
        
        private const float Speed = 15f;
        private const float BoostedSpeed = 30f;
        private const float FireCooldown = 0.25f;
        private const float SlowRotationSpeed = 0.025f;
        private const float FastRotationSpeed = 0.075f;
        private const int MaxBullets = 5;
        private const float BulletRechargeInterval = 1f;
        private const float MaxBoost = 100f;
        private const float BoostRegenDelay = 3f;
        private const float Friction = 0.95f;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        private int health = 100;
        private int bullets = 0;
        private float bulletRechargeTimer = 0f;

        private Texture2D idle, moving, left, right, slow_left, slow_right;
        private Texture2D texture;
        private Keys forwardKey, leftKey, rightKey, fireKey, boostKey;
        
        private List<Projectile> projectiles = new List<Projectile>();
        private float lastFireTime = 0f;
        private int playerIndex;
        
        private ContentManager contentManager;

        private float hitFlashTimer = 0f;
        private const float HitFlashDuration = 0.2f;

        private float boost = MaxBoost;
        private float boostKeyReleaseTimer = 0f;
        private bool isBoosting = false;

        public List<Projectile> GetProjectiles() => projectiles;

        public int Health {
            get => health;
            set {
                if (value < health) {
                    hitFlashTimer = HitFlashDuration;
                }
                health = value;
            }
        }

        public bool IsFlashing => hitFlashTimer > 0;

        public Texture2D GetTexture() => texture;

        public int GetBulletCount() => bullets;
        public float GetBoost() => boost;
        public float GetMaxBoost() => MaxBoost;

        public Player(Vector2 startPosition, Keys up, Keys left, Keys right, Keys fire, Keys boost, int index) {
            Position = startPosition;
            Velocity = Vector2.Zero;
            forwardKey = up;
            leftKey = left;
            rightKey = right;
            fireKey = fire;
            boostKey = boost;
            playerIndex = index;
        }

        public Circle GetBounds() {
            return new Circle(Position, 20f);
        }

        public void LoadContent(ContentManager content) {
            contentManager = content;
            if (playerIndex == 0) {
                idle = content.Load<Texture2D>("main_idle");
                moving = content.Load<Texture2D>("main_moving");
                left = content.Load<Texture2D>("main_left");
                right = content.Load<Texture2D>("main_right");
                slow_left = content.Load<Texture2D>("main_slow_left");
                slow_right = content.Load<Texture2D>("main_slow_right");
            } else {
                idle = content.Load<Texture2D>("sec_idle");
                moving = content.Load<Texture2D>("sec_moving");
                left = content.Load<Texture2D>("sec_left");
                right = content.Load<Texture2D>("sec_right");
                slow_left = content.Load<Texture2D>("sec_slow_left");
                slow_right = content.Load<Texture2D>("sec_slow_right");
            }
            texture = idle;
        }

        public void Update(GameTime gameTime) {
            KeyboardState  keyboard = Keyboard.GetState();
            Texture2D finalTexture = idle;

            float speed = Speed;
            float cosRotation = (float)Math.Cos(Rotation);
            float sinRotation = (float)Math.Sin(Rotation);

            isBoosting = false;
            if (keyboard.IsKeyDown(boostKey) && keyboard.IsKeyDown(forwardKey) && boost > 0) {
                speed = BoostedSpeed;
                isBoosting = true;
                boostKeyReleaseTimer = 0f;
            } else {
                boostKeyReleaseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboard.IsKeyDown(forwardKey)) {
                Velocity += new Vector2(cosRotation, sinRotation) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                finalTexture = moving;
            }
            if (keyboard.IsKeyDown(leftKey) && !keyboard.IsKeyDown(rightKey)) {
                Rotation -= keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                finalTexture = keyboard.IsKeyDown(forwardKey) ? left : slow_left;
            }
            if (!keyboard.IsKeyDown(leftKey) && keyboard.IsKeyDown(rightKey)) {
                Rotation += keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                finalTexture = keyboard.IsKeyDown(forwardKey) ? right : slow_right;
            }
            texture = finalTexture;
            Velocity *= Friction;
            Position += Velocity;

            if (keyboard.IsKeyDown(fireKey) && gameTime.TotalGameTime.TotalSeconds - lastFireTime > FireCooldown) {
                if (bullets > 0) {
                    FireProjectile();
                    bullets--;
                    lastFireTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
            }
            foreach (var projectile in projectiles) {
                projectile.Update(gameTime);
            }
            projectiles.RemoveAll(p => !p.Active);

            if (isBoosting) {
                boost -= 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (boost < 0) boost = 0;
            } else {
                if (boostKeyReleaseTimer >= BoostRegenDelay && boost < MaxBoost) {
                    float boostIncrement = 50f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    boost = Math.Min(boost + boostIncrement, MaxBoost);
                }
            }

            bulletRechargeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (bulletRechargeTimer >= BulletRechargeInterval) {
                bulletRechargeTimer = 0f;
                if (bullets < MaxBullets) {
                    bullets++;
                }
            }

            if (hitFlashTimer > 0) hitFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }


        public void Draw(SpriteBatch spriteBatch) {
            foreach (var projectile in projectiles) {
                if (projectile.Active) {
                    projectile.Draw(spriteBatch);
                }
            }
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation,
                new Vector2(texture.Width / 2, texture.Height / 2), 1.3f, SpriteEffects.None, 0f);

        }

        private void FireProjectile() {
            float offsetDistance = 20f;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * offsetDistance;
            Vector2 projectilePosition = Position + offset;

            Projectile projectile = new Projectile(projectilePosition, Rotation, playerIndex);
            projectile.LoadContent(contentManager);
            projectiles.Add(projectile);
        }
        
    }
}
