using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace kinematics_20160720
{
    class metronom_cls
    {
        private int Period_ms = 1000;
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

        private bool Lamp_on = false;
        public bool lamp_on
        {
            set { Lamp_on = value; }
            get { return Lamp_on; }
        }


    }
}
