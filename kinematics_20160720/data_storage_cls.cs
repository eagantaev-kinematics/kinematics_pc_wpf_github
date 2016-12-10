using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class data_storage_cls
    {
        private const int DATA_STORAGE_LENGTH = 20000;
        private int current_in_index = 0;
        private double[] Storage = new double[DATA_STORAGE_LENGTH];
        public data_storage_cls()
        {

        }
        public void init()
        {

        }

        public void data_push(double angle)
        {
            Storage[current_in_index] = angle;
            current_in_index++;
        }
        public void cycle_delimiter_push()
        {
            Storage[current_in_index] = Double.NaN;
            current_in_index++;
        }

        public void storage_reset()
        {
            current_in_index = 0;
        }
    }
}
