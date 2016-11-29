using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class tester_cls
    {
        histogram_cls histogram;
        public tester_cls(histogram_cls Histogram)
        {
            histogram = Histogram;
        }

        //**********************************************************************************

        public static bool array_equality(int[] array1, int[] array2, int length)
        {
            bool result = true;

            for (int i = 0; i < length; i++ )
            {
                if (array1[i] != array2[i])
                    result = false;
            }

                return result;
        }

    }
}
