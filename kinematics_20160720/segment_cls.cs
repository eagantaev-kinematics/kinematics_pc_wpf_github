using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class Segment_cls
    {
        private raw_kinematics_data_cls raw_data;
        private int segment_id;
        private Double[] segment_axis;
        private Double X, Y, Z;
        private Double[] x;
        private Double[] y;
        private Double[] z;
        private Double[] xl;
        private Double[] yl;
        private Double[] zl;

        private sensor_cls Accelerometer, Gyroscope, Magnetometer;
        public sensor_cls accelerometer
        {
            set { Accelerometer = value; }
            get { return Accelerometer; }
        }
        public sensor_cls gyroscope
        {
            set { Gyroscope = value; }
            get { return Gyroscope; }
        }
        public sensor_cls magnetometer
        {
            set { magnetometer = value; }
            get { return magnetometer; }
        }

        private sensor_cls[] Sensors_array = new sensor_cls[3];
        public sensor_cls[] sensors_array
        {
            set { Sensors_array = value; }
            get { return Sensors_array; }
        }

        public Segment_cls(int Segment_id, raw_kinematics_data_cls Raw_data)
        {
            raw_data = Raw_data;
            segment_id = Segment_id;
            segment_axis = new Double[3];
            x = new Double[3];
            y = new Double[3];
            z = new Double[3];
            xl = new Double[3];
            yl = new Double[3];
            zl = new Double[3];

            Accelerometer = new sensor_cls();
            Gyroscope = new sensor_cls();
            Magnetometer = new sensor_cls();

            Sensors_array[0] = Accelerometer;
            Sensors_array[1] = Gyroscope;
            Sensors_array[2] = Magnetometer;
        }

        public void calculate_segment_position()
        {
            //*
            // zabrat' dannye svoego datchika
            if(raw_data.Kinematics_Data.Length == raw_data.Raw_Data_Length)
            {
                Double[] gyro = new Double[3];
                Double[] accel = new Double[3];
                Double[] magnet = new Double[3];
                

                // fill data arrays
                //int i = 1;
                int i = segment_id - 1;
                for (int j = 0; j < 3; j++)
                {
                    Int16 aux = (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2 + 1]); // high byte
                    aux <<= 8; // shift
                    aux += (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2]); // low byte
                    gyro[j] = (Double)aux;
                }
                for (int j = 3; j < 6; j++)
                {
                    Int16 aux = (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2 + 1]); // high byte
                    aux <<= 8; // shift
                    aux += (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2]); // low byte
                    accel[j - 3] = (Double)aux;
                }
                for (int j = 6; j < 9; j++)
                {
                    Int16 aux = (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2 + 1]); // high byte
                    aux <<= 8; // shift
                    aux += (Int16)(raw_data.Kinematics_Data[i * 18 + j * 2]); // low byte
                    magnet[j - 6] = (Double)aux;
                }

                Accelerometer.x = (int)accel[0];
                Accelerometer.y = (int)accel[1];
                Accelerometer.z = (int)accel[2];
                Gyroscope.x = (int)gyro[0];
                Gyroscope.y = (int)accel[1];
                Gyroscope.z = (int)accel[2];
                Magnetometer.x = (int)magnet[0];
                Magnetometer.y = (int)magnet[1];
                Magnetometer.z = (int)magnet[2];

                // sformirovat' matricu povorota global'naya-v-lokal'noi ****************************************
                // 1) normiruem vektory uskoreniya i magnitnogo polya
                Double norma = accel[0] * accel[0] + accel[1] * accel[1] + accel[2] * accel[2];
                norma = Math.Sqrt(norma);
                if (norma != 0.0)
                {
                    accel[0] = accel[0] / norma;
                    accel[1] = accel[1] / norma;
                    accel[2] = accel[2] / norma;
                }

                norma = magnet[0] * magnet[0] + magnet[1] * magnet[1] + magnet[2] * magnet[2];
                norma = Math.Sqrt(norma);
                if (norma != 0.0)
                {
                    magnet[0] = magnet[0] / norma;
                    magnet[1] = magnet[1] / norma;
                    magnet[2] = magnet[2] / norma;
                }
                // 2) vychislyaem tretii ort (vektornoe proizvedenie uskoreniya i magnitnogo polya)
                // (AyBz - AzBy, AzBx - AxBz, AxBy - AyBx)
                y[0] = magnet[0];
                y[1] = magnet[1];
                y[2] = magnet[2];
                z[0] = accel[0];
                z[1] = accel[1];
                z[2] = accel[2];
                x[0] = y[1] * z[2] - y[2] * z[1];
                x[1] = y[2] * z[0] - y[0] * z[2];
                x[2] = y[0] * z[1] - y[1] * z[0];

                norma = x[0] * x[0] + x[1] * x[1] + x[2] * x[2];
                norma = Math.Sqrt(norma);
                if (norma != 0.0)
                {
                    x[0] = x[0] / norma;
                    x[1] = x[1] / norma;
                    x[2] = x[2] / norma;
                }

                // 3) Popravlyaem vektor magnitnogo polya (kak vektornoe proizvedenie dvuh ortov)
                y[0] = z[1] * x[2] - z[2] * x[1];
                y[1] = z[2] * x[0] - z[0] * x[2];
                y[2] = z[0] * x[1] - z[1] * x[0];
                norma = y[0] * y[0] + y[1] * y[1] + y[2] * y[2];
                norma = Math.Sqrt(norma);
                if (norma != 0.0)
                {
                    y[0] = y[0] / norma;
                    y[1] = y[1] / norma;
                    y[2] = y[2] / norma;
                }


                Double nx = x[0] * x[0] + x[1] * x[1] + x[2] * x[2];
                Double ny = y[0] * y[0] + y[1] * y[1] + y[2] * y[2];
                Double nz = z[0] * z[0] + z[1] * z[1] + z[2] * z[2];

                Double dp_xy = x[0] * y[0] + x[1] * y[1] + x[2] * y[2];
                Double dp_xz = x[0] * z[0] + x[1] * z[1] + x[2] * z[2];
                Double dp_yz = y[0] * z[0] + y[1] * z[1] + y[2] * z[2];

                // formiruem matricu povorota global'naya v lokal'noi
                Double[,] m = new Double[3, 3];
                m[0, 0] = x[0];
                m[1, 0] = x[1];
                m[2, 0] = x[2];
                m[0, 1] = y[0];
                m[1, 1] = y[1];
                m[2, 1] = y[2];
                m[0, 2] = z[0];
                m[1, 2] = z[1];
                m[2, 2] = z[2];
                Double det_m = m[0, 0] * m[1, 1] * m[2, 2] - m[0, 0] * m[1, 2] * m[2, 1] - m[0, 1] * m[1, 0] * m[2, 2] + m[0, 1] * m[1, 2] * m[2, 0] + m[0, 2] * m[1, 0] * m[2, 1] - m[0, 2] * m[1, 1] * m[2, 0];
                // formiruem matricu povorota lokal'naya v global'noi
                Double[,] M = new Double[3, 3];
                M[0, 0] = x[0];
                M[0, 1] = x[1];
                M[0, 2] = x[2];
                M[1, 0] = y[0];
                M[1, 1] = y[1];
                M[1, 2] = y[2];
                M[2, 0] = z[0];
                M[2, 1] = z[1];
                M[2, 2] = z[2];
                Double det_M = M[0, 0] * M[1, 1] * M[2, 2] - M[0, 0] * M[1, 2] * M[2, 1] - M[0, 1] * M[1, 0] * M[2, 2] + M[0, 1] * M[1, 2] * M[2, 0] + M[0, 2] * M[1, 0] * M[2, 1] - M[0, 2] * M[1, 1] * M[2, 0];

                // test (proizvedenie matric dolzhno byt' edinichnoi)
                Double[,] p = new Double[3, 3];
                p[0, 0] = m[0, 0] * M[0, 0] + m[0, 1] * M[1, 0] + m[0, 2] * M[2, 0];
                p[0, 1] = m[0, 0] * M[0, 1] + m[0, 1] * M[1, 1] + m[0, 2] * M[2, 1];
                p[0, 2] = m[0, 0] * M[0, 2] + m[0, 1] * M[1, 2] + m[0, 2] * M[2, 2];
                p[1, 0] = m[1, 0] * M[0, 0] + m[1, 1] * M[1, 0] + m[1, 2] * M[2, 0];
                p[1, 1] = m[1, 0] * M[0, 1] + m[1, 1] * M[1, 1] + m[1, 2] * M[2, 1];
                p[1, 2] = m[1, 0] * M[0, 2] + m[1, 1] * M[1, 2] + m[1, 2] * M[2, 2];
                p[2, 0] = m[2, 0] * M[0, 0] + m[2, 1] * M[1, 0] + m[2, 2] * M[2, 0];
                p[2, 1] = m[2, 0] * M[0, 1] + m[2, 1] * M[1, 1] + m[2, 2] * M[2, 1];
                p[2, 2] = m[2, 0] * M[0, 2] + m[2, 1] * M[1, 2] + m[2, 2] * M[2, 2];

                xl[0] = M[0, 0];
                xl[1] = M[1, 0];
                xl[2] = M[2, 0];
                yl[0] = M[0, 1];
                yl[1] = M[1, 1];
                yl[2] = M[2, 1];
                zl[0] = M[0, 2];
                zl[1] = M[1, 2];
                zl[2] = M[2, 2];

                segment_axis[0] = M[0, 1];
                segment_axis[1] = M[1, 1];
                segment_axis[2] = M[2, 1];

                X = segment_axis[0];
                Y = segment_axis[1];
                Z = segment_axis[2];

            }
            //*/


        }// end calculate_segment_position


        public Double get_X()
        { return X; }

        public Double get_Y()
        { return Y; }

        public Double get_Z()
        { return Z; }

        public Double[] get_x()
        { return x; }
        public Double[] get_y()
        { return y; }
        public Double[] get_z()
        { return z; }
        public Double[] get_xl()
        { return xl; }
        public Double[] get_yl()
        { return yl; }
        public Double[] get_zl()
        { return zl; }

    }
}
