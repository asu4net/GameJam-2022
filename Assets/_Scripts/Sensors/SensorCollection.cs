using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace asu4net.Sensors
{
    [ExecuteInEditMode]
    public class SensorCollection : MonoBehaviour
    {
        public UnityEvent<SensorHitEventArgs> onEnter;
        public UnityEvent<SensorHitEventArgs> onStay;
        public UnityEvent<SensorHitEventArgs> onExit;

        public bool isDetecting => sensors.Any(sensor => sensor is not null && sensor.isDetecting);
        
        [SerializeField] [ReadOnly] private bool detecting;
        [SerializeField] private List<Sensor> sensors = new();

        private bool _onStayCalledThisFrame;
        private bool _wasDetecting;

        private void Awake()
        {
            sensors.ForEach(sensor =>
            {
                sensor.onEnter.AddListener((args) =>
                {
                    var otherSensors = sensors.ToList();
                    otherSensors.Remove(sensor);
                    if (otherSensors.Any(otherSensor => otherSensor.isDetectingEv)) return;
                    onEnter?.Invoke(args);
                });
                
                sensor.onStay.AddListener((args) =>
                {
                    if (_onStayCalledThisFrame || !sensors.Any(otherSensor => otherSensor.isDetectingEv)) return;
                    onStay?.Invoke(args);
                    _onStayCalledThisFrame = true;
                });
                
                sensor.onExit.AddListener((args) =>
                {
                    var otherSensors = sensors.ToList();
                    otherSensors.Remove(sensor);
                    if (otherSensors.Any(otherSensor => otherSensor.isDetectingEv)) return;
                    onExit?.Invoke(args);
                });
            });
        }

        private void Update()
        {
            _onStayCalledThisFrame = false;
            #if UNITY_EDITOR
            if (sensors.Count == 0) return;
            detecting = isDetecting;
            #endif
        }
    }
}