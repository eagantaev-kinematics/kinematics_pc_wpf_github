namespace kinematics_20160720
{
    public class sensor_cls
    {

        private int X, Y, Z;
        public int x
        {
            set { X = value; XYZ[0] = X; }
            get { return X; }
        }

        public int y
        {
            set { Y = value; XYZ[1] = Y; }
            get { return Y; }
        }

        public int z
        {
            set { Z = value; XYZ[2] = Z; }
            get { return Z; }
        }
        private int[] XYZ = new int[3];
        public int[] xyz
        {
            get { return XYZ; }
        }

        private histogram_cls Histo_x, Histo_y, Histo_z;
        public histogram_cls histogram_x
        {
            set { Histo_x = value; }
            get { return Histo_x; }
        }
        public histogram_cls histogram_y
        {
            set { Histo_y = value; }
            get { return Histo_y; }
        }
        public histogram_cls histogram_z
        {
            set { Histo_z = value; }
            get { return Histo_z; }
        }

        private histogram_cls[] Histogram_array = new histogram_cls[3];
        public histogram_cls[] histogram_array
        {
            get { return Histogram_array; }
        }
        
        public sensor_cls()
        {
            Histo_x = new histogram_cls(160, 13, 40);
            Histo_y = new histogram_cls(160, 13, 40);
            Histo_z = new histogram_cls(160, 13, 40);

            Histogram_array[0] = Histo_x;
            Histogram_array[1] = Histo_y;
            Histogram_array[2] = Histo_z;

            
        }
    }
}
