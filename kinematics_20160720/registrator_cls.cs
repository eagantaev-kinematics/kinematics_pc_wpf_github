using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class registrator_cls
    {
        data_storage_cls Storage;
        private bool Registering = false;
        metronom_cls metronome;

        private double[] mean_cycle_buffer;
        public double get_mean_cycle_data(int index)
        {
            if(index < mean_cycle_buffer.Length)
                return mean_cycle_buffer[index];
            else
                return Double.NaN;
        }

        private int Base_length_value = 0;
        public int base_length_value
        {
            get { return Base_length_value; }
        }
        private class single_cycle_cls
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
        public registrator_cls(data_storage_cls storage, metronom_cls Metronome)
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
            // calculate length of mean cycle
            Base_length_value = metronome.period_ms / 25; // 40 Hz = 25 mSec period

            int i = 0;
            int cycles_counter = 0;
            List<single_cycle_cls> list_of_cycles = new List<single_cycle_cls>();
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

        }// end private void calculate_mean_cycle()

    }
}
