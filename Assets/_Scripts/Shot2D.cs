using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace game.shot
{
    public class Shot2D : MonoBehaviour
    {
        public UnityEvent onShotPerformed;
        [field: SerializeField] private BulletController2D bullet { get; set; }
        [field: SerializeField] private float offset { get; set; }
        [field: SerializeField] private float force { get; set; }
        [field: SerializeField] private Transform cannon { get; set; }

        public void Shot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var shotDir = cannon.right;
            
            var spawnPoint = transform.position + shotDir * offset;
            
            var bulletInstance = Instantiate(bullet, spawnPoint, Quaternion.identity);
            onShotPerformed?.Invoke();
            bulletInstance.Fire(this, shotDir * force);
            Debug.DrawLine(spawnPoint, spawnPoint + shotDir * force, Color.red, 0.5f);
        }
    }
}


