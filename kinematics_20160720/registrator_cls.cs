using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Series;

namespace kinematics_20160720
{
    public class registrator_cls
    {
        public data_storage_cls Storage;
        private bool Registering = false;
        metronome_cls metronome;

        private double[] mean_cycle_buffer;             // srednee arifmeticheskoe
        private double[] mean_filtered_cycle_buffer;    // predvaritel'naya filtraciya, potom srednee arifmeticheskoe
        public double[] smoothed_cycle_buffer;    // predvaritel'naya filtraciya, potom srednee arifmeticheskoe, potom sglazhivanie
        public double get_mean_cycle_data(int index)
        {
            if(index < mean_cycle_buffer.Length)
                return mean_cycle_buffer[index];
            else
                return Double.NaN;
        }
        public double get_filtered_mean_cycle_data(int index)
        {
            if (index < mean_filtered_cycle_buffer.Length)
                return mean_filtered_cycle_buffer[index];
            else
                return Double.NaN;
        }
        public double get_smoothed_cycle_data(int index)
        {
            if (index < smoothed_cycle_buffer.Length)
                return smoothed_cycle_buffer[index];
            else
                return Double.NaN;
        }

        public int Base_length_value = 0;
        public int base_length_value
        {
            get { return Base_length_value; }
        }
        public class single_cycle_cls
        {
            int Start_index;
            public int start_index
            {
                get { return Start_index; }
                set { Start_index = value; }
            }
            int Length;
            public int length
            {
                get { return Length; }
                set { Length = value; }
            }
        }

        public List<single_cycle_cls> list_of_cycles;
        public registrator_cls(data_storage_cls storage, metronome_cls Metronome)
        {
            Storage = storage;
            metronome = Metronome;
        }
        public bool registering
        {
            set { Registering = value; }
            get { return Registering; }
        }

        private int Cycles_counter = 0;
        public int cycles_counter
        {
            set { Cycles_counter = value; }
            get { return Cycles_counter; }
        }

        public void start_registering()
        {
            Registering = true;
            Cycles_counter = 0;
            Storage.storage_reset();
        }

        public void stop_registering()
        {
            Registering = false;
            calculate_mean_cycle();
        }

