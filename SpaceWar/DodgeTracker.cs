using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SpaceWar {
    public class DodgeTracker {
        private class TrackedProjectile {
            public Projectile Projectile;
            public float TimeAlive;
            public float MinDistanceSquared;
            public Vector2 ClosestPlayerVelocity;

            public TrackedProjectile(Projectile projectile, Vector2 playerPos, Vector2 playerVelocity) {
                Projectile = projectile;
                TimeAlive = 0f;
                MinDistanceSquared = Vector2.DistanceSquared(projectile.Position, playerPos);
                ClosestPlayerVelocity = playerVelocity;
            }

            public void Update(Vector2 playerPos, float elapsed, Vector2 playerVelocity) {
                TimeAlive += elapsed;
                float currentDistanceSquared  = Vector2.DistanceSquared(Projectile.Position, playerPos);
                if (currentDistanceSquared < MinDistanceSquared) {
                    MinDistanceSquared = currentDistanceSquared;
                    ClosestPlayerVelocity = playerVelocity;
                }
            }

            public bool IsDodged(Vector2 playerPos, float dodgeThresholdSquared) {
                float currentDistanceSquared = Vector2.DistanceSquared(Projectile.Position, playerPos);
                bool wasMoving = ClosestPlayerVelocity.LengthSquared() > 1f;
                return wasMoving && currentDistanceSquared > MinDistanceSquared + dodgeThresholdSquared;
            }
        }

        private readonly List<TrackedProjectile> trackedProjectiles = new();
        private readonly Player player;
        private const float DetectionRadius = 100f;
        private const float DodgeCheckDelay = 0.5f;
        private readonly float dodgeThresholdSquared = DetectionRadius * DetectionRadius;

        public int Dodges { get; private set; }

        public DodgeTracker(Player player) {
            this.player = player;
        }

        public void Update(List<Projectile> enemyProjectiles, float elapsed, Vector2 playerVelocity) {
            foreach (var proj in enemyProjectiles) {
                if (proj.Active && Vector2.DistanceSquared(proj.Position, player.Position) < dodgeThresholdSquared && !IsAlreadyTracked(proj)) {
                    trackedProjectiles.Add(new TrackedProjectile(proj, player.Position, playerVelocity));
                }
            }

            for (int i = trackedProjectiles.Count - 1; i >= 0; i--) {
                var tracked = trackedProjectiles[i];

                if (!tracked.Projectile.Active) {
                    trackedProjectiles.RemoveAt(i);
                    continue;
                }

                tracked.Update(player.Position, elapsed, playerVelocity);

                if (tracked.TimeAlive > DodgeCheckDelay && tracked.IsDodged(player.Position, dodgeThresholdSquared)) {
                    Dodges++;
                    trackedProjectiles.RemoveAt(i);
                }
            }
        }

        private bool IsAlreadyTracked(Projectile p) =>
            trackedProjectiles.Any(t => t.Projectile == p);
    }
}
