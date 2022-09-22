using System;
using System.Collections;
using System.Collections.Generic;
using asu4net.Animation;
using asu4net.Sensors;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Sensor = asu4net.Sensors.Sensor;

namespace asu4net.Movement
{
    public abstract class Jump : MonoBehaviour
    {
        #region Properties & inspector fields
        public UnityEvent onJumpPerformed;
        
        protected bool pressed { get; private set; }
        protected bool released { get; private set; }
        protected float verticalSpeed
        {
            get => getVerticalSpeed(); 
            set => setVerticalSpeed(value);
        }

        public bool jumpEnabled { get; set; } = true;
        protected abstract Func<float> getVerticalSpeed { get; set; }
        protected abstract Action<float> setVerticalSpeed { get; set; }
        protected bool canJump { get; set; } = true;
        protected int currentJumps { get; set; }
        protected Coroutine waitCoyoteTime { get; private set; }

        [Separator("Jump settings")] [Space]
        [SerializeField] protected Sensor groundCheckSensor;
        [SerializeField] protected Sensor topSensor;
        [SerializeField] protected int maxJumps = 1;
        [SerializeField] private bool autoJump;
        [SerializeField] protected float coyoteTime = 0.05f;
        [SerializeField] protected float jumpBufferingTime = 0.1f;
        [SerializeField] protected bool clampFallSpeed = true;
        [SerializeField] protected float maxFallSpeed = 15;
        [SerializeField][Range(0, 1)] protected float stopMultiplier = 0.5f;
        [SerializeField] private bool useAnimations;

        [SerializeField] [ConditionalField(nameof(useAnimations))]
        private AnimationManager animationManager;
        [SerializeField] [ConditionalField(nameof(useAnimations))]
        private string jumpAnimationName = Defaults.JumpAnimationName;

        private bool jumpBuffered { get; set; }

        #endregion

        #region Unity life cycle

        protected virtual void Awake()
        {
            groundCheckSensor.onEnter.AddListener(OnGroundEnter);
            groundCheckSensor.onExit.AddListener(OnGroundExit);
        }

        protected void Update()
        {
            if (!useAnimations) return;
            //Animation management
            animationManager.Play(jumpAnimationName, new List<object>()
            {
                groundCheckSensor.isDetecting,
                verticalSpeed
            });
        }

        protected virtual void FixedUpdate()
        {
            if (groundCheckSensor.isDetecting) return;
            
            //On multiple jump -> unlock canJump
            if (!pressed && !canJump && currentJumps < maxJumps - 1)
            {
                currentJumps++;
                canJump = true;
            }

            //Limit fall speed
            if (clampFallSpeed)
                verticalSpeed = Mathf.Clamp(verticalSpeed, -maxFallSpeed, Mathf.Infinity);

            //Stop jump on top collision
            if (verticalSpeed > 0 && topSensor.isDetecting)
                FinishJump();
        }

        #endregion

        #region Sensor event subscribers

        protected virtual void OnGroundEnter(SensorHitEventArgs args)
        {
            ResetJump();
            if (!jumpBuffered) return;
            OnJumpBuffer();
        }

        protected virtual void OnGroundExit(SensorHitEventArgs args)
        {
            if (pressed) return;
            IEnumerator WaitCoyoteTime()
            {
                canJump = true;
                yield return new WaitForSeconds(coyoteTime);
                canJump = false;
            }

            waitCoyoteTime = StartCoroutine(WaitCoyoteTime());
        }

        #endregion

        #region Input System event subscribers

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            SetJumpData(context.performed, context.canceled);
            CheckJumpBuffering(context.performed);
        }

        #endregion

        #region Methods

        public virtual void ResetJump()
        {
            //Allow jump again...
            canJump = true;
            currentJumps = 0;
            if (autoJump) return;
            pressed = false;
        }
        
        protected virtual void OnJumpBuffer() {}

        private void CheckJumpBuffering(bool performed)
        {
            if (!performed || groundCheckSensor.isDetecting) return;
            
            IEnumerator SetJumpBuffered()
            {
                jumpBuffered = true;
                yield return new WaitForSeconds(jumpBufferingTime);
                jumpBuffered = false;
            }

            StartCoroutine(SetJumpBuffered());
        }

        public virtual bool SetJumpData(bool pressedValue, bool releasedValue)
        {
            if (!jumpEnabled) return false;
            
            pressed = pressedValue;
            released = releasedValue;

            if (!pressed || !canJump) return false;
            
            onJumpPerformed?.Invoke();
            
            if (waitCoyoteTime != null)
                StopCoroutine(waitCoyoteTime);
            verticalSpeed = 0;
            return true;
        }

        protected virtual void FinishJump()
        {
            verticalSpeed *= stopMultiplier;
            canJump = false;
            released = false;
        }

        #endregion
    }
}