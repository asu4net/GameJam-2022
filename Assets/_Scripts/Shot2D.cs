using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace game.shot
{
    public class Shot2D : MonoBehaviour
    {
        public UnityEvent onShotPerformed;
        public UnityEvent onPush;
        public UnityEvent onPoweredPush;
        public UnityEvent onPerfectPush;
        
        [field: SerializeField] private BulletController2D bullet { get; set; }
        [field: SerializeField] private float offset { get; set; }
        [field: SerializeField] private float force { get; set; }
        [field: SerializeField] private Transform cannon { get; set; }
        [field: SerializeField] private float cooldown { get; set; } = 1f;
        [field: SerializeField] private float perfectTime { get; set; } = 0.2f;
        
        public bool pressed { get; private set; }
        public bool released { get; private set; }
        public bool perfect { get; private set; }
        
        public bool onCooldown { get; private set; }

        public void Shot(InputAction.CallbackContext context)
        {
            pressed = context.performed;
            released = context.canceled;
            
            if (!pressed || onCooldown) return;

            perfect = true;
            GameManager.instance.WaitAndDo(perfectTime, () => perfect = false);
            
            onCooldown = true;
            GameManager.instance.WaitAndDo(cooldown, () => onCooldown = false);
            
            var shotDir = cannon.right;
            
            var spawnPoint = transform.position + shotDir * offset;
            
            var bulletInstance = Instantiate(bullet, spawnPoint, Quaternion.identity);
            onShotPerformed?.Invoke();
            bulletInstance.Fire(this, shotDir * force);
            Debug.DrawLine(spawnPoint, spawnPoint + shotDir * force, Color.red, 0.5f);
        }
    }
}


