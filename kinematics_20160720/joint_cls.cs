using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class joint_cls
    {
        //........................................................
        private int[] id;
        private segment_cls segment0, segment1;
        private Double YY_axis_angle;
        public Double yy_axis_angle
        {
            get { return YY_axis_angle; }
        }

        //........................................................

        public joint_cls(segment_cls segment0_parameter, segment_cls segment1_parameter)
        {
            id = new int[2];

            segment0 = segment0_parameter;
            segment1 = segment1_parameter;

            id[0] = segment0.id;
            id[1] = segment1.id;
        }
        public void calculate_angles()
        {

            Double X1 = segment0.get_X();
            Double X2 = segment1.get_X();
            Double Y1 = segment0.get_Y();
            Double Y2 = segment1.get_Y();
            Double Z1 = segment0.get_Z();
            Double Z2 = segment1.get_Z();
            Double n1 = Math.Sqrt(X1 * X1 + Y1 * Y1 + Z1 * Z1);
            Double n2 = Math.Sqrt(X2 * X2 + Y2 * Y2 + Z2 * Z2);
            Double angle_1_2 = Math.Acos((X1 * X2 + Y1 * Y2 + Z1 * Z2) / n1 / n2);
            YY_axis_angle = angle_1_2 * 180.0 / Math.PI;
        }


    }
}
