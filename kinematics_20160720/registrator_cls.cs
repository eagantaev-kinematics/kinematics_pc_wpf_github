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

        public registrator_cls(data_storage_cls storage)
        {
            Storage = storage;
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
        }
    }
}
