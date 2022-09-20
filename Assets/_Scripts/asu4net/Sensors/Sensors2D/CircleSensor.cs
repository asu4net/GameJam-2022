using UnityEngine;

namespace asu4net.Sensors.Sensors2D
{
    public class CircleSensor : Sensor
    {
        private float radius => size.magnitude / 4;
        
        protected override void OnDrawGizmos()
        {
            if (!gizmos) return;
            base.OnDrawGizmos();
            Gizmos.DrawSphere(Vector3.zero, .5f);
        }
        
        protected override bool Cast(ref SensorHit[] hits)
        {
            var raycastHit2D = Physics2D.CircleCast(origin, radius,
                Vector3.forward, Mathf.Infinity, baseLayer);
            var isHit = raycastHit2D.collider is not null;
            if (!isHit) return false;
            hits[0] = SensorHit.From(raycastHit2D);
            return true;
        }

        protected override bool CastAll(ref SensorHit[] hits)
        {
            var raycastHits2D = new RaycastHit2D[hits.Length];
            var isHit = Physics2D.CircleCastNonAlloc(origin, radius,
                 Vector3.forward, raycastHits2D, Mathf.Infinity, baseLayer) > 0;
            if (!isHit) return false;
            for (var i = 0; i < raycastHits2D.Length; i++)
                hits[i] = SensorHit.From(raycastHits2D[i]);
            return true;
        }
    }
}