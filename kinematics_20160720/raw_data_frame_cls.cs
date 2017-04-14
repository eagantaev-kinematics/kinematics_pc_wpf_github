using System;

namespace kinematics_20160720
{
    public class raw_data_frame_cls
    {
        private int FRAME_LENGTH = 19 * 9;      // 19 sensors * 9 axisses
        private Int16[] frame_array;
        private Boolean Frame_empty_flag = true;
        public Boolean frame_empty_flag
        {
            set { Frame_empty_flag = value; }
            get { return Frame_empty_flag; }
        }

        // public constructor
        public raw_data_frame_cls()
        {
            frame_array = new Int16[FRAME_LENGTH];

        }

        public void fill_frame(byte[] data_package)
        {
            if (data_package.Length == FRAME_LENGTH * 2)
            {
                for (int i = 0; i < FRAME_LENGTH; i++)
                {
                    Int16 aux = (Int16)(data_package[i * 2 + 1]); // high byte
                    aux <<= 8; // shift
                    aux += (Int16)(data_package[i * 2]); // low byte

                    frame_array[i] = aux;
                }
                Frame_empty_flag = false;
            }
            else
                Frame_empty_flag = true;
        }

        public Int16 get_frame_data(int index)
        {
            return frame_array[index];
        }
    }
}
