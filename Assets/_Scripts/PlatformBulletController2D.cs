using asu4net.Movement;
using asu4net.Movement.Movement2D;
using asu4net.Sensors.Sensors2D;
using UnityEngine;

namespace game.shot
{
    [RequireComponent(typeof(Collider2D))]
    public class PlatformBulletController2D : BulletController2D
    {
        [field: SerializeField] private Animator animator { get; set; }
        [field: SerializeField] private float timeToEnable = 0.5f;
        [field: SerializeField] private float timeToDestroy = 1f;
        [field: SerializeField] private float playerVerticalImpulse = 30f;
        [field: SerializeField] private float playerLateralImpulse = 30f;
        [field: SerializeField] private float lateralMultiplierHold = 1.5f;
        [field: SerializeField] private float verticalMultiplierHold = 1.5f;
        [field: SerializeField] private float lateralMultiplierPerfect = 2f;
        [field: SerializeField] private float verticalMultiplierPerfect = 2f;
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
        private Shot2D shot { get; set; }
        private bool platformEnabled { get; set; }

        private const string DestroyParam = "destroy";
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

        private void Update()
        {
            if (shot.released) Dispose();
        }

        protected override void OnFire(Shot2D sender, Vector2 force)
        {
            gameManager.WaitAndDo(timeToEnable, () => platformEnabled = true);
            state = State.Projectile;
            coll2D.isTrigger = true;
            sender.onShotPerformed.AddListener(Dispose);
            shot = sender;
        }

        private void PushPlayer(Vector2 direction)
        {
            if (disposed || !platformEnabled) return;

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

            if (shot.perfect)
            {
                var multiplier = direction.x != 0 ? lateralMultiplierPerfect : verticalMultiplierPerfect;
                impulse *= multiplier;
                shot.onPerfectPush?.Invoke();
            }
            else if (shot.jumpPressed)
            {
                var multiplier = direction.x != 0 ? lateralMultiplierHold : verticalMultiplierHold;
                impulse *= multiplier;
                shot.onPoweredPush?.Invoke();
            }
            else
            {
                shot.onPush?.Invoke();
            }

            playerRb.AddForce(direction * impulse, ForceMode2D.Impulse);
            Dispose();
        }
        
        private void Dispose()
        {
            if (disposed) return;
            disposed = true;
            animator.SetTrigger(DestroyParam);
            GameManager.instance.WaitAndDo(timeToDestroy, () => Destroy(gameObject));
        }
        
        private void BecomePlatform()
        {
            state = State.Platform;
            coll2D.isTrigger = true;
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