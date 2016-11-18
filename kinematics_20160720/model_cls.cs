using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    
    class model_cls
    {
        private Segment_cls[] segments;
        public Segment_cls[] Segments
        {
            get { return segments; }
        }

        private List<channel_cls> channels;
        private int number_of_channels = 0;

        public model_cls(raw_kinematics_data_cls raw_data)
        {
            channels = new List<channel_cls>();
            number_of_channels = 0;

            segments = new Segment_cls[20];

            for (int i = 1; i <= 19; i++)
            {
                segments[i] = new Segment_cls(i, raw_data);
            }
            segments[0] = null;
        }

        public void add_channel(angle_cls angle)
        {
            number_of_channels++;
            channels.Add(new channel_cls(angle, number_of_channels));
        }
    }
}
