namespace kinematics_20160720
{
    class channel_cls
    {
        private angle_cls angle;
        public angle_cls Angle 
        {
            get { return angle; }
        }
        private int order_number;

        public channel_cls(angle_cls Angle, int Order_number)
        {
            angle = Angle;
            order_number = Order_number;
        }
    }
}
