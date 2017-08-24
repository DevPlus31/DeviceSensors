using Android.App;
using Android.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using System.Diagnostics;
using Android.Locations;
using Android.OS;

namespace DeviceSensors
{
    public class DeviceSensor : Java.Lang.Object, ISensorMonitoring, ISensorEventListener, ILocationListener
    {
        private SensorManager sensorManager;
        private LocationManager locationManager;
        private Sensor accelerometerSensor;
        private Sensor gyroSensor;
        private Sensor compassSensor;
        private Sensor magnetometerSensor;
        private Sensor altimeterSensor;
        private Sensor lightSensor;

        private IDictionary<SensorType, bool> sensorTypeStatus; 

        private bool disposed;

        public DeviceSensor()
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Debug: Android DeviceSensor class started");
#endif

            sensorManager = (SensorManager)Application.Context.GetSystemService(Application.SensorService);
            locationManager = Application.Context.GetSystemService(Application.LocationService) as LocationManager;
            this.accelerometerSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Accelerometer);
            this.gyroSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Gyroscope);
            this.compassSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Orientation);
            this.magnetometerSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.MagneticField);
            // this.altimeterSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.L);
            this.lightSensor = this.sensorManager.GetDefaultSensor(Android.Hardware.SensorType.Light);
           
                      

       
            sensorTypeStatus = new Dictionary<SensorType, bool>()
            {
                {
                    SensorType.Accelerometer, 
                    false
                },
                {
                    SensorType.Compass,
                    false
                },
                {
                    SensorType.Gyroscope,
                    false
                },
                {
                    SensorType.Magntometer,
                    false
                }
            };
        }

        public event SensorReadingValueChangedEventHandler SensorValueChanged;

       

        public bool IsSensorActive(SensorType sensorType)
        {
            if (sensorTypeStatus.ContainsKey(sensorType))
                return sensorTypeStatus[sensorType];
            return false;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
           if (this.SensorValueChanged == null)
                return;

            var sensorType = ConvertDroidSensorTypeToGenericSensorType(e.Sensor.Type);
            
            switch(sensorType)
            {
                case DeviceSensors.SensorType.Accelerometer:
                    this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
                    {
                        Value = new SensorVectorValue() { X = e.Values[0], Y = e.Values[1], Z = e.Values[2] },
                        SensorType = SensorType.Accelerometer,
                        ValueType = ValueType.Vector


                    });
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Accelerometer: X = {e.Values[0]}, Y = {e.Values[1]}, Z = {e.Values[2]}");
#endif 
                    break;
                case SensorType.Gyroscope:
                    this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
                    {
                        Value = new SensorVectorValue() { X = e.Values[0], Y = e.Values[1], Z = e.Values[2] },
                        SensorType = SensorType.Gyroscope,
                        ValueType = ValueType.Vector
                       
                    });
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Gyroscope: X = {e.Values[0]}, Y = {e.Values[1]}, Z = {e.Values[2]}");
#endif 
                    break;
                case SensorType.Magntometer:
                    this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
                    {
                        Value = new SensorVectorValue() { X = e.Values[0], Y = e.Values[1], Z = e.Values[2] },
                        SensorType = SensorType.Magntometer,
                        ValueType = ValueType.Vector
                    });
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Magntometer: X = {e.Values[0]}, Y = {e.Values[1]}, Z = {e.Values[2]}");
#endif 
                    break;

                case SensorType.Barometer:
                    this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
                    {
                        SensorType = SensorType.Barometer,
                        // Value = new SensorVectorValue() 
                    });
                    break;
            }
        }

        public void Start(SensorType sensorType, int interval)
        {
            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    if (this.accelerometerSensor != null)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"Started Sensor: (Accelerometer)");
#endif
                        this.sensorManager.RegisterListener(this, this.accelerometerSensor, SensorDelay.Normal);
                    }
                    break;
                case SensorType.Gyroscope:
                    if (this.gyroSensor != null)
                    {
                        this.sensorManager.RegisterListener(this, this.gyroSensor, SensorDelay.Normal);
                    }
                    break;
                case SensorType.Magntometer:
                    if (this.magnetometerSensor != null)
                    {
                        this.sensorManager.RegisterListener(this, this.magnetometerSensor, SensorDelay.Normal);
                    }
                    break;
                case SensorType.Compass:
                    if (this.compassSensor != null)
                    {
                        this.sensorManager.RegisterListener(this, this.compassSensor, SensorDelay.Normal);
                    }
                    break;
                case SensorType.Altimeter:

                    if (this.locationManager != null)
                    {

                        this.locationManager.RequestLocationUpdates("gps", 3000, 2, this, Android.OS.Looper.MainLooper );
                    }

                    break;
            }

            sensorTypeStatus[sensorType] = true;
        }

        public void Stop(SensorType sensorType)
        {
            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    if (this.accelerometerSensor != null)
                    {
                        this.sensorManager.UnregisterListener(this, accelerometerSensor);
                    }
                    break;
                case SensorType.Gyroscope:
                    if (this.gyroSensor != null)
                    {
                        this.sensorManager.UnregisterListener(this, gyroSensor);
                    }
                    break;
                case SensorType.Magntometer: 
                    if (this.magnetometerSensor != null)
                    {
                        this.sensorManager.UnregisterListener(this, magnetometerSensor);
                    }
                    break;
                case SensorType.Compass:
                    if (this.compassSensor != null)
                    {
                        this.sensorManager.UnregisterListener(this, compassSensor);
                    }
                    break;
            }
            sensorTypeStatus[sensorType] = false;
        }

        private SensorType ConvertDroidSensorTypeToGenericSensorType(Android.Hardware.SensorType type)
        {
            switch(type)
            {
                case Android.Hardware.SensorType.Accelerometer:
                    return SensorType.Accelerometer;
                case Android.Hardware.SensorType.Gyroscope:
                    return SensorType.Gyroscope;
                case Android.Hardware.SensorType.Orientation:
                    return SensorType.Compass;
                case Android.Hardware.SensorType.Pressure:
                    return SensorType.Altimeter;
                    

            }

            return default(SensorType);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                this.Dispose();
            }

        }

        public void OnLocationChanged(Location location)
        {
            this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                Value = new SingleSensorValue() { Value = location.Altitude },
                SensorType = SensorType.Altimeter,
                ValueType = ValueType.SingleValue

            });
        }

        public void OnProviderDisabled(string provider)
        {
            // throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }
}
