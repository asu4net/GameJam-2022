using asu4net.Movement;
using asu4net.Movement.Movement2D;
using asu4net.Sensors.Sensors2D;
using UnityEngine;

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
        private GameManager gameManager { get; set; }
        private Rigidbody2D playerRb { get; set; }
        private Walk2D playerWalk { get; set; }
        private Jump playerJump { get; set; }
        private bool disposed { get; set; }
        
        private const string SnapTag = "snapPlatform";

        protected override void OnAwake()
        {
            gameManager = GameManager.instance;

            coll2D = GetComponent<Collider2D>();

            playerRb = gameManager.player.GetComponent<Rigidbody2D>();
            playerWalk = gameManager.player.GetComponent<Walk2D>();
            playerJump = gameManager.player.GetComponent<Jump>();

            topSensor.onEnter.AddListener(_ => PushPlayer(Vector2.up));
            bottomSensor.onEnter.AddListener(_ => PushPlayer(Vector2.up));
            leftSensor.onEnter.AddListener(_ => PushPlayer(new Vector2(-lateralImpulseDir.x, lateralImpulseDir.y)));
            rightSensor.onEnter.AddListener(_ => PushPlayer(lateralImpulseDir));
        }
        
        protected override void OnFire(Shot2D sender, Vector2 force)
        {
            state = State.Projectile;
            coll2D.isTrigger = true;
            sender.onShotPerformed.AddListener(Dispose);
        }

        private void PushPlayer(Vector2 direction)
        {
            if (disposed) return;
            
            print("Player pushed in direction: " + direction);
            
            if (state == State.Projectile) BecomePlatform();

            playerRb.velocity = Vector3.zero;

            playerJump.jumpEnabled = false;
            gameManager.WaitAndDo(disableJumpTime, () => playerJump.jumpEnabled = true);

            if (direction.x != 0)
            {
                playerWalk.enabled = false;
                gameManager.WaitAndDo(disableWalkTime, () => playerWalk.enabled = true);
                playerWalk.SetLookDir(direction.x);
            }

            var impulse = direction.x != 0 ? playerLateralImpulse : playerVerticalImpulse;

            playerRb.AddForce(direction * impulse, ForceMode2D.Impulse);
            Dispose();
        }
        
        private void Dispose()
        {
            disposed = true;
            Destroy(gameObject);
        }
        
        private void BecomePlatform()
        {
            state = State.Platform;
            coll2D.isTrigger = false;
            rb.Sleep();
            rb.isKinematic = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (state != State.Projectile) return;

            if (other.tag.Contains(SnapTag))
                BecomePlatform();
        }
    }
}