using System;
using System.Collections;
using asu4net.Movement;
using asu4net.Movement.Movement2D;
using UnityEngine;

namespace game.shot
{
    [RequireComponent(typeof(Collider2D))]
    public class PlatformBulletController2D : BulletController2D
    {
        private const string SnapTag = "snapPlatform";

        private Collider2D coll2D { get; set; }
        public enum State { Projectile, Platform }
        private State state { get; set; } = State.Projectile;

        protected override void OnAwake()
        {
            coll2D = GetComponent<Collider2D>();
        }
        
        protected override void OnFire(Shot2D sender, Vector2 force)
        {
            state = State.Projectile;
            coll2D.isTrigger = true;
            sender.onShotPerformed.AddListener(Dispose);
        }
        private void BecomePlatform()
        {
            state = State.Platform;
            coll2D.isTrigger = false;
            rb.Sleep();
            rb.isKinematic = true;
        }
        
        private void Dispose()
        {
            Destroy(gameObject);
        }

        private void ForceJump(DynamicJump jump)
        {
            jump.ResetJump();
            jump.JumpDuring(jump.maxPressTime);
            Dispose();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (state != State.Projectile) return;

            if (other.tag.Contains(SnapTag))
                BecomePlatform();
            
            else if (other.gameObject.TryGetComponent(out DynamicJump jump))
            {
                BecomePlatform();
                ForceJump(jump);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out DynamicJump jump)) return;
            ForceJump(jump);
        }
    }
}