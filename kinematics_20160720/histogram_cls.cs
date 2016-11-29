using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class histogram_cls
    {
        private int BUFFER_LENGTH;
        private int BINS_NUMBER;
        private double BIN_WIDTH;

        private int[] buffer;
        private int[] bins;

        private int recalculation_period;
        private int period_counter = 0;

        public histogram_cls(int buffer_length, int bins_number, double bin_width, int period)
        {
            BUFFER_LENGTH = buffer_length;
            buffer = new int[BUFFER_LENGTH];
            BINS_NUMBER = bins_number;
            bins = new int[BINS_NUMBER];

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


            return return_value;
        }

        private double sigma_calculation(int[] sorted_array, int min_max_cut_length)
        {
            double return_value = 0;


            return return_value;
        }

        public void histogram_parameters_calculation()
        {

        }


        //******************* TESTS *************************************
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



    }//end class histogram_cls
}
