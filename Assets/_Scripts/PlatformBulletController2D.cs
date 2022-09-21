using System;
using System.Collections;
using asu4net.Movement;
using asu4net.Movement.Movement2D;
using asu4net.Sensors.Sensors2D;
using UnityEngine;
using UnityEngine.InputSystem;

namespace game.shot
{
    [RequireComponent(typeof(Collider2D))]
    public class PlatformBulletController2D : BulletController2D
    {
        [field: SerializeField] private float playerVerticalImpulse = 30f;
        [field: SerializeField] private float playerLateralImpulse = 30f;
        [field: SerializeField] private Vector2 lateralImpulseDir { get; set; } = new Vector2(0.5f, 0f);
        [field: SerializeField] private float disableWalkTime { get; set; } = .5f;
        [field: SerializeField] private float disableJumpTime { get; set; } = .5f;

        [field: SerializeField] private BoxSensor2D topSensor { get; set; }
        [field: SerializeField] private BoxSensor2D bottomSensor { get; set; }
        [field: SerializeField] private BoxSensor2D leftSensor { get; set; }
        [field: SerializeField] private BoxSensor2D rightSensor { get; set; }
        private Collider2D coll2D { get; set; }
        public enum State { Projectile, Platform }
        private State state { get; set; } = State.Projectile;
        private GameObject player { get; set; }
        
        private const string SnapTag = "snapPlatform";

        protected override void OnAwake()
        {
            player = GameObject.FindWithTag(GameManager.PlayerTag);
            coll2D = GetComponent<Collider2D>();

            var playerRb = player.GetComponent<Rigidbody2D>();
            var playerWalk = player.GetComponent<Walk2D>();
            var playerJump = player.GetComponent<Jump>();
            
            IEnumerator DisableWalk()
            {
                playerWalk.enabled = false;
                yield return new WaitForSeconds(disableWalkTime);
                playerWalk.enabled = true;
            }

            IEnumerator DisableJump()
            {
                playerJump.jumpEnabled = false;
                yield return new WaitForSeconds(disableJumpTime);
                playerJump.jumpEnabled = true;
            }
            
            topSensor.onEnter.AddListener(args =>
            {
                if (state == State.Projectile) BecomePlatform();
                playerJump.StartCoroutine(DisableJump());
                playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                playerRb.AddForce(Vector2.up * playerVerticalImpulse, ForceMode2D.Impulse);
                Dispose();
            });
            
            bottomSensor.onEnter.AddListener(args =>
            {
                if (state == State.Projectile) BecomePlatform();
                playerJump.StartCoroutine(DisableJump());
                playerJump.jumpEnabled = false;
                playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                playerRb.AddForce(Vector2.up * playerVerticalImpulse, ForceMode2D.Impulse);
                Dispose();
            });
            
            leftSensor.onEnter.AddListener(args =>
            {
                if (state == State.Projectile) BecomePlatform();
                playerJump.StartCoroutine(DisableJump());
                playerWalk.StartCoroutine(DisableWalk());
                playerWalk.SetLookDir(-1);
                var impulse = lateralImpulseDir;
                impulse.x *= -1;
                playerRb.AddForce(impulse * playerLateralImpulse, ForceMode2D.Impulse);
                Dispose();
            });
            
            rightSensor.onEnter.AddListener(args =>
            {
                if (state == State.Projectile) BecomePlatform();
                playerJump.StartCoroutine(DisableJump());
                playerWalk.StartCoroutine(DisableWalk());
                playerWalk.SetLookDir(1);
                playerRb.AddForce(lateralImpulseDir * playerLateralImpulse, ForceMode2D.Impulse);
                Dispose();
            });
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
            topSensor.onEnter.RemoveAllListeners();
            rightSensor.onEnter.RemoveAllListeners();
            bottomSensor.onEnter.RemoveAllListeners();
            leftSensor.onEnter.RemoveAllListeners();
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (state != State.Projectile) return;

            if (other.tag.Contains(SnapTag))
                BecomePlatform();
        }
    }
}