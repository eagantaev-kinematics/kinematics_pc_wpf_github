namespace kinematics_20160720
{
    public class gyroscope_cls
    {
        private int X, Y, Z;
        public int x
        {
            set { X = value; }
            get { return X; }
        }

        public int y
        {
            set { Y = value; }
            get { return Y; }
        }

        public int z
        {
            set { Z = value; }
            get { return Z; }
        }

        private histogram_cls Histo;
        public histogram_cls histogram
        {
            set { Histo = value; }
            get { return Histo; }
        }
        public gyroscope_cls()
        {
            Histo = new histogram_cls(160, 13, 40);
        }
    }
}
