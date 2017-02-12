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

        public static bool array_equality(double[] array1, double[] array2, int length)
        {
            bool result = true;
            double EPSILON = 0.00000001;

            double abs_min = 0.0;
            // look up for abs min of array
            for (int i = 0; i < length; i++)
            {
                if (abs_min > Math.Abs(array1[i]))
                    abs_min = Math.Abs(array1[i]);
                if (abs_min > Math.Abs(array2[i]))
                    abs_min = Math.Abs(array2[i]);
            }

            if (abs_min != 0.0)
                EPSILON = abs_min * 0.00001;

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(array1[i] - array2[i]) > EPSILON)
                    result = false;
            }

            return result;
        }

        public static bool equality(int right_answer, int result)
        {
            if (right_answer == result)
                return true;
            else
                return false;
        }

        public static bool epsilon_equality(double right_answer, double result, double epsilon)
        {
            if (Math.Abs(right_answer - result) <= epsilon)
                return true;
            else
                return false;
        }

    }
}
