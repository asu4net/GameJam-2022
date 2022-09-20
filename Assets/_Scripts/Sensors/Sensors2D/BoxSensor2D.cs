using System;
using UnityEngine;

namespace asu4net.Sensors.Sensors2D
{
    public class BoxSensor2D : Sensor
    { 
        private Quaternion rotation => transform.rotation;
        
        protected override void OnDrawGizmos()
        {
            if (!gizmos) return;
            base.OnDrawGizmos();
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
        
        protected override bool Cast(ref SensorHit[] hits)
        {
            var raycastHit2D = Physics2D.BoxCast(origin, size, rotation.eulerAngles.z,
                Vector3.forward, Mathf.Infinity, baseLayer);
            var isHit = raycastHit2D.collider is not null;
            if (!isHit) return false;
            hits[0] = SensorHit.From(raycastHit2D);
            return true;
        }

        protected override bool CastAll(ref SensorHit[] hits)
        {
            var raycastHits2D = new RaycastHit2D[hits.Length];
            var isHit = Physics2D.BoxCastNonAlloc(origin, size, rotation.eulerAngles.z,
                Vector3.forward, raycastHits2D, Mathf.Infinity, baseLayer) > 0;
            if (!isHit) return false;
            for (var i = 0; i < raycastHits2D.Length; i++)
                hits[i] = SensorHit.From(raycastHits2D[i]);
            return true;
        }
    }
}