using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace asu4net.AttackSystem
{
    public class DirectionalAttack2D : Attack
    {
        #region Properties

        public Vector2 direction { get; set; } = Vector2.right;

        protected override List<object> animationParams =>
            new()
            {
                index,
                direction.x,
                direction.y
            };

        #endregion

        #region Input system events

        public void SetDirByInput(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            
            //Set attack dir to default
            if (dir == Vector2.zero)
            {
                direction = Vector2.right;
                return;
            }
            
            //Character rotation handles negative attack dir
            dir.x = Mathf.Abs(dir.x);
            
            //Remove diagonals
            if (dir.x != 0 && dir.y != 0)
            {
                if (dir.x >= dir.y) dir.y = 0;
                else dir.x = 0;
            }

            direction = dir;
        }

        public void CallByInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Call();
        }

        #endregion
    }
}