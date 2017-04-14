using System.Windows;

using OxyPlot;


namespace kinematics_20160720
{
    /// <summary>
    /// Interaction logic for chart0_window.xaml
    /// </summary>
    public partial class chart0_window : Window
    {
        channel_mean_plot_model view_model;
        public chart0_window()
        {
            InitializeComponent();
            view_model = new channel_mean_plot_model();

            channel0_mean_plot_view.Model = new PlotModel();
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //channel0_mean_plot_view.Model = view_model.plotModel;
           
        }
    }
 
}
