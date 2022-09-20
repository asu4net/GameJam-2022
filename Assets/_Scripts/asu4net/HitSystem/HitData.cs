using UnityEngine;

namespace asu4net.HitSystem
{
    [CreateAssetMenu(fileName = "HitData", menuName = "ScriptableObjects/HitData", order = 0)]
    public class HitData : ScriptableObject
    {
        #region Properties & fields

        public Vector3 point { get; private set; }
        public Hitter sender { get; private set; }

        #endregion

        #region Methods

        public virtual HitData Create(Vector3 pointValue, Hitter senderValue)
        {
            var instance = Instantiate(this);
            instance.sender = senderValue;
            instance.point = pointValue;
            return instance;
        }
        
        public override string ToString()
        {
            return $"Hit Data!\nHit point: {point}\nSender: {sender}";
        }

        #endregion
    }
}