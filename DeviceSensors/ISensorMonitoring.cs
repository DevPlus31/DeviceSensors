using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSensors
{

    public interface ISensorValue { }

    /**
     * Sensor Value 
     **/
    public class SensorVectorValue : ISensorValue
    {
        public double X;
        public double Y;
        public double Z;
    }

    public class SingleSensorValue : ISensorValue
    {
        public double Value { get; set; }
    }

    public enum ValueType
    {
        Vector,
        SingleValue
    }


    public enum SensorType
    {
        Accelerometer, 
        Gyroscope,
        Magntometer, 
        Compass,
        Pedometer,
        Altimeter,
        Barometer,
        Inclinometer,
     //   LightSensor,
     //   ActivitySensor 

    }

    public class SensorReadingValueChangedEventArgs : EventArgs
    {
        public SensorType SensorType { get; set; }

        public ISensorValue Value { get; set; }

        public ValueType ValueType { get; set; }
       
    }

    public delegate void SensorReadingValueChangedEventHandler(object sender, SensorReadingValueChangedEventArgs e);

    public interface ISensorMonitoring : IDisposable
    {
        event SensorReadingValueChangedEventHandler SensorValueChanged;

        void Start(SensorType sensorType, int interval);

        void Stop(SensorType sensorType);

        bool IsSensorActive(SensorType sensorType);
    }
}
