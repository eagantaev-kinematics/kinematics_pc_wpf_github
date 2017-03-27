using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinematics_20160720
{
    class skeleton_cls
    {
        private int NUMBER_OF_SEGMENTS = 19;

        private List<segment_cls> segments;
        private List<joint_cls> Joints;
        public List<joint_cls> joints
        {
            get { return Joints; }
        }

        private segment_cls segment;
        private joint_cls joint;

        private raw_data_storage_cls raw_data_storage;
        private raw_data_frame_cls newest_frame;

        // public constructor
        public skeleton_cls(raw_data_storage_cls raw_data_storage_parameter)
        {
            raw_data_storage = raw_data_storage_parameter;

            segment = new segment_cls(0);     // dummy fictious zero segment
            Joints = new List<joint_cls>();

            segments = new List<segment_cls>();
            segments.Add(segment);
            // formiruem spisok segmentov
            for (int i = 1; i <= NUMBER_OF_SEGMENTS; i++)
            {
                segment = new segment_cls(i);
                segments.Add(segment);
            }

            // formiruem spisok sustavov
            joint = new joint_cls(segments.ToArray()[1], segments.ToArray()[2]); // neck high
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[2], segments.ToArray()[3]); // neck low
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[4]); // chest
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[4], segments.ToArray()[5]); // reins
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[6]); // collarbone left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[6], segments.ToArray()[7]); // sholder left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[7], segments.ToArray()[8]); // elbow left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[8], segments.ToArray()[9]); // wrist left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[13]); // collarbone right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[13], segments.ToArray()[14]); // sholder right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[14], segments.ToArray()[15]); // elbow right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[15], segments.ToArray()[16]); // wrist right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[5], segments.ToArray()[10]); // hip left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[10], segments.ToArray()[11]); // knee left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[11], segments.ToArray()[12]); // ankle left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[5], segments.ToArray()[17]); // hip right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[17], segments.ToArray()[18]); // knee right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[18], segments.ToArray()[19]); // ankle right
            Joints.Add(joint);

        }

        // this method we call when new kinimatics data package is received
        public void update_data()
        {
            newest_frame = raw_data_storage.frame;
            // populate segments with their data
            for(int i=1; i<=NUMBER_OF_SEGMENTS; i++)
            {
                (segments.ToArray())[i].calculate_segment_position(newest_frame);
            }

            int NUMBER_OF_JOINTS = Joints.Count();

            for (int i =0; i < NUMBER_OF_JOINTS; i++)
            {
                (Joints.ToArray())[i].calculate_angles();
            }
        }

        


     }
}
