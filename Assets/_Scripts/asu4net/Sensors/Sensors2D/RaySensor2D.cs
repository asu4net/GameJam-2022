using UnityEngine;

namespace asu4net.Sensors.Sensors2D
{
    public class RaySensor2D : Sensor
    {
        private Vector2 direction => transform.right;
        
        [Header("RaySensor2D Settings: ")][Space]
        [SerializeField] private int rayRows = 3;
        [SerializeField] private float raySpacing = .1f;
        [SerializeField] private float rayDistance = 10f;
        
        protected override void OnDrawGizmos()
        {
            if (!gizmos) return;
            base.OnDrawGizmos();
            var positions = CalculateRayPositions();
            if (positions is null) return;
            foreach (var position in positions)
                Debug.DrawRay(position, direction * rayDistance, gizmosColor);
        }
        
        protected override bool Cast(ref SensorHit[] hits)
        {
            RaycastHit2D raycastHit2D = default;
            var positions = CalculateRayPositions();
            if (positions is null) return default;
            foreach (var position in positions)
            {
                raycastHit2D = Physics2D.Raycast(position, direction, rayDistance, baseLayer);
                if (raycastHit2D.collider is not null) break;
            }
            
            var isHit = raycastHit2D.collider is not null;
            if (!isHit) return false;
            hits[0] = SensorHit.From(raycastHit2D);
            return true;
        }

        protected override bool CastAll(ref SensorHit[] hits)
        {
            var isHit = false;
            var positions = CalculateRayPositions();
            if (positions is null) return default;
            var raycastHits2D = new RaycastHit2D[hits.Length];
            foreach (var position in positions)
            {
                isHit = Physics2D.RaycastNonAlloc(position, direction, raycastHits2D, rayDistance, baseLayer) > 0;
                if (isHit) break;
            }

            if (!isHit) return false;
            for (var i = 0; i < raycastHits2D.Length; i++)
                hits[i] = SensorHit.From(raycastHits2D[i]);
            return true;
        }
        
        private Vector3[] CalculateRayPositions()
        {
            if (rayRows < 0) return default;
            var positions = new Vector3[rayRows];
            for (var i = 1; i <= rayRows; i++)
            {
                var sign = i % 2 == 0 ? 1 : -1;
                positions[i-1] = origin + Vector3.up * (sign * (i == 1 ? raySpacing /2 : raySpacing) * i);
            }
            return positions;
        }
    }
}