using System;

namespace kinematics_20160720
{
    public class histogram_cls
    {
        private int BUFFER_LENGTH;
        private int BINS_NUMBER;
        private int MIN_MAX_CUT_LENGTH;

        private int[] buffer;
        private int[] buffer_sorted;
        private double mean;
        private double sigma;
        private double low_edge, high_edge;
        private double bin_width;

        private int[] Bins;
        public int[] bins
        {
            get { return Bins; }
        }

        private int Main_bin; // middle of maximal bin
        public int main_bin
        {
            get { return Main_bin; }
        }
        private int Main_bin_value;
        public int main_bin_value
        {
            get { return Main_bin_value; }
        }

        private int recalculation_period;
        private int period_counter = 0;

        public histogram_cls(int buffer_length, int bins_number, int period)
        {
            MIN_MAX_CUT_LENGTH = 7; // drop 7 min and 7 max values

            BUFFER_LENGTH = buffer_length;
            buffer = new int[BUFFER_LENGTH];
            buffer_sorted = new int[BUFFER_LENGTH];
            BINS_NUMBER = bins_number;
            Bins = new int[BINS_NUMBER + 2];  // defined number of bins + all less then + all greater then

            recalculation_period = period;

        }

        public void add_value(int value)
        {
            // shift buffer array
            for(int i=0; i<(BUFFER_LENGTH-1); i++)
            {
                buffer[i] = buffer[i + 1];
            }
            //save value
            buffer[BUFFER_LENGTH - 1] = value;

            // increment period counter
            period_counter++;
            if(period_counter >= recalculation_period)
            {
                //reset counter
                recalculation_period = 0;

                // recalculate histogramm
                histogram_parameters_calculation();

            }

        }

        private void bubble_sort(int[] source_array, ref int[] sorted_array)
        {
            int len = source_array.Length;

            for (int i = 0; i < len; i++ )
            {
                sorted_array[i] = source_array[i];
            }

            for (int i = len; i > 1; i--)
            {
                for (int j = 1; j < i; j++)
                {
                    if(sorted_array[j] < sorted_array[j-1])
                    {
                        int aux = sorted_array[j];
                        sorted_array[j] = sorted_array[j - 1];
                        sorted_array[j - 1] = aux;
                    }
                }
            }
        }

        private double mean_calculation(int[] sorted_array, int min_max_cut_length)
        {
            double return_value = 0;
            int length = sorted_array.Length;

            if(length > (min_max_cut_length*2))
            {
                for (int i = min_max_cut_length; i < (length - min_max_cut_length); i++ )
                    return_value += sorted_array[i];
                return_value /= (double)(length - min_max_cut_length*2);
            }

            return return_value;
        }

        private double mean_calculation(double[] sorted_array, int min_max_cut_length)
        {
            double return_value = 0;
            int length = sorted_array.Length;

            if (length > (min_max_cut_length * 2))
            {
                for (int i = min_max_cut_length; i < (length - min_max_cut_length); i++)
                    return_value += sorted_array[i];
                return_value /= (double)(length - min_max_cut_length * 2);
            }

            return return_value;
        }

        private double sigma_calculation(int[] sorted_array, int min_max_cut_length)
        {
            double return_value = 0;
            int length = sorted_array.Length;
            double[] deviation = new double[length];
            double mean = mean_calculation(sorted_array, min_max_cut_length);

            for (int i = 0; i < length; i++)
                deviation[i] = (double)sorted_array[i] - mean;
            for (int i = 0; i < length; i++)
                deviation[i] = deviation[i] * deviation[i];

            return_value = mean_calculation(deviation, min_max_cut_length);
            return_value = Math.Sqrt(return_value);

                return return_value;
        }

        private void bins_calculation(int[] sorted_array, ref int[] bin_array, int bins_number, double low_edge, double bin_width)
        {
            for(int i=0; i<(bins_number+2); i++)
                bin_array[i] = 0;

            int index = 0;
            for(int i=0; i<(bins_number+1); i++)
            {
                while((index<sorted_array.Length) && (sorted_array[index] < (low_edge + i*bin_width)))
                {
                    bin_array[i]++;
                    index++;
                }
            }
            // fill greater then right edge bin
            bin_array[bins_number + 1] = sorted_array.Length - index;
        }

