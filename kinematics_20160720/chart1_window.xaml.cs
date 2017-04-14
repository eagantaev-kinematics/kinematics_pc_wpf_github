using System.Windows;

using OxyPlot;

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
