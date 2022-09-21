using System.Collections;
using MyBox;
using UnityEngine;

namespace asu4net.Movement
{
    public abstract class DynamicJump : Jump
    {
        #region Properties & inspector fields
        private float jumpTime { get; set; }
        
        [Separator("DynamicJump settings")] [Space]
        [SerializeField] private float jumpSpeed = 10;
        [field: SerializeField] public float maxPressTime { get; private set; } = 0.35f;
        #endregion
        
        #region Unity life cycle

        protected override void FixedUpdate()
        {
            HandleJump();
            base.FixedUpdate();
        }

        #endregion

        #region Jump inherited

        protected override void FinishJump()
        {
            base.FinishJump();
            jumpTime = 0;
        }

        protected override void OnJumpBuffer()
        {
            JumpDuring(maxPressTime);
        }

        public override void ResetJump()
        {
            base.ResetJump();
            jumpTime = 0;
        }

        #endregion

        #region Methods

        public void JumpDuring(float seconds)
        {
            IEnumerator PressJump()
            {
                SetJumpData(true, false);
                yield return new WaitForSeconds(seconds);
                SetJumpData(false, true);
            }
            
            StartCoroutine(PressJump());
        }
        
        private void HandleJump()
        {
            //On jump pressed -> apply const speed
            if (pressed && canJump)
            {
                verticalSpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }

            if (groundCheckSensor.isDetecting) return;

            //On jump time finished or release -> reset speed and lock canJump
            if (released || jumpTime >= maxPressTime)
            {
                FinishJump();
            }
        }
        #endregion
    }
}