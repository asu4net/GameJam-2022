using asu4net.HitSystem;
using UnityEngine;

namespace asu4net.LifeSystem
{
    [CreateAssetMenu(fileName = "DamageHitData", menuName = "ScriptableObjects/DamageHitData", order = 1)]
    public class DamageHitData : HitData
    {
        #region Properties & Fields

        public Vector3 direction { get; private set; }
        [field: SerializeField] public float damage { get; private set; }

        #endregion

        #region Methods

        public override HitData Create(Vector3 pointVal, Hitter senderVal)
        {
            var instance = (DamageHitData) base.Create(pointVal, senderVal);
            instance.direction = (point - sender.transform.position).normalized;
            return instance;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nDirection: {direction}\nDamage: {damage}";
        }

        #endregion
    }
}