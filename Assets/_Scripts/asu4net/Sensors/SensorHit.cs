using System;
using UnityEngine;

namespace asu4net.Sensors
{
    [Serializable]
    public struct SensorHit
    {
        public float distance;
        public Vector3 normal;
        public Vector3 point;
        public Transform transform;

        public static SensorHit From(RaycastHit2D hit)
        {
            var sensorHit = new SensorHit()
            {
                distance = hit.distance,
                normal = hit.normal,
                point = hit.point,
                transform = hit.transform
            };
            return sensorHit;
        }
        
        public static SensorHit From(RaycastHit hit)
        {
            var sensorHit = new SensorHit()
            {
                distance = hit.distance,
                normal = hit.normal,
                point = hit.point,
                transform = hit.transform
            };
            return sensorHit;
        }
    }
}