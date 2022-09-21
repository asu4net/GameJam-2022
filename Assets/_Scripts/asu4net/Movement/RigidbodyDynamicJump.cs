using System;
using MyBox;
using UnityEngine;

namespace asu4net.Movement.Movement2D
{
    public class RigidbodyDynamicJump : DynamicJump
    {
        public enum Mode { Jump, Jump2D }

        #region Properties & inspector fields

        protected override Func<float> getVerticalSpeed { get; set; }
        protected override Action<float> setVerticalSpeed { get; set; }
        
        [Separator("RigidbodyDynamicJump settings")] [Space]
        [SerializeField] private Mode mode;

        [SerializeField] [ConditionalField(nameof(mode), false, Mode.Jump)]
        private Rigidbody rb;

        [SerializeField] [ConditionalField(nameof(mode), false, Mode.Jump2D)]
        private Rigidbody2D rb2D;
        
        #endregion

        #region Unity life cycle
        
        protected override void Awake()
        {
            switch (mode)
            {
                case Mode.Jump:
                    getVerticalSpeed = () => rb.velocity.y;
                    setVerticalSpeed = value => rb.velocity = new Vector2(rb.velocity.x, value);
                    break;
                case Mode.Jump2D:
                    getVerticalSpeed = () => rb2D.velocity.y;
                    setVerticalSpeed = value =>
                    {
                        if (!jumpEnabled) return;
                        rb2D.velocity = new Vector2(rb2D.velocity.x, value);
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            base.Awake();
        }

        #endregion
    }
}