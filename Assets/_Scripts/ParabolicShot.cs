
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace game
{
    public class ParabolicShot2D : MonoBehaviour
    {
        [SerializeField] private UnityEvent onShotPerformed;
        [field: SerializeField] private GameObject bullet { get; set; }
        [field: SerializeField] private float offset { get; set; }
        [field: SerializeField] private Vector2 force { get; set; }

        private Vector2 direction { get; set; }

        public void SetShotDirection(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
        }
        
        public void Shot(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            var spawnPoint = transform.position + (Vector3) direction * offset;
            var bulletInstance = Instantiate(bullet, spawnPoint, Quaternion.identity);
            var bulletInstanceRb = bulletInstance.GetComponent<Rigidbody2D>();
            bulletInstanceRb.AddForce(force, ForceMode2D.Impulse);
            onShotPerformed?.Invoke();
        }
    }
}


