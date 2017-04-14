using System;
using System.Linq;

using OxyPlot;
using OxyPlot.Series;

namespace kinematics_20160720
{
    class cycle_chart_cls
    {

        private Double cycle_length; // cycle length in seconds
        private OxyPlot.Wpf.PlotView plotview;

        public cycle_chart_cls(OxyPlot.Wpf.PlotView plotview_parameter, String title)
        {
            plotview = plotview_parameter;

            plotview.Model = new PlotModel();
            OxyPlot.Axes.LinearAxis time_axis = new OxyPlot.Axes.LinearAxis();
            OxyPlot.Axes.LinearAxis angle_axis = new OxyPlot.Axes.LinearAxis();
            time_axis.Position = OxyPlot.Axes.AxisPosition.Bottom;
            angle_axis.Position = OxyPlot.Axes.AxisPosition.Left;
            time_axis.Minimum = 0;
            time_axis.Maximum = 10;
            angle_axis.Minimum = 0;
            angle_axis.Maximum = 180;
            plotview.Model.Axes.Add(time_axis);
            plotview.Model.Axes.Add(angle_axis);
            LineSeries series = new LineSeries();
            plotview.Model.Series.Add(series);

            plotview.Model.Title = title;
        }

        public void set_time_axis(double min_value, double max_value)
        {
            plotview.Model.Axes.ToArray()[0].Minimum = min_value;
            plotview.Model.Axes.ToArray()[0].Maximum = max_value;
        }

        public void set_angle_axis(double min_value, double max_value)
        {
            plotview.Model.Axes.ToArray()[1].Minimum = min_value;
            plotview.Model.Axes.ToArray()[1].Maximum = max_value;
        }
    }
}
