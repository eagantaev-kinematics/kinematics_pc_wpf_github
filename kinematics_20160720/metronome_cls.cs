using System;
using System.Threading;


namespace kinematics_20160720
{
    public class metronome_cls
    {

        private int Tick_length;        // tick length in mSec
        public int tick_length
        {
            set { Tick_length = value; }
            get { return Tick_length; }
        }
        private int Ticks_in_cycle;
        public int ticks_in_cycle
        {
            set { Ticks_in_cycle = value; }
            get { return Ticks_in_cycle; }
        }
        private int Period_ms;
        public int period_ms
        {
            set { Period_ms = value; }
            get { return Period_ms; }
        }
        private int Lamp_period_ms = 300;
        public int lamp_period_ms
        {
            set { Lamp_period_ms = value; }
            get { return Lamp_period_ms; }
        }

        

        private bool Metronome_on = false;
        public bool metronome_on
        {
            set { Metronome_on = value; }
            get { return Metronome_on; }
        }

        EventArgs e;
        public event EventHandler Metronome_tick;
        public event EventHandler Metronome_master_tick;
        public event EventHandler Lamp_on;
        public event EventHandler Lamp_off;

        // public constructor
        public metronome_cls()
        {
            Tick_length = 1000;     // default values: tick - second; two ticks in cycle
            Ticks_in_cycle = 2;

            Period_ms = 2000;
        }


        public void metronome_thread_method()
        {
            int cycle_tick_counter = 0;
            e = new EventArgs();

            while (Metronome_on)
            {
                //Event will be null if there are no subscribers
                if (Lamp_on != null)
                    Lamp_on(this, e);

                //Lamp_on_delegate();

                Thread.Sleep(Lamp_period_ms);
                // Event will be null if there are no subscribers
                if (Lamp_off != null)
                    Lamp_off(this, e);
                Thread.Sleep(Tick_length - Lamp_period_ms);
                cycle_tick_counter++;
                
                if((cycle_tick_counter % Ticks_in_cycle) == 0)  // master tick time
                {
                    cycle_tick_counter = 0;
                    // Event will be null if there are no subscribers
                    if (Metronome_master_tick != null)
                        Metronome_master_tick(this, e);  // raise master tick event
                }
                else
                {
                    // Event will be null if there are no subscribers
                    if (Metronome_tick != null)
                        Metronome_tick(this, e);         // raise tick event
                }

            }
        }


    }
}
