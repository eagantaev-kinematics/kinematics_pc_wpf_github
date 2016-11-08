using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class Segment_cls
    {
        private int segment_id;


        public Segment_cls(int Segment_id)
        {
            segment_id = Segment_id;
        }

        public void calculate_segment_position(int id, byte[] raw_data_package)
        {
            //*
            // zabrat' dannye svoego datchika
            if(raw_data_package.Length == 342)
            {
                Double[] gyro = new Double[3];
                Double[] accel = new Double[3];
                Double[] magnet = new Double[3];
                Double[] x = new Double[3];
                Double[] y = new Double[3];
                Double[] z = new Double[3];

                // fill data arrays
                int i = id - 1;
                for (int j = 0; j < 3; j++)
                {
                    gyro[j] = (Double)((Int16)raw_data_package[i * 18 + j * 2] + ((Int16)(raw_data_package[i * 18 + j * 2 + 1]) << 8));
                }
                for (int j = 3; j < 6; j++)
                {
                    accel[j-3] = (Double)((Int16)raw_data_package[i * 18 + j * 2] + ((Int16)(raw_data_package[i * 18 + j * 2 + 1]) << 8));
                }
                for (int j = 6; j < 9; j++)
                {
                    magnet[j-6] = (Double)((Int16)raw_data_package[i * 18 + j * 2] + ((Int16)(raw_data_package[i * 18 + j * 2 + 1]) << 8));
                }

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

            }
            //*/


        }// end calculate_segment_position
    }
}
