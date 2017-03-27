using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    public class angle_cls
    {
        private segment_cls segment1, segment2;
        private Double angle;

        public Double Angle
        {
            get { return angle; }
        }
        public angle_cls(segment_cls segm1, segment_cls segm2)
        {
            segment1 = segm1;
            segment2 = segm2;
        }

        public void calculate()
        {
            //segment1.calculate_segment_position();
            //segment2.calculate_segment_position();

            Double X1 = segment1.get_X();
            Double X2 = segment2.get_X();
            Double Y1 = segment1.get_Y();
            Double Y2 = segment2.get_Y();
            Double Z1 = segment1.get_Z();
            Double Z2 = segment2.get_Z();
            Double n1 = Math.Sqrt(X1 * X1 + Y1 * Y1 + Z1 * Z1);
            Double n2 = Math.Sqrt(X2 * X2 + Y2 * Y2 + Z2 * Z2);
            Double angle_1_2 = Math.Acos((X1 * X2 + Y1 * Y2 + Z1 * Z2) / n1 / n2);
            angle = angle_1_2 * 180.0 / Math.PI;
        }
    }
}
