using System;

namespace kinematics_20160720
{
    public class data_storage_cls
    {
        private const int DATA_STORAGE_LENGTH = 20000;
        private int Current_in_index = 0;
        public int current_in_index
        {
            get { return Current_in_index; }
        }
        private double[] Storage = new double[DATA_STORAGE_LENGTH];
        private double[] Smoothed_Storage = new double[DATA_STORAGE_LENGTH];
        
        public data_storage_cls()
        {

        }
        public void init()
        {

        }

        public void data_push(double angle)
        {
            Storage[current_in_index] = angle;
            Current_in_index++;
        }
        public void cycle_delimiter_push()
        {
            Storage[current_in_index] = Double.NaN;
            Current_in_index++;
        }

        public void storage_reset()
        {
            Current_in_index = 0;
        }
        public double get_data(int index)
        {  
            return Storage[index];
        }
        public double get_smoothed_data(int index)
        {
            return Smoothed_Storage[index];
        }

        public void smooth_data()
        {
            int i = 0;
            int start = 0;
            while (i < current_in_index)
            {
                if (!(Double.IsNaN(Storage[i])))
                {
                    i++;
                }
                else // here is a NAN delimiter
                {
                    double[] source = new double[i - start];
                    double[] smoothed = new double[source.Length];
                    // copy data
                    for (int j = 0; j < source.Length; j++)
                         source[j] = Storage[j + start];

                    Smoother_cls.smooth_double_array(source, ref smoothed);

                    //copy data
                    for (int j = 0; j < smoothed.Length; j++)
                        Smoothed_Storage[j+start] = smoothed[j];
                    Smoothed_Storage[i] = Double.NaN;
                    i++;
                    start = i;
                }
            }
        }
    }
}
