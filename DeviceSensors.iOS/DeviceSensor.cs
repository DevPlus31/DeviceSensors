using CoreLocation;
using CoreMotion;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace DeviceSensors
{
    public class DeviceSensor : ISensorMonitoring
    {
        private CMMotionManager motionManager;
        private CLLocationManager locationManager;
        private CMAltimeter altimeter;
        private CMMotionActivityManager motionActivityManager;
        private IDictionary<SensorType, bool> sensorTypeStatus;
        private bool disposed;


        public DeviceSensor()
        {
            motionManager = new CMMotionManager();
            locationManager = new CLLocationManager();
            motionActivityManager = new CMMotionActivityManager();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                altimeter = new CMAltimeter();
            }

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
                },
               /* {
                    SensorType.ActivitySensor,
                    false
                }*/
            };
        }

        public event SensorReadingValueChangedEventHandler SensorValueChanged;

        public void Dispose()
        {
            if (!disposed)
            {
                motionManager.Dispose();
                this.Dispose();
                disposed = true; 
            }
        }

        public bool IsSensorActive(SensorType sensorType)
        {
            if (sensorTypeStatus.ContainsKey(sensorType))
                return sensorTypeStatus[sensorType];
            return false;
        }

        public void Start(SensorType sensorType, int interval)
        {
            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    if (motionManager.AccelerometerAvailable)
                    {
                        this.motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue,
                                   new CMAccelerometerHandler(this.OnAccelerometerReadingValueChanged));
                    }
                    break;
                case SensorType.Gyroscope:
                    if (this.motionManager.GyroAvailable)
                    {
                        this.motionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue,
                                    new CMGyroHandler(this.OnGyroReadingValueChanged));
                    }
                    break;
                case SensorType.Magntometer:
                    if (this.motionManager.MagnetometerAvailable)
                    {
                        this.motionManager.StartMagnetometerUpdates(NSOperationQueue.CurrentQueue,
                                    new CMMagnetometerHandler(this.OnMagnteometerReadingValueChanged));
                    }
                    break;
                case SensorType.Compass: 
                    if (CLLocationManager.HeadingAvailable)
                    {
                        this.locationManager.StartUpdatingHeading();
                        this.locationManager.UpdatedHeading += LocationManager_UpdatedHeading;
                    }
                    break;
                case SensorType.Barometer:
                    if (altimeter != null && CMAltimeter.IsRelativeAltitudeAvailable)
                    {
                        altimeter.StartRelativeAltitudeUpdates(NSOperationQueue.CurrentQueue, new Action<CMAltitudeData, NSError>(this.OnAltimeterReadingValueChanged));
                    }
                    break;
            /*    case SensorType.ActivitySensor:
                    if (CMMotionActivityManager.IsActivityAvailable)
                    {
                        this.motionActivityManager.StartActivityUpdates(NSOperationQueue.CurrentQueue, new CMMotionActivityHandler(this.OnActivityUpdated));
                    }
                    break;*/
                    
                    
            }

            sensorTypeStatus[sensorType] = true;
        }

      /*  private void OnActivityUpdated(CMMotionActivity activity)
        {
            if (this.SensorValueChanged == null)
                return;

            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.ActivitySensor,
                ValueType = ValueType.Vector,
            //     Value = new SensorVectorValue() { X = activity. }
            });

        }*/

        private void OnAltimeterReadingValueChanged(CMAltitudeData arg1, NSError arg2)
        {
            if (this.SensorValueChanged == null)
                return;

            SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Altimeter,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = arg1.Timestamp, Y = arg1.RelativeAltitude.DoubleValue, Z = arg1.Pressure.DoubleValue}
            });
        }

        private void LocationManager_UpdatedHeading(object sender, CLHeadingUpdatedEventArgs e)
        {
            if (this.SensorValueChanged == null)
                return;

            SensorReadingValueChangedEventHandler sensorValueChanged = this.SensorValueChanged;
            SensorReadingValueChangedEventArgs changedArgs = new SensorReadingValueChangedEventArgs();
            changedArgs.SensorType = SensorType.Compass;
            
            
           // changedArgs.Value = 
        }

        private void OnMagnteometerReadingValueChanged(CMMagnetometerData magnetometerData, NSError error)
        {
            if (this.SensorValueChanged == null)
                return;

            this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Magntometer,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = magnetometerData.MagneticField.X, Y = magnetometerData.MagneticField.Y, Z = magnetometerData.MagneticField.Z},
            });
        }

        private void OnGyroReadingValueChanged(CMGyroData gyroData, NSError error)
        {
            if (this.SensorValueChanged == null)
                return;


            this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Gyroscope,
                Value = new SensorVectorValue() { X = gyroData.RotationRate.x, Y = gyroData.RotationRate.y, Z = gyroData.RotationRate.z }, 
                
            });
        }

        private void OnAccelerometerReadingValueChanged(CMAccelerometerData data, NSError error)
        {
            if (this.SensorValueChanged == null)
                return;

            if (error != null)
            {

               

                return;
            }

            this.SensorValueChanged((object)this, new SensorReadingValueChangedEventArgs()
            {
                SensorType = SensorType.Accelerometer,
                ValueType = ValueType.Vector,
                Value = new SensorVectorValue() { X = data.Acceleration.X, Y = data.Acceleration.Y, Z = data.Acceleration.Z },
                
                
            });
        }

        public void Stop(SensorType sensorType)
        {

            switch(sensorType)
            {
                case SensorType.Accelerometer:
                    if (this.motionManager.AccelerometerAvailable && this.motionManager.AccelerometerActive)
                        this.motionManager.StopAccelerometerUpdates();
                    break;
                case SensorType.Gyroscope:
                    if (this.motionManager.GyroAvailable && this.motionManager.GyroActive)
                        this.motionManager.StopGyroUpdates();
                    break;
                case SensorType.Magntometer:
                    if (this.motionManager.MagnetometerAvailable && this.motionManager.MagnetometerActive)
                        this.motionManager.StopMagnetometerUpdates();
                    break;
                case SensorType.Compass:
                    this.locationManager.StopUpdatingHeading();
                    break;
            }
            sensorTypeStatus[sensorType] = false;
        }
    }
}
