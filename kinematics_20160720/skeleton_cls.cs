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

        private int Active_joint_index = 0;
        public int active_joint_index
        {
            set { Active_joint_index = value; }
            get { return Active_joint_index; }
        }
        private int Active_angle_index = 0;
        public int active_angle_index
        {
            set { Active_angle_index = value; }
            get { return Active_angle_index; }
        }

        private segment_cls segment;
        private joint_cls joint;

        private raw_data_storage_cls raw_data_storage;
        private raw_data_frame_cls newest_frame;

        // public constructor
        public skeleton_cls(raw_data_storage_cls raw_data_storage_parameter)
        {
            raw_data_storage = raw_data_storage_parameter;

            segment = new segment_cls(0, 0);     // dummy fictious zero segment
            Joints = new List<joint_cls>();

            segments = new List<segment_cls>();
            segments.Add(segment);
            // formiruem spisok segmentov
            segment = new segment_cls(1, 14);       // golova
            segments.Add(segment);                  
            segment = new segment_cls(2, 13);       // sheya
            segments.Add(segment);
            segment = new segment_cls(3, 12);       // grud'
            segments.Add(segment);
            segment = new segment_cls(4, 8);        // zhivot
            segments.Add(segment);
            segment = new segment_cls(5, 15);       // kopchik
            segments.Add(segment);
            segment = new segment_cls(6, 0);        // levoe nadplech
            segments.Add(segment);
            segment = new segment_cls(7, 1);        // levoe plecho
            segments.Add(segment);
            segment = new segment_cls(8, 2);        // levoe predplech'e
            segments.Add(segment);
            segment = new segment_cls(9, 3);        // levaya kist'
            segments.Add(segment);
            segment = new segment_cls(10, 9);       // levoe bedro
            segments.Add(segment);
            segment = new segment_cls(11, 10);      // levaya golen'
            segments.Add(segment);
            segment = new segment_cls(12, 11);      // levaya stopa
            segments.Add(segment);
            segment = new segment_cls(13, 4);       // pravoe nadplech'e
            segments.Add(segment);
            segment = new segment_cls(14, 5);       // pravoe plecho
            segments.Add(segment);
            segment = new segment_cls(15, 6);       // pravoe predplech'e
            segments.Add(segment);
            segment = new segment_cls(16, 7);       // pravaya kist'
            segments.Add(segment);
            segment = new segment_cls(17, 16);      // pravoe bedro
            segments.Add(segment);
            segment = new segment_cls(18, 17);      // pravaya golen'
            segments.Add(segment);
            segment = new segment_cls(19, 18);      // pravaya stopa
            segments.Add(segment);

            // formiruem spisok sustavov
            joint = new joint_cls(segments.ToArray()[2], segments.ToArray()[1], "Шея верх"); // neck high
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[2], "Шея низ"); // neck low
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[4], segments.ToArray()[3], "Грудной отдел"); // chest
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[5], segments.ToArray()[4], "Поясничный отдел"); // reins
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[6], "Ключица левая"); // collarbone left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[6], segments.ToArray()[7], "Плечо левое"); // sholder left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[7], segments.ToArray()[8], "Локоть левый"); // elbow left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[8], segments.ToArray()[9], "Запястье левое"); // wrist left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[3], segments.ToArray()[13], "Ключица правая"); // collarbone right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[13], segments.ToArray()[14], "Плечо правое"); // sholder right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[14], segments.ToArray()[15], "Локоть правый"); // elbow right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[15], segments.ToArray()[16], "Запястье правое"); // wrist right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[5], segments.ToArray()[10], "Тазобедренный левый"); // hip left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[10], segments.ToArray()[11], "Колено левое"); // knee left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[11], segments.ToArray()[12], "Голеностоп левый"); // ankle left
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[5], segments.ToArray()[17], "Тазобедренный правый"); // hip right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[17], segments.ToArray()[18], "Колено правое"); // knee right
            Joints.Add(joint);
            joint = new joint_cls(segments.ToArray()[18], segments.ToArray()[19], "Голеностоп правый"); // ankle right
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
