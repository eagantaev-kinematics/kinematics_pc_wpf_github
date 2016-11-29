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

        }

        private double mean_calculation(int[] sorted_array, int min_max_cut_length)
        {
            double return_value = 0;


            return return_value;
        }

        private double sigma_calculation(int[] sorted_array, int min_max_cut_length, double mean)
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
            bool result = true;

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
            right_answer = new int[3] { 1, 2, 3 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[3] { 3, 2, 1 };
            right_answer = new int[3] { 1, 2, 3 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 1, 2, 3, 4, 5 };
            right_answer = new int[5] { 1, 2, 3, 4, 5 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 5, 4, 3, 2, 1 };
            right_answer = new int[5] { 1, 2, 3, 4, 5 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);
            //***************************
            source = new int[5] { 3, 2, 2, 2, 1 };
            right_answer = new int[5] { 1, 2, 3, 4, 5 };
            bubble_sort(source, ref sorted);
            result = result && tester_cls.array_equality(right_answer, sorted, sorted.Length);

            return result;
        }

        //******************* TESTS *************************************

    }//end class histogram_cls
}
