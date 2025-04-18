using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceWar {
    public class Player {
        
        private const float FireCooldown = 0.25f;
        private const float SlowRotationSpeed = 0.025f;
        private const float FastRotationSpeed = 0.075f;
        private const float Friction = 0.95f;
        private const float BulletRechargeInterval = 1f;
        private const float MaxBoost = 100f;
        private const float BoostRegenDelay = 3f;
        private const float HealthRegenDelay = 5f;
        private const float HealthRegenInterval = 2f;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        private int health = 100;
        public string Username;
        private float bulletRechargeTimer = 0f;
        private float healthRegenTimer = 0f;

        private Texture2D idle, moving, left, right, slow_left, slow_right;
        private SoundEffect shootSound;
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

        private Rectangle arenaBounds;

        public List<Projectile> GetProjectiles() => projectiles;

        public int Health {
            get => health;
            set {
                if (value < health) {
                    hitFlashTimer = HitFlashDuration;
                    healthRegenTimer = 0f;
                }
                health = value;
            }
        }
        public int Bullets { get; set; }

        public bool IsFlashing => hitFlashTimer > 0;

        public Texture2D GetTexture() => texture;

        public float GetBoost() => boost;
        public float GetMaxBoost() => MaxBoost;

        public int TotalBulletsFired { get; private set; } = 0;
        public int TotalDamageDealt { get; set; } = 0;
        public float TimeInMotion { get; private set; } = 0f;
        public float TotalHitDistance { get; set; } = 0f;

        private DodgeTracker dodgeTracker;
        public int Dodges => dodgeTracker?.Dodges ?? 0;

        public bool Winner { get; set; } = false;

        public GameOptions GameOptions;

        public Player(string username, Vector2 startPosition, Keys up, Keys left, Keys right, Keys fire, Keys boost, int index, Rectangle arenaBounds, GameOptions gameOptions) {
            GameOptions = gameOptions;
            Username = username;
            Position = startPosition;
            Velocity = Vector2.Zero;
            forwardKey = up;
            leftKey = left;
            rightKey = right;
            fireKey = fire;
            boostKey = boost;
            playerIndex = index;
            this.arenaBounds = arenaBounds;
            if (playerIndex == 0) Rotation = MathHelper.Pi;
            dodgeTracker = new DodgeTracker(this);
        }

        public Circle GetBounds() {
            return new Circle(Position, 20f);
        }

        public void LoadContent(ContentManager content) {
            contentManager = content;
            string prefix = playerIndex == 0 ? "main_" : "sec_";
            idle = content.Load<Texture2D>(prefix + "idle");
            moving = content.Load<Texture2D>(prefix + "moving");
            left = content.Load<Texture2D>(prefix + "left");
            right = content.Load<Texture2D>(prefix + "right");
            slow_left = content.Load<Texture2D>(prefix + "slow_left");
            slow_right = content.Load<Texture2D>(prefix + "slow_right");
            texture = idle;
            shootSound = content.Load<SoundEffect>("laser");
        }

        public void Update(GameTime gameTime) {
            KeyboardState  keyboard = Keyboard.GetState();
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float speed = GameOptions.Speed;
            float cosRotation = (float)Math.Cos(Rotation);
            float sinRotation = (float)Math.Sin(Rotation);
            // Movement handling
            isBoosting = false;
            if (keyboard.IsKeyDown(boostKey) && keyboard.IsKeyDown(forwardKey) && boost > 0) {
                speed = GameOptions.BoostedSpeed;
                isBoosting = true;
                boostKeyReleaseTimer = 0f;
            } else {
                boostKeyReleaseTimer += elapsedTime;
            }

            if (keyboard.IsKeyDown(forwardKey)) {
                Velocity += new Vector2(cosRotation, sinRotation) * speed * elapsedTime;
                if (!Winner) TimeInMotion += elapsedTime;
                texture = moving;
            } else {
                texture = idle;
            }
            if (keyboard.IsKeyDown(leftKey) && !keyboard.IsKeyDown(rightKey)) {
                Rotation -= keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                texture = keyboard.IsKeyDown(forwardKey) ? left : slow_left;
            }
            if (!keyboard.IsKeyDown(leftKey) && keyboard.IsKeyDown(rightKey)) {
                Rotation += keyboard.IsKeyDown(forwardKey) ? FastRotationSpeed : SlowRotationSpeed;
                texture = keyboard.IsKeyDown(forwardKey) ? right : slow_right;
            }
            Velocity *= Friction;
            Position += Velocity;
            Position = Vector2.Clamp(Position, new Vector2(arenaBounds.Left, arenaBounds.Top), new Vector2(arenaBounds.Right, arenaBounds.Bottom));
            // Shooting handling
            if (keyboard.IsKeyDown(fireKey) && gameTime.TotalGameTime.TotalSeconds - lastFireTime > FireCooldown) {
                if (Bullets > 0) {
                    FireProjectile();
                    Bullets--;
                    lastFireTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
            }
            // Projectile handling
            for (int i = projectiles.Count - 1; i >= 0; i--) {
                var projectile = projectiles[i];
                projectile.Update(gameTime);
                if (!projectile.Active) {
                    projectiles.RemoveAt(i);
                }
            }
            // Boost handling
            if (isBoosting && !GameOptions.InfiniteBoost) {
                boost -= 60f * elapsedTime;
                if (boost < 0) boost = 0;
            } else {
                if (boostKeyReleaseTimer >= BoostRegenDelay && boost < MaxBoost) {
                    float boostIncrement = 50f * elapsedTime;
                    boost = Math.Min(boost + boostIncrement, MaxBoost);
                }
            }
            // Bullet regeneration
            bulletRechargeTimer += elapsedTime;
            if (bulletRechargeTimer >= BulletRechargeInterval) {
                bulletRechargeTimer = 0f;
                if (Bullets < GameOptions.MaxBullets) {
                    Bullets++;
                }
            }
            // Health regeneration
            healthRegenTimer += elapsedTime;
            if (healthRegenTimer >= HealthRegenDelay + HealthRegenInterval && !Winner) {
                healthRegenTimer = HealthRegenDelay;
                if (health < 100) {
                    health += 10;
                }
            }

            if (hitFlashTimer > 0) hitFlashTimer -= elapsedTime;
        }

        public void UpdateDodging(List<Projectile> enemyProjectiles, float elapsedTime) {
            dodgeTracker.Update(enemyProjectiles, elapsedTime, Velocity);
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
            if (!Winner) TotalBulletsFired++;
            float offsetDistance = 15f;
            Vector2 offset = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * offsetDistance;
            Vector2 projectilePosition = Position + offset;

            Projectile projectile = new Projectile(projectilePosition, Rotation, playerIndex, GameOptions.BulletSpeed);
            projectile.LoadContent(contentManager);
            projectiles.Add(projectile);
            shootSound.Play(volume: 0.2f, pitch: 0f, pan: 0f);
        }
        
    }
}
