namespace asu4net.Sensors
{
    public struct SensorHitEventArgs
    {
        public bool multiple;
        public SensorHit[] hits;
        public SensorHit hit;
    }
}