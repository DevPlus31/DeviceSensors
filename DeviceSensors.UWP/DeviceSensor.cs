using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace DeviceSensors
{
    public class DeviceSensor : ISensorMonitoring
    {

        private Accelerometer accelerometer;
        private Altimeter altimeter;
        private Compass compass;
        private Gyrometer gyrometer;
        private Magnetometer magnetometer;
        private bool disposed;

        private IDictionary<SensorType, bool> sensorTypeStatus;

        public DeviceSensor()
        {
            disposed = false;
            sensorTypeStatus = new Dictionary<SensorType, bool>()
            {
                {
                    SensorType.Accelerometer,
                    false
                },
                {
                    SensorType.Gyroscope,
                    false
                },
                {
                    SensorType.Magntometer, 
                    false
                },
                {
                    SensorType.Altimeter,
                    false
                }
            };
        }

        public event SensorReadingValueChangedEventHandler SensorValueChanged;

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Dispose();
            }
                
        }

        public bool IsSensorActive(SensorType sensorType)
        {
            return sensorTypeStatus[sensorType];  
        }

        public void Start(SensorType sensorType, int interval)
        {
            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    accelerometer = Accelerometer.GetDefault();
                    if (accelerometer != null)
                    {
                        accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                    }
                    break;
                case SensorType.Gyroscope:
                    gyrometer = Gyrometer.GetDefault();
                    if (gyrometer != null)
                    {
                        gyrometer.ReadingChanged += Gyrometer_ReadingChanged;
                    }
                    break;
                case SensorType.Magntometer:
                    magnetometer = Magnetometer.GetDefault();
                    if (magnetometer != null)
                    {
                        magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
                    }
                    break;
                case SensorType.Altimeter:
                    altimeter = Altimeter.GetDefault();
                    if (altimeter != null)
                    {
                        altimeter.ReadingChanged += Altimeter_ReadingChanged;
                    }
                    break;
            }

            sensorTypeStatus[sensorType] = true;
                
        }

        private void Altimeter_ReadingChanged(Altimeter sender, AltimeterReadingChangedEventArgs args)
        {

            if (SensorValueChanged == null)
                return;

            var reading = args.Reading;

#if DEBUG
            Debug.WriteLine($"Altimeter Raised Event  Altitude = {reading.AltitudeChangeInMeters}");
#endif
            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Altimeter,
                ValueType = ValueType.SingleValue,
                Value = new SingleSensorValue() { Value = reading.AltitudeChangeInMeters,},

            });
        }

        private void Magnetometer_ReadingChanged(Magnetometer sender, MagnetometerReadingChangedEventArgs args)
        {
            if (SensorValueChanged == null)
                return;

            var reading = args.Reading;
#if DEBUG
            Debug.WriteLine($"Magnetometer Raised Event X = {reading.MagneticFieldX}, Y={reading.MagneticFieldY}, Z={reading.MagneticFieldZ}");
#endif

            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Magntometer,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = reading.MagneticFieldX, Y = reading.MagneticFieldY, Z = reading.MagneticFieldZ },

            });


        }

        private void Gyrometer_ReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            if (SensorValueChanged == null)
                return;

            var reading = args.Reading;

#if DEBUG
            Debug.WriteLine($"Magnetometer Raised Event X = {reading.AngularVelocityX}, Y={reading.AngularVelocityY}, Z={reading.AngularVelocityZ}");
#endif

            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Gyroscope,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = reading.AngularVelocityX, Y = reading.AngularVelocityY, Z = reading.AngularVelocityZ },
            });
        }

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            var reading = args.Reading;

#if DEBUG
            Debug.WriteLine($"Accelerometer Raised Event X = {reading.AccelerationX}, Y={reading.AccelerationY}, Z={reading.AccelerationZ}");
#endif

            if (SensorValueChanged == null)
                return;

            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Accelerometer,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = reading.AccelerationX, Y = reading.AccelerationY, Z = reading.AccelerationZ },

            });
        }

        public void Stop(SensorType sensorType)
        {
            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                    break;
                case SensorType.Gyroscope:
                    gyrometer.ReadingChanged -= Gyrometer_ReadingChanged;
                    break;
                case SensorType.Magntometer:
                    magnetometer.ReadingChanged -= Magnetometer_ReadingChanged;
                    break;
                case SensorType.Altimeter:
                    altimeter.ReadingChanged -= Altimeter_ReadingChanged;
                    break; 
            }
            sensorTypeStatus[sensorType] = false;
        }
    }
}
