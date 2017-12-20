using System;
using System.Collections.Generic;
using System.IO;

namespace kinematics_20160720
{
    class raw_data_storage_cls
    {
        private udp_receiver_cls udp_receiver;
        private List<raw_data_frame_cls> raw_data_frames;
        private raw_data_frame_cls Frame;
        public raw_data_frame_cls frame
        {
            //set { Frame = value; }
            get { return Frame; }
        }
        Boolean Registering_now_flag = false;
        public Boolean registering_now_flag
        {
            set { Registering_now_flag = value; }
            get { return Registering_now_flag; }
        }

        BinaryWriter raw_frame_file_storage;
        //..................................................

        // public constructor
        public raw_data_storage_cls(udp_receiver_cls udp_receiver_parameter)
        {
            udp_receiver = udp_receiver_parameter;
            raw_data_frames = new List<raw_data_frame_cls>();
        }



        public void add_new_frame()
        {
            Frame = udp_receiver.raw_data_frame;

            if (Registering_now_flag)
            {
                raw_data_frames.Add(udp_receiver.raw_data_frame);
            }
        }
    }
}
