using System;

namespace kinematics_20160720
{
    class joint_cls
    {

        int FRAME_LENGTH = 25;
        //........................................................
        private int[] id;
        private String Name;
        public String name
        {
            get { return Name; }
        }
        private segment_cls segment0, segment1;
        private Double YY_axis_angle;
        public Double yy_axis_angle
        {
            get { return YY_axis_angle; }
        }
        private Double Frontal_angle;
        public Double frontal_angle
        {
            get { return Frontal_angle; }
        }

        private Double Sagittal_angle;
        private Double Horizontal_angle;

        private Double[] Angles;
        public Double[] angles
        {
            get { return Angles; }
        }

        private Double[,] Mean_cycle;
        public Double[,] mean_cycle
        {
            get { return Mean_cycle; }
        }
        private int[] Mean_cycle_sample_counters;
        private int Mean_cycle_length;
        public int mean_cycle_length
        {
            get { return Mean_cycle_length; }
        }
        private int Mean_cycle_index = 0;

        private Boolean Registering_flag = false;
        public Boolean registering_flag
        {
            get { return Registering_flag; }
        }

        //........................................................

        public joint_cls(segment_cls segment0_parameter, segment_cls segment1_parameter, String name_parameter)
        {
            id = new int[2];

            segment0 = segment0_parameter;
            segment1 = segment1_parameter;

            id[0] = segment0.id;
            id[1] = segment1.id;

            Name = name_parameter;

            Angles = new Double[4];
        }
        public void calculate_angles()
        {

            Double X1 = segment0.get_X();
            Double X2 = segment1.get_X();
            Double Y1 = segment0.get_Y();
            Double Y2 = segment1.get_Y();
            Double Z1 = segment0.get_Z();
            Double Z2 = segment1.get_Z();

            // calculate angle between axisses
            Double n1 = Math.Sqrt(X1 * X1 + Y1 * Y1 + Z1 * Z1);
            Double n2 = Math.Sqrt(X2 * X2 + Y2 * Y2 + Z2 * Z2);
            Double angle_1_2 = Math.Acos((X1 * X2 + Y1 * Y2 + Z1 * Z2) / n1 / n2);
            YY_axis_angle = angle_1_2 * 180.0 / Math.PI;
            Angles[0] = YY_axis_angle;

            // calculate frontal projection
            Double arg = X2 / Z2;
            if(!Double.IsNaN(arg))
                //Frontal_angle = -(Math.Atan((X2/Z2)))*180/Math.PI;
                Frontal_angle = Math.Sign(X2)*(Math.Acos((Z2/Math.Sqrt(X2*X2 + Z2*Z2)))) * 180 / Math.PI;
            Angles[1] = Frontal_angle;

            // calculate sagittal projection
            Sagittal_angle = -Math.Sign(Y2) * (Math.Acos((Z2 / Math.Sqrt(Y2 * Y2 + Z2 * Z2)))) * 180 / Math.PI; ;
            Angles[2] = Sagittal_angle;


            // calculate horizontal projection
            Horizontal_angle = Math.Sign(X2) * (Math.Acos((-Y2 / Math.Sqrt(X2 * X2 + Y2 * Y2)))) * 180 / Math.PI; ;
            Angles[3] = Horizontal_angle;

            if (Registering_flag)
            {
                // if registering, save data in mean cycle array
                for (int i = 0; i < 4; i++)
                {
                    Mean_cycle[i, Mean_cycle_index] += Angles[i];   // uppend data to mean cycle integral
                }
                Mean_cycle_sample_counters[Mean_cycle_index]++;
                //
                Mean_cycle_index++; 
            }
        }

        public void start_register(metronome_cls metronome)
        {
            Mean_cycle_length = (metronome.tick_length * metronome.ticks_in_cycle) / FRAME_LENGTH;
            Mean_cycle = new Double[4, Mean_cycle_length + 10];
            Mean_cycle_sample_counters = new int[Mean_cycle_length + 10];

            Registering_flag = true;
        }

        public void metronome_master_tick_reset()
        {
            Mean_cycle_index = 0;
        }

        public void stop_register()
        {
            // calculate mean cycle
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Mean_cycle_length; j++)
                {
                    if (Mean_cycle_sample_counters[j] != 0)
                        Mean_cycle[i, j] /= Mean_cycle_sample_counters[j];   // calculate mean
                }
            }

            Mean_cycle_index = 0;
            Registering_flag = false;
        }


    }
}
