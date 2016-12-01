using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class sensor_cls
    {
        private accelerometer_cls accel;
        private gyroscope_cls gyro;
        private magnetometer_cls magnet;

        public accelerometer_cls accelerometer
        {
            set { accel = value; }
            get { return accel; }
        }

        public gyroscope_cls gyroscope
        {
            set { gyro = value; }
            get { return gyro; }
        }

        public magnetometer_cls magnetometer
        {
            set { magnet = value; }
            get { return magnet; }
        }

        public sensor_cls()
        {
            accel = new accelerometer_cls();
            gyro = new gyroscope_cls();
            magnet = new magnetometer_cls();
        }
    }
}
