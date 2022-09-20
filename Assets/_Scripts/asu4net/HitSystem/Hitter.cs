using asu4net.Sensors;
using MyBox;
using UnityEngine;

namespace asu4net.HitSystem
{
    [RequireComponent(typeof(SensorCollection))]
    public class Hitter : MonoBehaviour
    {
        #region Properties & Fields

        [SerializeField] private bool customHitData;
        [SerializeField] [ConditionalField(nameof(customHitData))] [DisplayInspector] 
        private HitData hitDataField;
        public HitData hitData
        {
            get => hitDataField;
            set => hitDataField = value;
        }

        #endregion

        #region Member

        private float _contactTimer;
        private bool _poisonEnabled;
        private SensorCollection _sensor;

        #endregion

        #region Unity life cycle

        private void OnValidate()
        {
            _sensor = GetComponent<SensorCollection>();
        }
        protected virtual void Awake()
        {
            if (!customHitData)
                hitData = ScriptableObject.CreateInstance<HitData>();
            _sensor.onEnter.AddListener(OnSensorEvent);
        }

        #endregion

        #region Event listeners

        protected void OnSensorEvent(SensorHitEventArgs args)
        {
            if (args.multiple)
            {
                foreach(var sensorHit in args.hits)
                    MakeHit(sensorHit);
                return;
            }
            MakeHit(args.hit);
        }

        #endregion

        #region Methods

        protected void MakeHit(SensorHit sensorHit)
        {
            var isHittable = sensorHit.transform.TryGetComponent(out IHittable hittable);
            if (!isHittable) return;
            hittable.TakeHit(hitData.Create(sensorHit.point, this));
        }

        #endregion
    }
}