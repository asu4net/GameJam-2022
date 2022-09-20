using System;
using UnityEngine;

namespace game.shot
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletController2D : MonoBehaviour
    {
        protected Rigidbody2D rb { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            OnAwake();
        }
        
        protected virtual void OnAwake() {}
        
        public void Fire(Shot2D sender, Vector2 force)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
            OnFire(sender, force);
        }

        protected virtual void OnFire(Shot2D sender, Vector2 force) { }
    }
}