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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.Text;
using System.IO;
//using System.IO.Ports;
using System.Net.Sockets;
using System.Net;



namespace kinematics_20160720
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // *****
        raw_kinematics_data_cls raw_data;
        model_cls model;

        //***** calibration
        histogram_cls histogram;

        private Int32 packet_counter = 0;
        private delegate void NoArgDelegate();

        //udp***
        UdpClient kinematics_listener;
        IPEndPoint remote_endpoint = new IPEndPoint(0, 0);

        IPEndPoint local_kinematics_endpoint = new IPEndPoint(0, 0);



        string debug_string = "no data\n";

        public MainWindow()
        {
            InitializeComponent();

            string Host = System.Net.Dns.GetHostName();
            debug_info_panel.Content += "my host -> " + Host + "\r\n";
            string IP1 = System.Net.Dns.GetHostByName(Host).AddressList[0].ToString();
            debug_info_panel.Content += "my ip -> " + IP1 + "\r\n";

            //local_kinematics_endpoint.Address = IPAddress.Parse("192.168.1.1");
            local_kinematics_endpoint.Address = IPAddress.Parse(IP1);
            local_kinematics_endpoint.Port = 112;

            kinematics_listener = new UdpClient();

            try
            {
                kinematics_listener.Client.Bind(local_kinematics_endpoint);
            }
            catch (Exception e)
            {
                
            }

            raw_data = new raw_kinematics_data_cls();
            model = new model_cls(raw_data);
            // fill model
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[2]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[3], model.Segments[4]));


            histogram = new histogram_cls(160, 13, 40);  // object just to run tests
            //model.Segments[1].sensor.accelerometer.histogram = new histogram_cls(160, 13, 40);
            
        }

        private Thread dataReceivingThread;
        private Boolean doJob = true;


        private void dataReceivingMethod()
        {
            doJob = true;

            while (doJob)
            {
                Thread.Sleep(5);

                //***************************************************************************

                // читаем данные
                if (kinematics_listener.Available > 0)
                {

                    try
                    {
                        // udp
                        raw_data.Kinematics_Data = kinematics_listener.Receive(ref remote_endpoint);
                        packet_counter++;
                        debug_string = "packet counter - " + packet_counter.ToString() + "\n";
                    }
                    catch (Exception e)
                    {
                        debug_string = "udp data read fail!!!  " + e.ToString() + "\n";
                    }

                }

                //***************************************************************************
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new NoArgDelegate(UpdateUserInterface));
            }

        }// end dataReceivingMethod

        private void UpdateUserInterface()
        {
            

            info_panel_label.Content = debug_string;
            info_panel_label.UpdateLayout();

            if (raw_data.Kinematics_Data.Length == raw_data.Raw_Data_Length)
            {
                data_panel_label.Content = "";
                for(int i=0; i<19; i++)
                {
                    string data_string = "";
                    for(int j=0; j<9; j++)
                    {
                        Int16 data = (Int16)((Int16)raw_data.Kinematics_Data[i * 18 + j * 2] + ((Int16)(raw_data.Kinematics_Data[i * 18 + j * 2 + 1]) << 8));
                        data_string += String.Format("{0, 10}  ", data);
                    }
                    data_string += "\n";
                    data_panel_label.Content += data_string;
                }
            }
            data_panel_label.UpdateLayout();
            
            for (int i = 1; i <= 19; i++)
                model.Segments[i].calculate_segment_position();
            //*
            (model.Channels.ToArray())[0].Angle.calculate();
            (model.Channels.ToArray())[1].Angle.calculate();
            (model.Channels.ToArray())[3].Angle.calculate();
            Double angle1 = (model.Channels.ToArray())[0].Angle.Angle;
            Double angle2 = (model.Channels.ToArray())[1].Angle.Angle;
            Double angle3 = (model.Channels.ToArray())[3].Angle.Angle;


            segment1_axis.Content = String.Format("{0,10:F3}", angle1);
            segment2_axis.Content = String.Format("{0,10:F3}", angle2);
            segment_1_2_angle.Content = String.Format("{0,10:F3}", angle3);
            //********************************************************************
            //*/

            //*
            model.Segments[1].sensor.accelerometer.histogram_x.add_value(model.Segments[1].sensor.accelerometer.x);
            if(packet_counter % 40 == 0)
            {
                hist_1_1_label.Content = "";
                for (int i = 0; i < model.Segments[1].sensor.accelerometer.histogram_x.bins.Length; i++)
                    hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                hist_1_1_label.Content += "|| " + model.Segments[1].sensor.accelerometer.histogram_x.main_bin.ToString();
            }
            model.Segments[1].sensor.accelerometer.histogram_y.add_value(model.Segments[1].sensor.accelerometer.y);
            if (packet_counter % 40 == 0)
            {
                hist_1_2_label.Content = "";
                for (int i = 0; i < model.Segments[1].sensor.accelerometer.histogram_y.bins.Length; i++)
                    hist_1_2_label.Content += model.Segments[1].sensor.accelerometer.histogram_y.bins[i].ToString() + " ";
                hist_1_2_label.Content += "|| " + model.Segments[1].sensor.accelerometer.histogram_y.main_bin.ToString();
            }
            model.Segments[1].sensor.accelerometer.histogram_z.add_value(model.Segments[1].sensor.accelerometer.z);
            if (packet_counter % 40 == 0)
            {
                hist_1_3_label.Content = "";
                for (int i = 0; i < model.Segments[1].sensor.accelerometer.histogram_z.bins.Length; i++)
                    hist_1_3_label.Content += model.Segments[1].sensor.accelerometer.histogram_z.bins[i].ToString() + " ";
                hist_1_3_label.Content += "|| " + model.Segments[1].sensor.accelerometer.histogram_z.main_bin.ToString();
            }
            //*/
            //*
            model.Segments[2].sensor.accelerometer.histogram_x.add_value(model.Segments[2].sensor.accelerometer.x);
            if (packet_counter % 40 == 0)
            {
                hist_2_1_label.Content = "";
                for (int i = 0; i < model.Segments[2].sensor.accelerometer.histogram_x.bins.Length; i++)
                    hist_2_1_label.Content += model.Segments[2].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                hist_2_1_label.Content += "|| " + model.Segments[2].sensor.accelerometer.histogram_x.main_bin.ToString();
            }
            //*/
        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            start_button.Content = "Started";
            start_button.IsEnabled = false;

            stop_button.IsEnabled = true;

            dataReceivingThread = new Thread(new ThreadStart(this.dataReceivingMethod));
            dataReceivingThread.IsBackground = true;
            dataReceivingThread.Start();

        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            //Environment.Exit(0);
            doJob = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dataReceivingThread.Abort();
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

        private void main_window_Loaded(object sender, RoutedEventArgs e)
        {
            test_results_panel.Content += "\r\n --> ";

            test_results_panel.Content += histogram.bubble_sort_test().ToString() + " bubble_sort_test \r\n --> ";
            test_results_panel.Content += histogram.mean_calculation_test().ToString() + " mean_calculation_test \r\n --> ";
            test_results_panel.Content += histogram.sigma_calculation_test().ToString() + " sigma_calculation_test \r\n --> ";
            test_results_panel.Content += histogram.bins_calculation_test().ToString() + " bins_calculation_test \r\n --> ";
        }


    }
}// end namespace kinematics_20160720
