using System;
using System.Diagnostics;
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

using System.Runtime.InteropServices;

using OxyPlot;
using OxyPlot.Series;



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

        angle_graph_cls angle_chart0, angle_chart1, angle_chart2;
        mean_cycle_graph_cls mean_cycle_chart0, mean_cycle_chart1, mean_cycle_chart2;

        string debug_string = "no data\n";

        data_storage_cls storage0 = new data_storage_cls();
        data_storage_cls storage1 = new data_storage_cls();
        data_storage_cls storage2 = new data_storage_cls();
        public registrator_cls registrator0, registrator1, registrator2;

        System.Windows.Forms.Control userControl_unity3d = new System.Windows.Forms.Control();
        //адрес приложения 
        string unity_game_path = "C:\\workspace\\unity_workspace\\skeleton\\skeleton_00_01\\skeleton_00_01\\skeleton_00_01.exe";
        //string unity_game_path = "C:\\workspace\\unity_workspace\\cmd.exe";
        ProcessStartInfo MyPSI;
        Process unity_game_process;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        chart0_window chart0Window;
        chart1_window chart1Window;
        chart2_window chart2Window;

        PlotModel aux0_plot_model;
        OxyPlot.Series.LineSeries aux0_series;

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

            angle_chart0 = new angle_graph_cls(angle_0_graph_canvas, chart0_legend_label);
            angle_chart1 = new angle_graph_cls(angle_1_graph_canvas, chart1_legend_label);
            angle_chart2 = new angle_graph_cls(angle_2_graph_canvas, chart2_legend_label);
            //mean_cycle_chart0 = new mean_cycle_graph_cls(channel_0_mean_graph_canvas);
            mean_cycle_chart1 = new mean_cycle_graph_cls(channel_1_mean_graph_canvas);
            mean_cycle_chart2 = new mean_cycle_graph_cls(channel_2_mean_graph_canvas);

            registrator0 = new registrator_cls(storage0, metronom);
            registrator1 = new registrator_cls(storage1, metronom);
            registrator2 = new registrator_cls(storage2, metronom);

            //windowsFormsHost.Child = userControl_unity3d;
            //MyPSI = new ProcessStartInfo(unity_game_path);
            //unity_game_process = new Process();
            //unity_game_process.StartInfo = MyPSI;
            //unity_game_process.Start();
            //System.IntPtr handle1 = unity_game_process.MainWindowHandle;
            //System.IntPtr handle2 = userControl_unity3d.Handle;
            //SetParent(handle1, handle2);


            //(windowsFormsHost.Child as System.Windows.Forms.WebBrowser).Navigate("file:///C:/workspace/unity_workspace/skeleton/skeleton_00_01/skeleton_00_01/web_play/web_play.html");

            
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
                        if(registrator0.registering)
                        {
                            storage0.data_push((model.Channels.ToArray())[0].Angle.Angle);
                        }
                        if (registrator1.registering)
                        {
                            storage1.data_push((model.Channels.ToArray())[1].Angle.Angle);
                        }
                        if (registrator2.registering)
                        {
                            storage2.data_push((model.Channels.ToArray())[3].Angle.Angle);
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

            //*
            // zapolnenie tablicy dannyh datchikov
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
            
            //*/
            Double angle1 = (model.Channels.ToArray())[0].Angle.Angle;
            Double angle2 = (model.Channels.ToArray())[1].Angle.Angle;
            Double angle3 = (model.Channels.ToArray())[3].Angle.Angle;

            angle_chart0.add_stroke(angle1);
            angle_chart1.add_stroke(angle2);
            angle_chart2.add_stroke(angle3);

            segment1_axis.Content = String.Format("{0,10:F3}", angle1);
            segment2_axis.Content = String.Format("{0,10:F3}", angle2);
            segment_1_2_angle.Content = String.Format("{0,10:F3}", angle3);
            //********************************************************************

            //*
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

            test_results_panel.Content += histogram.bubble_sort_test().ToString() + " histogram.bubble_sort_test \r\n --> ";
            test_results_panel.Content += histogram.mean_calculation_test().ToString() + " mean_calculation_test \r\n --> ";
            test_results_panel.Content += histogram.sigma_calculation_test().ToString() + " sigma_calculation_test \r\n --> ";
            test_results_panel.Content += histogram.bins_calculation_test().ToString() + " bins_calculation_test \r\n --> ";
            test_results_panel.Content += registrator0.bubble_sorting_test().ToString() + " registrator0.bubble_sorting_test \r\n --> ";
        }

        private void metronome_blink()
        {
            if (metronom.lamp_on)
            {
                angle_chart0.add_metronome_marker_stroke();
                angle_chart1.add_metronome_marker_stroke();
                angle_chart2.add_metronome_marker_stroke();
                metronome_lamp_label.Background = System.Windows.Media.Brushes.Red;
                if (registrator0.registering)
                {
                    registrator0.cycles_counter++;
                    cycle_count_label.Content = "Циклы: " + registrator0.cycles_counter.ToString();
                    cycle_count_label.UpdateLayout();
                    storage0.cycle_delimiter_push();
                }
                if (registrator1.registering)
                {
                    registrator1.cycles_counter++;
                    cycle_count_label.Content = "Циклы: " + registrator1.cycles_counter.ToString();
                    cycle_count_label.UpdateLayout();
                    storage1.cycle_delimiter_push();
                }
                if (registrator2.registering)
                {
                    registrator2.cycles_counter++;
                    cycle_count_label.Content = "Циклы: " + registrator2.cycles_counter.ToString();
                    cycle_count_label.UpdateLayout();
                    storage2.cycle_delimiter_push();
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
            registrator0.start_registering();
            registrator1.start_registering();
            registrator2.start_registering();
        }

        private void stop_registration_button_Click(object sender, RoutedEventArgs e)
        {
            //mean_cycle_chart0.reset_graph();
            registrator0.stop_registering();
            //mean_cycle_chart0.calculate_horizontal_step(registrator0.base_length_value);
            aux0_plot_model = new PlotModel();

            
            // draw charts of elementary cycles
            int counter = 0;
            LineSeries[] graphs = new LineSeries[registrator0.list_of_cycles.Count];
            foreach (registrator_cls.single_cycle_cls item in registrator0.list_of_cycles)
            {
                //mean_cycle_chart0.rewind_graph();
                graphs[counter] = new LineSeries();
                graphs[counter].Color = OxyColor.FromRgb(0, 255, 0);
                if (Math.Abs(item.length - registrator0.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator0.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator0.Base_length_value) && (i >= item.length))
                            value = registrator0.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator0.Storage.get_data(item.start_index + i);
                        //mean_cycle_chart0.add_stroke(value, 0);
                        graphs[counter].Points.Add(new DataPoint(i * 0.025, value));
                    }  
                }
                aux0_plot_model.Series.Add(graphs[counter]);
                counter++;
                
            }
            channel0_raw_plot_view.Model = aux0_plot_model;
            channel0_raw_plot_view.UpdateLayout();
            // draw charts of smoothed elementary cycles
            counter = 0;
            LineSeries[] smoothed_graphs = new LineSeries[registrator0.list_of_cycles.Count];
            foreach (registrator_cls.single_cycle_cls item in registrator0.list_of_cycles)
            {
                //mean_cycle_chart0.rewind_graph();
                smoothed_graphs[counter] = new LineSeries();
                smoothed_graphs[counter].Color = OxyColor.FromRgb(0, 0, 0);
                if (Math.Abs(item.length - registrator0.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator0.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator0.Base_length_value) && (i >= item.length))
                            value = registrator0.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator0.Storage.get_smoothed_data(item.start_index + i);
                        //mean_cycle_chart0.add_stroke(value, 4);
                        smoothed_graphs[counter].Points.Add(new DataPoint(i * 0.025, value));
                    }
                }
                aux0_plot_model.Series.Add(smoothed_graphs[counter]);
                counter++;
            }
            channel0_raw_plot_view.Model = aux0_plot_model;
            channel0_raw_plot_view.UpdateLayout();
            // draw a mean cycle chart
            //mean_cycle_chart0.rewind_graph();
            for (int i = 0; i < registrator0.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator0.get_mean_cycle_data(i);
                if (!Double.IsNaN(value))                   ;
                    //mean_cycle_chart0.add_stroke(value, 2);
            }
            // draw a filtered mean cycle chart
            //mean_cycle_chart0.rewind_graph();
            for (int i = 0; i < registrator0.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator0.get_filtered_mean_cycle_data(i);
                if (!Double.IsNaN(value))                   ;
                    //mean_cycle_chart0.add_stroke(value, 3);
            }

            // draw a smoothed mean cycle chart
            //mean_cycle_chart0.rewind_graph();
            for (int i = 0; i < registrator0.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator0.get_smoothed_cycle_data(i);
                if (!Double.IsNaN(value))                   ;
                    //mean_cycle_chart0.add_stroke(value, 1);
            }


            mean_cycle_chart1.reset_graph();
            registrator1.stop_registering();
            mean_cycle_chart1.calculate_horizontal_step(registrator1.base_length_value);
            
            // draw charts of elementary cycles
            foreach (registrator_cls.single_cycle_cls item in registrator1.list_of_cycles)
            {
                mean_cycle_chart1.rewind_graph();
                if (Math.Abs(item.length - registrator1.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator1.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator1.Base_length_value) && (i >= item.length))
                            value = registrator1.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator1.Storage.get_data(item.start_index + i);
                        mean_cycle_chart1.add_stroke(value, 0);
                    }
                }
            }
            // draw charts of smoothed elementary cycles
            foreach (registrator_cls.single_cycle_cls item in registrator1.list_of_cycles)
            {
                mean_cycle_chart1.rewind_graph();
                if (Math.Abs(item.length - registrator1.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator1.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator1.Base_length_value) && (i >= item.length))
                            value = registrator1.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator1.Storage.get_smoothed_data(item.start_index + i);
                        mean_cycle_chart1.add_stroke(value, 4);
                    }
                }
            }
            // draw a mean cycle chart
            mean_cycle_chart1.rewind_graph();
            for (int i = 0; i < registrator1.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator1.get_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart1.add_stroke(value, 2);
            }
            // draw a filtered mean cycle chart
            mean_cycle_chart1.rewind_graph();
            for (int i = 0; i < registrator1.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator1.get_filtered_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart1.add_stroke(value, 3);
            }
            // draw a smoothed mean cycle chart
            mean_cycle_chart1.rewind_graph();
            for (int i = 0; i < registrator1.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator1.get_smoothed_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart1.add_stroke(value, 1);
            }

            mean_cycle_chart2.reset_graph();
            registrator2.stop_registering();
            mean_cycle_chart2.calculate_horizontal_step(registrator2.base_length_value);
            
            // draw charts of elementary cycles
            foreach (registrator_cls.single_cycle_cls item in registrator2.list_of_cycles)
            {
                mean_cycle_chart2.rewind_graph();
                if (Math.Abs(item.length - registrator2.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator2.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator2.Base_length_value) && (i >= item.length))
                            value = registrator2.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator2.Storage.get_data(item.start_index + i);
                        mean_cycle_chart2.add_stroke(value, 0);
                    }
                }
            }
            // draw charts of smoothed elementary cycles
            foreach (registrator_cls.single_cycle_cls item in registrator2.list_of_cycles)
            {
                mean_cycle_chart2.rewind_graph();
                if (Math.Abs(item.length - registrator2.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator2.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator2.Base_length_value) && (i >= item.length))
                            value = registrator2.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator2.Storage.get_smoothed_data(item.start_index + i);
                        mean_cycle_chart2.add_stroke(value, 4);
                    }
                }
            }
            // draw a mean cycle chart
            mean_cycle_chart2.rewind_graph();
            for (int i = 0; i < registrator2.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator2.get_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart2.add_stroke(value, 2);
            }
            // draw a filtered mean cycle chart
            mean_cycle_chart2.rewind_graph();
            for (int i = 0; i < registrator2.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator2.get_filtered_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart2.add_stroke(value, 3);
            }
            // draw a smoothed mean cycle chart
            mean_cycle_chart2.rewind_graph();
            for (int i = 0; i < registrator2.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                double value = registrator2.get_smoothed_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_cycle_chart2.add_stroke(value, 1);
                else
                {
                    int x = 4;
                }
                    
            }

            


        }//end private void stop_registration_button_Click(object sender, RoutedEventArgs e)

        private void wbWinForms_DocumentTitleChanged(object sender, EventArgs e)
        {
                this.Title = (sender as System.Windows.Forms.WebBrowser).DocumentTitle;
        }

        private void on_channel0_mean_chart_mouse_left_up(object sender, MouseButtonEventArgs e)
        {
            chart0Window = new chart0_window();
            //chart0Window.Owner = this;

            OxyPlot.Series.LineSeries series0 = new LineSeries();
            series0.Color = OxyColor.FromRgb(255, 0, 0);

            if(registrator0.smoothed_cycle_buffer != null)
            {
                int length = registrator0.smoothed_cycle_buffer.Length;
                for (int i = 0; i < length; i++ )
                {
                    if(!Double.IsNaN(registrator0.smoothed_cycle_buffer[i]))
                    {
                        series0.Points.Add(new DataPoint(i * 0.025, registrator0.smoothed_cycle_buffer[i]));
                    }
                }
            }

            chart0Window.channel0_mean_plot_view.Model.Series.Add(series0);
            chart0Window.Show();
        }

        private void on_channel1_mean_chart_mouse_left_up(object sender, MouseButtonEventArgs e)
        {
            chart1Window = new chart1_window();
            //chart0Window.Owner = this;

            OxyPlot.Series.LineSeries series0 = new LineSeries();
            series0.Color = OxyColor.FromRgb(255, 0, 0);

            if (registrator1.smoothed_cycle_buffer != null)
            {
                int length = registrator1.smoothed_cycle_buffer.Length;
                for (int i = 0; i < length; i++)
                {
                    if (!Double.IsNaN(registrator1.smoothed_cycle_buffer[i]))
                    {
                        series0.Points.Add(new DataPoint(i * 0.025, registrator1.smoothed_cycle_buffer[i]));
                    }
                }
            }

            chart1Window.channel1_mean_plot_view.Model.Series.Add(series0);
            chart1Window.Show();
        }

        private void on_channel2_mean_chart_mouse_left_up(object sender, MouseButtonEventArgs e)
        {
            chart2Window = new chart2_window();
            //chart0Window.Owner = this;

            OxyPlot.Series.LineSeries series0 = new LineSeries();
            series0.Color = OxyColor.FromRgb(255, 0, 0);

            if (registrator2.smoothed_cycle_buffer != null)
            {
                int length = registrator2.smoothed_cycle_buffer.Length;
                for (int i = 0; i < length; i++)
                {
                    if (!Double.IsNaN(registrator2.smoothed_cycle_buffer[i]))
                    {
                        series0.Points.Add(new DataPoint(i * 0.025, registrator2.smoothed_cycle_buffer[i]));
                    }
                }
            }

            chart2Window.channel2_mean_plot_view.Model.Series.Add(series0);
            chart2Window.Show();
        }

    }//end public partial class MainWindow : Window
    //************************************************************************************************
}// end namespace kinematics_20160720
