using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using OxyPlot;
using OxyPlot.Series;

namespace kinematics_20160720
{
    class joint_chart_pannel_cls
    {
        private int FRAME_PERIOD = 24;

        private timeline_chart_cls timeline_chart;
        private cycle_chart_cls frontal_chart, saggital_chart, horizontal_chart;

        //private OxyPlot.Wpf.PlotView plotview;
        private PlotModel plotmodel;

        // debug 
        int counter = 0;
        Boolean do_job = true;

        // public constructor
        public joint_chart_pannel_cls(ref OxyPlot.Wpf.PlotView plotview)
        {
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
        }

        EventArgs e;
        public event EventHandler chart_update_tick;
        public void chart_update_thread_method()
        {
            while (do_job)
            {
                // every ... mSec request current joint data and display them
                Thread.Sleep(FRAME_PERIOD);

                // request data
                //Event will be null if there are no subscribers
                if (chart_update_tick != null)
                    chart_update_tick(this, e);

                // draw data *****************************************************************
                counter++;
            }
        }
    }
}
