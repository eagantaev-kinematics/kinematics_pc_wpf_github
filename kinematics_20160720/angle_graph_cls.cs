using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Shapes;

namespace kinematics_20160720
{
    class angle_graph_cls
    {
        private int MIN_RANGE = 0;
        private int MAX_RANGE = 180;
        private int MARGIN = 10;

        private Canvas canvas;
        private double height;
        private double width;
        private double vertical_ratio;
        private double horizontal_ratio;

        private int current_X = 0;
        private double old_Y = 0;

        public angle_graph_cls(Canvas graph_canvas)
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
                new_stroke.Stroke = System.Windows.Media.Brushes.Blue;
                new_stroke.X1 = current_X;
                new_stroke.X2 = (current_X+=2);
                new_stroke.Y1 = old_Y;
                new_stroke.Y2 = (height - MARGIN) - vertical_ratio * new_value;
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

        public void add_metronome_marker_stroke()
        {
                Line marker_stroke = new Line();
                marker_stroke.StrokeThickness = 3;
                marker_stroke.Stroke = System.Windows.Media.Brushes.Red;
                marker_stroke.X1 = current_X;
                marker_stroke.X2 = current_X;
                marker_stroke.Y1 = MARGIN;
                marker_stroke.Y2 = (height - MARGIN);
                canvas.Children.Add(marker_stroke);
                canvas.UpdateLayout();

        }



    }
}