        public void histogram_parameters_calculation()
        {
            // sort buffer
            bubble_sort(buffer, ref buffer_sorted);
            // calculate mean
            mean = mean_calculation(buffer_sorted, MIN_MAX_CUT_LENGTH);
            sigma = sigma_calculation(buffer_sorted, MIN_MAX_CUT_LENGTH);

            low_edge = mean - 3 * sigma;
            high_edge = mean + 3 * sigma;

            bin_width = (high_edge - low_edge) / BINS_NUMBER;

            // calculate bins
            bins_calculation(buffer_sorted, ref Bins, BINS_NUMBER, low_edge, bin_width);
            Main_bin = calculate_main_bin();

        }

        int calculate_main_bin()
        {
            int max = 0;
            int max_index = 0;
            int return_value = 0;

            for(int i=0; i<Bins.Length; i++)
            {
                if(max < Bins[i])
                {
                    max = Bins[i];
                    max_index = i;
                }
            }

            Main_bin_value = max;
            return_value = (int)(low_edge + max_index * bin_width + bin_width / 2.0);

            return return_value;
        }


        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        public bool bubble_sort_test()
        {
            bool result = false;

            int[] source = new int[2]{ 1, 1 };
            int[] sorted = new int[2];
            int[] right_answer = new int[2]{ 1, 1};
            //***************************
            bubble_sort(source, ref sorted);
            result = tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source =       new int[2] { 1, 2 };
            right_answer = new int[2] { 1, 2 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source =       new int[2] { 2, 1 };
            right_answer = new int[2] { 1, 2 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[3] { 1, 2, 3 };
            sorted = new int[3];
            right_answer = new int[3] { 1, 2, 3 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[3] { 3, 2, 1 };
            sorted = new int[3];
            right_answer = new int[3] { 1, 2, 3 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 1, 2, 3, 4, 5 };
            sorted = new int[5];
            right_answer = new int[5] { 1, 2, 3, 4, 5 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 5, 4, 3, 2, 1 };
            sorted = new int[5];
            right_answer = new int[5] { 1, 2, 3, 4, 5 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 3, 2, 2, 2, 1 };
            sorted = new int[5];
            right_answer = new int[5] { 1, 2, 2, 2, 3 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);

            return result;
        }

        //******************* TESTS *************************************

        public bool mean_calculation_test()
        {
            bool result = false;

            double mean = 0;
            int[] sorted_array = {1,3,5,17,99};
            int max_min_cut;

            max_min_cut = 0;
            mean = mean_calculation(sorted_array, max_min_cut);
            result = tester_cls.equality(25, (int)mean);
            max_min_cut = 1;
            mean = mean_calculation(sorted_array, max_min_cut);
            result = result && tester_cls.equality(8, (int)mean);
            max_min_cut = 2;
            mean = mean_calculation(sorted_array, max_min_cut);
            result = result && tester_cls.equality(5, (int)mean);


            return result;
        }

        //******************* TESTS *************************************

        public bool sigma_calculation_test()
        {
            bool result = false;

            double sigma = 0;
            double epsilon = 0.001;
            int[] array = { 1, 2, 3, 4, 5 };
            int max_min_cut = 1;
            double right_answer = Math.Sqrt(2.0/3.0);

            sigma = sigma_calculation(array, max_min_cut);
            result = tester_cls.epsilon_equality(right_answer, sigma, epsilon);




            return result;
        }

        //******************* TESTS *************************************

        public bool bins_calculation_test()
        {
            bool result = false;
            
            int[] sorted_array = {1,2,3,4,5};
            int[] right_answer;
            int[] bin_array = new int[5];
            
            bins_calculation(sorted_array, ref bin_array, 3, 1.5, 1.0);
            right_answer = new int[5] { 1, 1, 1, 1, 1 };
            result = tester_cls.array_equality(right_answer, bin_array, 5);

            bins_calculation(sorted_array, ref bin_array, 3, 2.5, 3.0);
            right_answer = new int[5] { 2, 3, 0, 0, 0 };
            result = result && tester_cls.array_equality(right_answer, bin_array, 5);

            bins_calculation(sorted_array, ref bin_array, 3, -1.5, 1.0);
            right_answer = new int[5] { 0, 0, 0, 1, 4 };
            result = result && tester_cls.array_equality(right_answer, bin_array, 5);

            return result;
        }

    }//end class histogram_cls
}