        private void calculate_mean_cycle()
        {
            Storage.smooth_data();
            // calculate length of mean cycle
            Base_length_value = metronome.period_ms / 25; // 40 Hz = 25 mSec period

            int i = 0;
            int cycles_counter = 0;
            list_of_cycles = new List<single_cycle_cls>();
            while(i < Storage.current_in_index)
            {
                if(!(Double.IsNaN(Storage.get_data(i))))
                {
                    i++;
                }
                else // here is a NAN delimiter
                {
                    i++;
                    cycles_counter++;
                    list_of_cycles.Add(new single_cycle_cls());
                    list_of_cycles.Last().start_index = i;
                    list_of_cycles.Last().length = 0;
                    while ((i < Storage.current_in_index) && (!(Double.IsNaN(Storage.get_data(i)))))
                    {
                        i++;
                        list_of_cycles.Last().length++;
                    }

                }
            }

            // assemble array of mean cycle
            mean_cycle_buffer = new double[Base_length_value];
            mean_filtered_cycle_buffer = new double[Base_length_value];
            int good_cycles_counter = 0;
            foreach(single_cycle_cls item in list_of_cycles)
            {
                if (Math.Abs(item.length - Base_length_value) <= 1)
                {
                    // add this cycle to mean
                    for (int j = 0; j < Math.Min(item.length, Base_length_value); j++)
                    {
                        mean_cycle_buffer[j] += Storage.get_data(item.start_index + j);
                    }
                    if (item.length < Base_length_value)
                        mean_cycle_buffer[Base_length_value - 1] = mean_cycle_buffer[Base_length_value - 2];
                    good_cycles_counter++;
                }
            }
            // calculate mean
            if (good_cycles_counter > 0)
            {
                for (int j = 0; j < Base_length_value; j++)
                    mean_cycle_buffer[j] /= good_cycles_counter;
            }
            //************************* simple mean ******************************************************

            // create list of j-th elements of cycles
            List<double> j_slice = new List<double>();
            // assemble array of filtered mean cycle
            for (int j = 0; j < Base_length_value; j++)
            {
                // populate list of j-th elements of cycles
                foreach (single_cycle_cls item in list_of_cycles)
                {
                    if(j<=item.length)
                    {
                        //create new item of list
                        j_slice.Add(Storage.get_smoothed_data(item.start_index + j));
                    }
                }
                // create auxiliary array (for sorting)
                double[] sorted_array = new double[j_slice.Count];
                int k = 0;
                foreach(double item in j_slice)
                {
                    sorted_array[k] = item;
                    k++;
                }
 
                j_slice.Clear();

                // sort array
                bubble_sorting(ref sorted_array);
                // calculate mean
                mean_filtered_cycle_buffer[j] = 0;
                int counter = 0;
                // drop 2 maxes and 2 mines
                if(sorted_array.Length >= 5)
                {
                    for(int m=2; m<(sorted_array.Length-2); m++)
                    {
                        mean_filtered_cycle_buffer[j] += sorted_array[m];
                        counter++;
                    }
                    mean_filtered_cycle_buffer[j] /= counter;
                }
                else if (sorted_array.Length >= 1)
                {
                    for (int m = 0; m < sorted_array.Length; m++)
                    {
                        mean_filtered_cycle_buffer[j] += sorted_array[m];
                        counter++;
                    }
                    mean_filtered_cycle_buffer[j] /= counter;
                }
            }

            // smooth filtered mean array ***********************
            smoothed_cycle_buffer = new double[mean_filtered_cycle_buffer.Length];
            Smoother_cls.smooth_double_array(mean_filtered_cycle_buffer, ref smoothed_cycle_buffer);


        }// end private void calculate_mean_cycle()

        private int get_index(int m, int n, int filter_length, int array_length)
        {
            int index = n + (m - filter_length / 2);
            if (index >= 0 && index < array_length)
                return index;
            else if(index < 0)
                return 0;
            else
                return array_length - 1;

        }

        private void bubble_sorting(ref double[] array)
        {
            int length = array.Length;

            if (length >= 2)
            {
                for (int l = length; l >= 2; l--)
                {
                    for (int i = 1; i < l; i++)
                    {
                        if (array[i - 1] > array[i])
                        {
                            double aux = array[i];
                            array[i] = array[i - 1];
                            array[i - 1] = aux;
                        }
                    }
                }
            }
        }// end private void bubble_sorting(double[] array)

        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        //TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS ***** TESTS
        public bool bubble_sorting_test()
        {
            bool result = false;

            double[] source = new double[2] { 1.0, 1.0 };
            double[] right_answer = new double[2] { 1.0, 1.0 };
            //***************************
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[2] { 1, 2 };
            right_answer = new double[2] { 1, 2 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[2] { 2, 1 };
            right_answer = new double[2] { 1, 2 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[3] { 1, 2, 3 };
            right_answer = new double[3] { 1, 2, 3 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[3] { 3, 2, 1 };
            right_answer = new double[3] { 1, 2, 3 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[5] { 1, 2, 3, 4, 5 };
            right_answer = new double[5] { 1, 2, 3, 4, 5 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[5] { 5, 4, 3, 2, 1 };
            right_answer = new double[5] { 1, 2, 3, 4, 5 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);
            //***************************
            source = new double[5] { 3, 2, 2, 2, 1 };
            right_answer = new double[5] { 1, 2, 2, 2, 3 };
            bubble_sorting(ref source);
            result = tester_cls.array_equality(right_answer, source, source.Length);

            return result;
        }

        //******************* TESTS *************************************

    }
}
