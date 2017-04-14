namespace kinematics_20160720
{
    class Smoother_cls
    {
        static double[] filter = { 1.0 / 23.0, 2.0 / 23.0, 5.0 / 23.0, 7.0 / 23.0, 5.0 / 23.0, 2.0 / 23.0, 1.0 / 23.0 };
        static public void smooth_double_array(double[] source_array, ref double[] target_array)
        {
            int length = source_array.Length;
            if(length == target_array.Length)   // else nothing else matter
            {
                int filter_length = filter.Length;
                for(int i=0; i<length; i++)
                {
                    target_array[i] = 0;
                    for (int j = 0; j < filter_length; j++)
                    {
                        target_array[i] += filter[j] * source_array[get_index(j, i, filter.Length, length)];
                    }
                }
            }
        }

        static private int get_index(int j, int i, int filter_length, int array_length)
        {
            int index = i + (j - filter_length / 2);
            if (index >= 0 && index < array_length)
                return index;
            else if (index < 0)
                return 0;
            else
                return array_length - 1;

        }
    }
}
