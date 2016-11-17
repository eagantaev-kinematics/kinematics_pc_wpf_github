using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class raw_kinematics_data_cls
    {
        private const int RAW_DATA_LENGTH = 342;
        public int Raw_Data_Length
        {
            get { return RAW_DATA_LENGTH; }
        }

        private byte[] kinematics_data;

        public raw_kinematics_data_cls()
        {
            kinematics_data = new byte[RAW_DATA_LENGTH];
        }

        public byte[] Kinematics_Data
        {
            get { return kinematics_data; }

            set
            {
                kinematics_data = value;
            }
        }



    }
}
