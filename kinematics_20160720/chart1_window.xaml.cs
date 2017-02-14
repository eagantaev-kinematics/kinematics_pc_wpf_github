using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using OxyPlot;
using OxyPlot.Series;

namespace kinematics_20160720
{
    /// <summary>
    /// Interaction logic for chart1_window.xaml
    /// </summary>
    public partial class chart1_window : Window
    {
        channel_mean_plot_model view_model;
        public chart1_window()
        {
            InitializeComponent();
            view_model = new channel_mean_plot_model();

            channel1_mean_plot_view.Model = new PlotModel();
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //channel0_mean_plot_view.Model = view_model.plotModel;
           
        }
    }
}
