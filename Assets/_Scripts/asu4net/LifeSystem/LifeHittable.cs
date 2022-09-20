using asu4net.HitSystem;
using UnityEngine;

namespace asu4net.LifeSystem
{
    public class LifeHittable : Hittable
    {
        [Header("LifeHittable Settings:")]
        [SerializeField] private Life life;
        
        private const string UnhandledHitDataWarn = "LifeHittable can not handle the received hit data";

        private void Awake()
        {
            life.SetMaxLife();
        }

        protected override void HandleHit(HitData data)
        {
            if (data is not DamageHitData dmgHitData)
            {
                Debug.LogWarning(UnhandledHitDataWarn);
                return;
            }
            
            life.Remove(dmgHitData.damage);
        }
    }
}