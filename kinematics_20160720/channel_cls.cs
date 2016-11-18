using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class channel_cls
    {
        private angle_cls angle;
        private int order_number;

        public channel_cls(angle_cls Angle, int Order_number)
        {
            angle = Angle;
            order_number = Order_number;
        }
    }
}
