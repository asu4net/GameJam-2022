using asu4net.Sensors;
using asu4net.StateSystem;
using UnityEngine;

namespace asu4net.AI
{
    public class PatrolState : State
    {
        #region Properties & Fields

        public Transform lastTarget { get; private set; }
        
        [Header("Settings:")]
        [SerializeField] private string stateOnSuccess = "ChaseState";
        
        [Space]
        [SerializeField] private Sensor sensor;

        #endregion

        #region Unity inherited

        private void Awake()
        {
            sensor.gameObject.SetActive(false);
        }

        #endregion

        #region Methods

        public override void EnterState(IState previousState)
        {
            sensor.gameObject.SetActive(true);
            sensor.onStay.AddListener(OnSensorStay);
        }
        private void OnSensorStay(SensorHitEventArgs args)
        {
            lastTarget = args.hits[0].transform;
            stateManager.SwitchState(stateOnSuccess);
        }
        public override void ExitState()
        {
            sensor.gameObject.SetActive(false);
            sensor.onStay.RemoveListener(OnSensorStay);
        }

        #endregion
    }
}