using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace asu4net.Movement
{
    [Serializable]
    public abstract class Movable : MonoBehaviour
    {
        public abstract float LookDir { get; set; }
        public abstract float Speed { get; set; }
        public abstract bool IsStopped { get; set; }
        public abstract float MoveDir { get; set; }
        
        public abstract void MoveTowards(Vector3 destination);
        public abstract void SetMoveDirByInput(InputAction.CallbackContext context);
    }
}