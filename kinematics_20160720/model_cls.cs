using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    
    class model_cls
    {
        private segment_cls[] segments;
        public segment_cls[] Segments
        {
            get { return segments; }
        }

        private List<channel_cls> channels;
        public List<channel_cls> Channels
        {
            get { return channels; }
        }
        private int number_of_channels = 0;

        public model_cls(raw_kinematics_data_cls raw_data)
        {
            channels = new List<channel_cls>();
            number_of_channels = 0;

            segments = new segment_cls[20];

            for (int i = 1; i <= 19; i++)
            {
                segments[i] = new segment_cls(i);
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
