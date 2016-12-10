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
        System.Media.SoundPlayer player = new System.Media.SoundPlayer("Speech_Misrecognition.wav");
        private Thread dataReceivingThread;
        private Thread metronomeThread;
        private Thread[] calculate_segment_threads = new Thread[20];
        metronom_cls metronom = new metronom_cls();

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

        angle_graph_cls angle_chart;

        string debug_string = "no data\n";

        data_storage_cls storage = new data_storage_cls();
        registrator_cls registrator;
        

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

            //metronomeThread = new Thread(new ThreadStart(this.metronome_thread_method));
            //metronomeThread.IsBackground = true;
            stop_metronome_button.IsEnabled = false;
            //metronomeThread.Start();

            angle_chart = new angle_graph_cls(angle_0_graph_canvas);

            registrator = new registrator_cls(storage);

        }

        

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
                        
                        for (int i = 1; i <= 19; i++)
                        {
                            //calculate_segment_threads[i].Abort();
                            //calculate_segment_threads[i] = new Thread(new ThreadStart(model.Segments[i].calculate_segment_position));
                            //calculate_segment_threads[i].IsBackground = true;
                            //calculate_segment_threads[i].Start();

                            model.Segments[i].calculate_segment_position();
                        }

                        (model.Channels.ToArray())[0].Angle.calculate();
                        (model.Channels.ToArray())[1].Angle.calculate();
                        (model.Channels.ToArray())[3].Angle.calculate();

                        // save data
                        if(registrator.registering)
                        {
                            storage.data_push((model.Channels.ToArray())[0].Angle.Angle);
                        }

                        //***************************************************************************
                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(UpdateUserInterface));
                    }
                    catch (Exception e)
                    {
                        debug_string = "udp data read fail!!!  " + e.ToString() + "\n";
                    }

                }

            }

        }// end dataReceivingMethod

        private void UpdateUserInterface()
        {
            

            info_panel_label.Content = debug_string;
            info_panel_label.UpdateLayout();

            /*
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

            (model.Channels.ToArray())[0].Angle.calculate();
            (model.Channels.ToArray())[1].Angle.calculate();
            (model.Channels.ToArray())[3].Angle.calculate();
            //*/
            Double angle1 = (model.Channels.ToArray())[0].Angle.Angle;
            Double angle2 = (model.Channels.ToArray())[1].Angle.Angle;
            Double angle3 = (model.Channels.ToArray())[3].Angle.Angle;

            angle_chart.add_stroke(angle1);

            segment1_axis.Content = String.Format("{0,10:F3}", angle1);
            segment2_axis.Content = String.Format("{0,10:F3}", angle2);
            segment_1_2_angle.Content = String.Format("{0,10:F3}", angle3);
            //********************************************************************

            /*
            double ratio = 0;
            int sensor_type = 0; // 0 - accel, 1 - gyro, 2 - magnet;

            histogram_cls[,] hist = new histogram_cls[5, 4];
            sensor_cls[] sensor = new sensor_cls[5];

            for (int i = 1; i < 5; i++)
            {
                sensor[i] = model.Segments[i].sensors_array[sensor_type];
            }
            

            for (int i = 1; i < 5; i++ )
            {
                for(int j=1; j<4; j++)
                    hist[i, j] = sensor[i].histogram_array[j-1];
            }

            // add value
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                    hist[i, j].add_value(sensor[i].xyz[j-1]);
            }

            Label[,] labels = new Label[5, 4];
            labels[1, 1] = hist_1_1_label; labels[1, 2] = hist_1_2_label; labels[1, 3] = hist_1_3_label;
            labels[2, 1] = hist_2_1_label; labels[2, 2] = hist_2_2_label; labels[2, 3] = hist_2_3_label;
            labels[3, 1] = hist_3_1_label; labels[3, 2] = hist_3_2_label; labels[3, 3] = hist_3_3_label;
            labels[4, 1] = hist_4_1_label; labels[4, 2] = hist_4_2_label; labels[4, 3] = hist_4_3_label;

            Canvas[,] canvases = new Canvas[5, 4];
            canvases[1, 1] = hist_1_1_canvas; canvases[1, 2] = hist_1_2_canvas; canvases[1, 3] = hist_1_3_canvas;
            canvases[2, 1] = hist_2_1_canvas; canvases[2, 2] = hist_2_2_canvas; canvases[2, 3] = hist_2_3_canvas;
            canvases[3, 1] = hist_3_1_canvas; canvases[3, 2] = hist_3_2_canvas; canvases[3, 3] = hist_3_3_canvas;
            canvases[4, 1] = hist_4_1_canvas; canvases[4, 2] = hist_4_2_canvas; canvases[4, 3] = hist_4_3_canvas;

            if (packet_counter % 40 == 0)
            {
                for (int i = 1; i < 5; i++)
                {
                    for (int j = 1; j < 4; j++)
                    {
                        labels[i, j].Content = "";
                        canvases[i, j].Children.Clear();
                        ratio = 0;
                        if (hist[i, j].main_bin_value != 0)
                            ratio = canvases[i, j].ActualHeight * 0.75 / hist[i, j].main_bin_value;
                        for (int k = 0; k < hist[i, j].bins.Length; k++)
                        {
                            Line bin_stroke;
                            bin_stroke = new Line();
                            bin_stroke.StrokeThickness = 13;
                            bin_stroke.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                            bin_stroke.X1 = 40 + k * 15;
                            bin_stroke.X2 = 40 + k * 15;
                            bin_stroke.Y1 = canvases[i, j].ActualHeight;
                            bin_stroke.Y2 = canvases[i, j].ActualHeight - (hist[i, j].bins[k] * ratio);
                            canvases[i, j].Children.Add(bin_stroke);
                            canvases[i, j].UpdateLayout();
                        }
                        labels[i, j].Content += hist[i, j].main_bin.ToString();
                        labels[i, j].UpdateLayout();
                    }
                }
            }
            //*/

        }// end update user interface

        //*
        public void metronome_thread_method()
        {
            while (metronom.metronome_on)
            {
                //if (metronom.metronome_on)
                //{
                Thread.Sleep(metronom.period_ms - metronom.lamp_period_ms);
                metronom.lamp_on = true;
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new NoArgDelegate(metronome_blink));
                Thread.Sleep(metronom.lamp_period_ms);
                metronom.lamp_on = false;
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new NoArgDelegate(metronome_blink));
                //}
            }
        }
        //*/

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
            doJob = false;
            dataReceivingThread.Abort();
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

        private void metronome_blink()
        {
            if (metronom.lamp_on)
            {
                angle_chart.add_metronome_marker_stroke();
                metronome_lamp_label.Background = System.Windows.Media.Brushes.Red;
                if (registrator.registering)
                {
                    registrator.cycles_counter++;
                    cycle_count_label.Content = "Циклы: " + registrator.cycles_counter.ToString();
                    cycle_count_label.UpdateLayout(); 
                }

                player.Play();
            }
            else
                metronome_lamp_label.Background = System.Windows.Media.Brushes.Black;
        }

        private void start_metronome_button_Click(object sender, RoutedEventArgs e)
        {
            int period = 0;
            try
            {
                period = int.Parse(metronome_temp_textbox.Text);
                if (period > 0)
                {
                    period = 60000 / period;
                    metronom.period_ms = period;
                }
            }
            catch(Exception ex)
            { }

            start_metronome_button.IsEnabled = false;
            stop_metronome_button.IsEnabled = true;
            metronom.metronome_on = true;
            //metronomeThread.Start();
            metronomeThread = new Thread(new ThreadStart(this.metronome_thread_method));
            metronomeThread.IsBackground = true;
            metronomeThread.Start();
        }

        private void stop_metronome_button_Click(object sender, RoutedEventArgs e)
        {
            start_metronome_button.IsEnabled = true;
            stop_metronome_button.IsEnabled = false;
            metronom.metronome_on = false;
            metronomeThread.Abort();
            metronome_lamp_label.Background = System.Windows.Media.Brushes.Black;
        }

        private void start_registration_button_Click(object sender, RoutedEventArgs e)
        {
            registrator.start_registering();
        }

        private void stop_registration_button_Click(object sender, RoutedEventArgs e)
        {
            registrator.stop_registering();
        }

    }
}// end namespace kinematics_20160720
