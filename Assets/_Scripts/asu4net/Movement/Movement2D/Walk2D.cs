using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using asu4net.Animation;
using game;
using Unity.Mathematics;
using UnityEngine.Events;

//TODO: Move logic to movable (haha fun)
//TODO: Add run support

namespace asu4net.Movement.Movement2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Walk2D : Movable
    {
        public UnityEvent onStep;
        
        public override float Speed { get; set; }
        public float velocity
        {
            get => _rb.velocity.x;
            private set => _rb.velocity = new Vector2(value, _rb.velocity.y);
        }

        public override float LookDir
        {
            get => _lookDir;
            set => SetLookDir((value));
        }
        public bool canMove { get; set; } = true;
        public override bool IsStopped
        {
            get => !canMove && velocity == 0;
            set
            {
                canMove = !value;
                if (canMove) return; //if resuming return
                velocity = 0;
            }
        }
        public override float MoveDir { get; set; }
        public bool wallCollision
        {
            get
            {
                if (!useSensors) return false;
                return wallLeftSensor.isDetecting && MoveDir < 0 ||
                       wallRightSensor.isDetecting && MoveDir > 0;
            }
        }

        [Header("Settings:")] [SerializeField] private float stepInterval = 0.5f;
        [SerializeField] private float startSpeed = 10f;
        [SerializeField] private bool autoLookDir = true;
        [SerializeField] private bool playAnimations;

        [ConditionalField(nameof(playAnimations))] [SerializeField]
        private AnimationManager animationManager;
        
        [ConditionalField(nameof(playAnimations))] [SerializeField]
        private string runAnimationName = Defaults.RunAnimationName;

        [SerializeField] private bool useSensors;
        
        [ConditionalField(nameof(useSensors))] [SerializeField] 
        private Sensors.Sensor wallRightSensor;
        
        [ConditionalField(nameof(useSensors))] [SerializeField] 
        private Sensors.Sensor wallLeftSensor;
        
        private Rigidbody2D _rb;
        private float _lookDir;
        private bool _executeOnStep = true;

        private const float ZeroVerticalSpeed = 0.0001f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            Speed = startSpeed;
        }
        private void Update()
        {
            Move();
            HandleAnimations();

            var sensors = wallRightSensor.transform.parent;
            sensors.rotation = quaternion.Euler(sensors.rotation.x, sensors.rotation.y * -1, sensors.rotation.z);
        }

        public void SetLookDir(float value)
        {
            var dir = Mathf.Sign(value);
            var notChanged = Math.Abs(dir - _lookDir) < 0.001f;
            if (notChanged) return;
            _lookDir = dir;
            var newRot = Quaternion.LookRotation(Vector3.forward * dir);
            _rb.transform.rotation = newRot;
        }
        
        public override void SetMoveDirByInput(InputAction.CallbackContext context)
            => MoveDir = context.ReadValue<float>();
        
        public override void MoveTowards(Vector3 destination)
            => MoveDir = Mathf.Sign(((Vector2) destination - _rb.position).x);

        private void Move()
        {
            if (!canMove) return;

            if (wallCollision)
            {
                velocity = 0;
                return;
            }

            if (MoveDir != 0 && autoLookDir)
                LookDir = MoveDir;

            velocity = Speed * MoveDir;

            if (!_executeOnStep || MoveDir == 0 || Mathf.Abs(_rb.velocity.y) > ZeroVerticalSpeed) return;
            
            onStep?.Invoke();
            _executeOnStep = false;
            GameManager.instance.WaitAndDo(stepInterval, () => _executeOnStep = true);
        }

        private void HandleAnimations()
        {
            if (!playAnimations) return;
            
            var isWalking = canMove && MoveDir != 0 && velocity != 0;
            animationManager.Play(runAnimationName, isWalking);
        }
    }
}