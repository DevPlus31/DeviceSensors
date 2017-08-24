using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DeviceSensors;

namespace Sample.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        DeviceSensors.DeviceSensor DeviceSensor;


        public event PropertyChangedEventHandler PropertyChanged;


        private double x;

        public Double X
        {
            get { return x; }
            set { x = value; OnPropertyChanged("X"); }
        }

        private double y;

        public Double Y
        {
            get { return y; }
            set { y = value; OnPropertyChanged("Y"); }
        }

        private double z;

        

        public Double Z
        {
            get { return z; }
            set { z = value; OnPropertyChanged("Z"); }
        }

        private double alti;

        public Double Alti
        {
            get { return alti; }
            set { alti = value; OnPropertyChanged("Alti"); }
        }


        private double magnx;

        public Double MagnX
        {
            get { return magnx; }
            set { magnx = value; OnPropertyChanged("MagnX"); }
        }

        private double magny;

        public Double MagnY
        {
            get { return magny; }
            set { magny = value; OnPropertyChanged("MagnY"); }
        }

        private double magnz;

        public Double MagnZ
        {
            get { return magnz; }
            set { magnz = value; OnPropertyChanged("MagnZ"); }
        }


        private double gyrox;

        public Double GyroX
        {
            get { return gyrox; }
            set { gyrox = value; OnPropertyChanged("MagnX"); }
        }

        private double gyroy;

        public Double GyroY
        {
            get { return gyroy; }
            set { gyroy = value; OnPropertyChanged("GyroY"); }
        }

        private double gyroz;



        public Double GyroZ
        {
            get { return gyroz; }
            set { gyroz = value; OnPropertyChanged("GyroZ"); }
        }


        public MainViewModel()
        {
            DeviceSensor = new DeviceSensors.DeviceSensor();
            DeviceSensor.Start(SensorType.Accelerometer, 1000);
            DeviceSensor.Start(SensorType.Altimeter, 1000);
            DeviceSensor.Start(SensorType.Magntometer, 0);
            DeviceSensor.Start(SensorType.Gyroscope, 0);

            DeviceSensor.SensorValueChanged += DeviceSensor_SensorValueChanged;
        }

        private void DeviceSensor_SensorValueChanged(object sender, SensorReadingValueChangedEventArgs e)
        {
            if (e.ValueType == DeviceSensors.ValueType.Vector)
            {
                var value = (DeviceSensors.SensorVectorValue)e.Value;

                if (e.SensorType == SensorType.Accelerometer)
                {
                    X = value.X;
                    Z = value.Z;
                    Y = value.Y;
                }
                else if (e.SensorType == SensorType.Magntometer)
                {
                    MagnX = value.X;
                    MagnY = value.Y;
                    MagnZ = value.Z;

                }
                else if (e.SensorType == SensorType.Gyroscope)
                {
                    GyroX = value.X;
                    GyroY = value.Y;
                    GyroZ = value.Z;

                }

            }
            else
            {
                var value = (DeviceSensors.SingleSensorValue)e.Value;
                Alti = value.Value;
            }

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName= null)
        {
           if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
