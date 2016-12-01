using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class accelerometer_cls
    {
        private int X, Y, Z;
        public int x
        {
            set { X = value; }
            get { return X; }
        }

        public int y
        {
            set { Y = value; }
            get { return Y; }
        }

        public int z
        {
            set { Z = value; }
            get { return Z; }
        }

        private histogram_cls Histo_x, Histo_y, Histo_z;
        public histogram_cls histogram_x
        {
            set { Histo_x = value; }
            get { return Histo_x; }
        }
        public histogram_cls histogram_y
        {
            set { Histo_y = value; }
            get { return Histo_y; }
        }
        public histogram_cls histogram_z
        {
            set { Histo_z = value; }
            get { return Histo_z; }
        }

        public accelerometer_cls()
        {
            Histo_x = new histogram_cls(160, 13, 40);
            Histo_y = new histogram_cls(160, 13, 40);
            Histo_z = new histogram_cls(160, 13, 40);
        }

    }
}
