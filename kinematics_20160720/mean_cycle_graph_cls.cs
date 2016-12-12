using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Shapes;


namespace kinematics_20160720
{
    class mean_cycle_graph_cls
    {
        private int MIN_RANGE = 0;
        private int MAX_RANGE = 180;
        private int MARGIN = 10;

        private Canvas canvas;
        private double height;
        private double width;
        private double vertical_ratio;
        //private double horizontal_ratio;

        private int current_X = 0;
        private double old_Y = -1;

        public mean_cycle_graph_cls(Canvas graph_canvas)
        {
            canvas = graph_canvas;
            height = canvas.Height;
            width = canvas.Width;

            vertical_ratio = height / (MAX_RANGE - MIN_RANGE - 2*MARGIN);
        }
        public void add_stroke(double new_value)
        {
            if((new_value >= MIN_RANGE) && (new_value <= MAX_RANGE))
            {
                Line new_stroke = new Line();
                new_stroke.StrokeThickness = 2;
                new_stroke.Stroke = System.Windows.Media.Brushes.Green;
                new_stroke.X1 = current_X;
                new_stroke.X2 = (current_X+=5);
                new_stroke.Y2 = (height - MARGIN) - vertical_ratio * new_value;
                if (old_Y == -1)
                    new_stroke.Y1 = new_stroke.Y2;
                else
                    new_stroke.Y1 = old_Y;
                //new_stroke.Y2 = (height - MARGIN) - (Math.Sin(0.01*current_X)*75 + 80);
                old_Y = new_stroke.Y2;
                canvas.Children.Add(new_stroke);
                canvas.UpdateLayout();

                if(current_X >= width)
                {
                    current_X = 0;
                    canvas.Children.Clear();
                }
            }
        }

        public void reset_graph()
        {
            current_X = 0;
            old_Y = -1;
            canvas.Children.Clear();
        }


    }
}
