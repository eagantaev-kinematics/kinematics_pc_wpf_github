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

            histogram_cls hist;
            Canvas canv;
            Label lbl;
            //* 1 row
            hist = model.Segments[1].sensor.accelerometer.histogram_x;
            hist.add_value(model.Segments[1].sensor.accelerometer.x);
            if(packet_counter % 40 == 0)
            {

                hist_1_1_label.Content = "";
                hist_1_1_canvas.Children.Clear();
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = hist_1_1_canvas.ActualHeight * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;  
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = hist_1_1_canvas.ActualHeight;
                    bin_stroke.Y2 = hist_1_1_canvas.ActualHeight - (hist.bins[i] * ratio);
                    hist_1_1_canvas.Children.Add(bin_stroke);
                }
                hist_1_1_label.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[1].sensor.accelerometer.histogram_y;
            hist.add_value(model.Segments[1].sensor.accelerometer.y);
            if (packet_counter % 40 == 0)
            {

                hist_1_2_label.Content = "";
                hist_1_2_canvas.Children.Clear();
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = hist_1_2_canvas.ActualHeight * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_2_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = hist_1_2_canvas.ActualHeight;
                    bin_stroke.Y2 = hist_1_2_canvas.ActualHeight - (hist.bins[i] * ratio);
                    hist_1_2_canvas.Children.Add(bin_stroke);
                }
                hist_1_2_label.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[1].sensor.accelerometer.histogram_z;
            hist.add_value(model.Segments[1].sensor.accelerometer.z);
            if (packet_counter % 40 == 0)
            {

                hist_1_3_label.Content = "";
                hist_1_3_canvas.Children.Clear();
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = hist_1_3_canvas.ActualHeight * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_3_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = hist_1_3_canvas.ActualHeight;
                    bin_stroke.Y2 = hist_1_3_canvas.ActualHeight - (hist.bins[i] * ratio);
                    hist_1_3_canvas.Children.Add(bin_stroke);
                }
                hist_1_3_label.Content += hist.main_bin.ToString();
            }
            //*/
            //* 2 row
            hist = model.Segments[2].sensor.accelerometer.histogram_x;
            hist.add_value(model.Segments[2].sensor.accelerometer.x);
            if (packet_counter % 40 == 0)
            {

                hist_2_1_label.Content = "";
                hist_2_1_canvas.Children.Clear();
                double height = hist_2_1_canvas.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    hist_2_1_canvas.Children.Add(bin_stroke);
                }
                hist_2_1_label.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[2].sensor.accelerometer.histogram_y;
            hist.add_value(model.Segments[2].sensor.accelerometer.y);
            if (packet_counter % 40 == 0)
            {

                hist_2_2_label.Content = "";
                hist_2_2_canvas.Children.Clear();
                double height = hist_2_2_canvas.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    hist_2_2_canvas.Children.Add(bin_stroke);
                }
                hist_2_2_label.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[2].sensor.accelerometer.histogram_z;
            hist.add_value(model.Segments[2].sensor.accelerometer.z);
            if (packet_counter % 40 == 0)
            {

                hist_2_3_label.Content = "";
                hist_2_3_canvas.Children.Clear();
                double height = hist_2_3_canvas.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    hist_2_3_canvas.Children.Add(bin_stroke);
                }
                hist_2_3_label.Content += hist.main_bin.ToString();
            }
            //*/
            //* 3 row
            hist = model.Segments[3].sensor.accelerometer.histogram_x;
            hist.add_value(model.Segments[3].sensor.accelerometer.x);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_3_1_label;
                lbl.Content = "";
                canv = hist_3_1_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[3].sensor.accelerometer.histogram_y;
            hist.add_value(model.Segments[3].sensor.accelerometer.y);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_3_2_label;
                lbl.Content = "";
                canv = hist_3_2_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[3].sensor.accelerometer.histogram_z;
            hist.add_value(model.Segments[3].sensor.accelerometer.z);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_3_3_label;
                lbl.Content = "";
                canv = hist_3_3_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
            }
            //*/
            //* 4 row
            hist = model.Segments[4].sensor.accelerometer.histogram_x;
            hist.add_value(model.Segments[4].sensor.accelerometer.x);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_4_1_label;
                lbl.Content = "";
                canv = hist_4_1_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[4].sensor.accelerometer.histogram_y;
            hist.add_value(model.Segments[4].sensor.accelerometer.y);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_4_2_label;
                lbl.Content = "";
                canv = hist_4_2_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
            }
            hist = model.Segments[4].sensor.accelerometer.histogram_z;
            hist.add_value(model.Segments[4].sensor.accelerometer.z);
            if (packet_counter % 40 == 0)
            {
                lbl = hist_4_3_label;
                lbl.Content = "";
                canv = hist_4_3_canvas;
                canv.Children.Clear();
                double height = canv.ActualHeight;
                double ratio = 0;
                if (hist.main_bin_value != 0)
                    ratio = height * 0.75 / hist.main_bin_value;
                for (int i = 0; i < hist.bins.Length; i++)
                {
                    //hist_1_1_label.Content += model.Segments[1].sensor.accelerometer.histogram_x.bins[i].ToString() + " ";
                    //hist_1_1_label.Content += hist.bins[i].ToString() + " ";
                    Line bin_stroke;
                    bin_stroke = new Line();
                    bin_stroke.StrokeThickness = 13;
                    bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                    bin_stroke.X1 = 40 + i * 15;
                    bin_stroke.X2 = 40 + i * 15;
                    bin_stroke.Y1 = height;
                    bin_stroke.Y2 = height - (hist.bins[i] * ratio);
                    canv.Children.Add(bin_stroke);
                }
                lbl.Content += hist.main_bin.ToString();
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
