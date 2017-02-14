using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Series;

namespace kinematics_20160720
{
    class channel_mean_plot_model
    {

        public PlotModel plotModel { get; private set; }
        public channel_mean_plot_model()
        {
            //this.MyModel = new PlotModel { Title = "channel 1" };
            this.plotModel = new PlotModel();


            OxyPlot.Series.LineSeries series0 = new LineSeries();
            series0.Color = OxyColor.FromRgb(255, 0, 0);

            series0.Points.Add(new DataPoint(0, 0));
            series0.Points.Add(new DataPoint(1, 1));
            this.plotModel.Series.Add(series0);


            /*
            OxyPlot.Series.FunctionSeries series1 = new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)");
            series1.Color = OxyColor.FromRgb(255, 0, 0);
            OxyPlot.Series.FunctionSeries series2 = new FunctionSeries(Math.Sin, 0, 10, 0.1, "sin(x)");
            series2.Color = OxyColor.FromRgb(0, 0, 255);

            
            this.MyModel.Series.Add(series1);
            this.MyModel.Series.Add(series2);
            */
        }

    }
}




