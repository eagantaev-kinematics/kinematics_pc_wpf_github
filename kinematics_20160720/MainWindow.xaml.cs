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
    /// 


    public delegate void NoArgDelegate();
    delegate void debug_log(string message);


    public partial class MainWindow : Window
    {
        // variables ***************************
        OxyPlot.Wpf.PlotView[] plotviews;
        int active_plotview_index = 0;
        int cycle_counter = 0;

        string file_prefix;
        StreamWriter mean_cycle_out;

        int data_result_switch = 0;

        // objects ***************************
        metronome_cls metronome;
        udp_receiver_cls udp_receiver;
        raw_data_storage_cls raw_data_storage;
        skeleton_cls skeleton;

        //                                        ********************
        //                                         PUBLIC CONSTRUCTOR
        //                                        ********************
        public MainWindow()
        {
            InitializeComponent();

            // objects *****************************
            debug_log debug_log_append = new debug_log(debug_pannel_add_string);
            udp_receiver = new udp_receiver_cls(debug_log_append);
            metronome = new metronome_cls();
            joint_chart_pannel = new joint_chart_pannel_cls(main_joint_angle_timeline_plot_view, main_joint_angle_plot_view, frontal_projection_plot_view,
                                                            sagittal_projection_plot_view, horizontal_projection_plot_view);
            raw_data_storage = new raw_data_storage_cls(udp_receiver);
            skeleton = new skeleton_cls(raw_data_storage);


            /*
            raw_data = new raw_kinematics_data_cls();
            model = new model_cls(raw_data);
            // fill model
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[2]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[3], model.Segments[4]));
            */

            histogram = new histogram_cls(160, 13, 40);  // object just to run tests

            //metronomeThread = new Thread(new ThreadStart(this.metronome_thread_method));
            //metronomeThread.IsBackground = true;
            stop_metronome_button.IsEnabled = false;
            //metronomeThread.Start();

            //angle_chart0 = new angle_graph_cls(angle_0_graph_canvas, chart0_legend_label);
            //angle_chart1 = new angle_graph_cls(angle_1_graph_canvas, chart1_legend_label);
            //angle_chart2 = new angle_graph_cls(angle_2_graph_canvas, chart2_legend_label);
            //mean_cycle_chart0 = new mean_cycle_graph_cls(channel_0_mean_graph_canvas);
            //mean_cycle_chart1 = new mean_cycle_graph_cls(channel_1_mean_graph_canvas);
            //mean_cycle_chart2 = new mean_cycle_graph_cls(channel_2_mean_graph_canvas);

            registrator0 = new registrator_cls(storage0, metronome);
            //registrator1 = new registrator_cls(storage1, metronome);
            //registrator2 = new registrator_cls(storage2, metronome);

            //windowsFormsHost.Child = userControl_unity3d;
            //MyPSI = new ProcessStartInfo(unity_game_path);
            //unity_game_process = new Process();
            //unity_game_process.StartInfo = MyPSI;
            //unity_game_process.Start();
            //System.IntPtr handle1 = unity_game_process.MainWindowHandle;
            //System.IntPtr handle2 = userControl_unity3d.Handle;
            //SetParent(handle1, handle2);


            //(windowsFormsHost.Child as System.Windows.Forms.WebBrowser).Navigate("file:///C:/workspace/unity_workspace/skeleton/skeleton_00_01/skeleton_00_01/web_play/web_play.html");

            //subscribe on metronome events
            metronome.Metronome_tick += on_metronome_tick;
            metronome.Metronome_master_tick += on_metronome_master_tick;
            metronome.Lamp_on += on_metronome_lamp_on;
            metronome.Lamp_off += on_metronome_lamp_off;

            //subscribe on udp receiver event
            udp_receiver.udp_data_received_event += on_udp_data_received;

            // subscribe on joint chart pannel events
            joint_chart_pannel.chart_update_tick += on_chart_update_tick;



            // threads *****************************
            chart_update_thread = new Thread(new ThreadStart(joint_chart_pannel.chart_update_thread_method));
            chart_update_thread.Start();

            plotviews = new OxyPlot.Wpf.PlotView[4];
            plotviews[0] = main_joint_angle_plot_view;
            plotviews[1] = frontal_projection_plot_view;
            plotviews[2] = sagittal_projection_plot_view;
            plotviews[3] = horizontal_projection_plot_view;

            

        }// end constructor
        //***********************************************************************************************************************************


        //                                        ********************
        //                                               EVENTS
        //                                        ********************
        // Define what actions to take when the event is raised.
        void on_metronome_tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(metronome_tick_processing));
        }
        void metronome_tick_processing()
        {
            chery1_wav.Play();

        }
        void on_metronome_master_tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(metronome_master_tick_processing));
        }
        void metronome_master_tick_processing()
        {
            
            CrashTom_wav.Play();

            foreach(joint_cls joint in skeleton.joints )
            {
                joint.metronome_master_tick_reset();
            }

            // if registering now
            if(skeleton.joints.ToArray()[0].registering_flag)
            {
                cycle_counter++;
                cycle_count_label1.Content = "Циклы: " + cycle_counter.ToString();
                if (cycle_counter >= 20)
                    stop_registration_button_Click(null, null);
            }
        }

        //*
        void on_metronome_lamp_on(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(switch_metronome_lamp_on));
            
        }
        //*/

        void switch_metronome_lamp_on()
        {
            metronome_lamp_label1.Background = System.Windows.Media.Brushes.Red;
            
        }

        
        
        void on_metronome_lamp_off(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(switch_metronome_lamp_off));
        }
        void switch_metronome_lamp_off()
        {
            metronome_lamp_label1.Background = System.Windows.Media.Brushes.Black;
        }

        System.Media.SoundPlayer player = new System.Media.SoundPlayer("Speech_Misrecognition.wav");
        System.Media.SoundPlayer chery1_wav = new System.Media.SoundPlayer("CHERY1.wav");
        System.Media.SoundPlayer CrashTom_wav = new System.Media.SoundPlayer("CrashTom.wav");

        void on_udp_data_received(object sender, EventArgs e)
        {
            // add frame to frame storage 
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(raw_data_storage.add_new_frame)); 
            // update user interface
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(UpdateUserInterface));
            // poulate segments data
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new NoArgDelegate(skeleton.update_data));
        }



        void on_chart_update_tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                           new NoArgDelegate(add_point_to_main_timeline_chart));
        }

        Double time = 0;
        void add_point_to_main_timeline_chart()
        {
            if (data_result_switch == 0)    // data
            {
                LineSeries series = (LineSeries)(plotviews[active_plotview_index].Model.Series.ToArray()[0]);

                series.Points.Add(new DataPoint(time, skeleton.joints.ToArray()[skeleton.active_joint_index].angles[skeleton.active_angle_index]));
                time += 0.025;
                if (time >= 10.0)
                {
                    time = 0;
                    series.Points.Clear();
                }


                plotviews[active_plotview_index].Model.Series.ToArray()[0] = series;

                plotviews[active_plotview_index].InvalidatePlot(); 
            }

        }

        //********************************** working threads **********************************
        private Thread dataReceivingThread;
        private Thread metronomeThread;
        private Thread[] calculate_segment_threads = new Thread[20];
        private Thread chart_update_thread;
        //********************************** working threads **********************************

        // *****
        raw_kinematics_data_cls raw_data;
        model_cls model;

        //***** calibration
        histogram_cls histogram;

        private Int32 packet_counter = 0;
        
        void debug_pannel_add_string(String message)
        {
            debug_info_panel.Content += message;
        }

        //udp***
        //UdpClient kinematics_listener;
        //IPEndPoint remote_endpoint = new IPEndPoint(0, 0);

        //IPEndPoint local_kinematics_endpoint = new IPEndPoint(0, 0);

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
        //chart2_window chart2Window;
        //chart3_window chart3Window;

        PlotModel aux0_plot_model, aux1_plot_model, aux2_plot_model;
        OxyPlot.Series.LineSeries aux0_series;

        joint_chart_pannel_cls joint_chart_pannel;



        /*
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
        */

        private void UpdateUserInterface()
        {

            info_panel_label.Content = udp_receiver.debug_string;
            //info_panel_label.UpdateLayout();

            

            
            // zapolnenie tablicy dannyh datchikov
            if (!(raw_data_storage.frame.frame_empty_flag))
            {
                data_panel_label.Content = "";
                for(int i=0; i<19; i++)
                {
                    string data_string = "";
                    for(int j=0; j<9; j++)
                    {
                        //Int16 data = (Int16)((Int16)raw_data.Kinematics_Data[i * 18 + j * 2] + ((Int16)(raw_data.Kinematics_Data[i * 18 + j * 2 + 1]) << 8));
                        Int16 data = raw_data_storage.frame.get_frame_data(i * 9 + j);
                        data_string += String.Format("{0, 10}  ", data);
                    }
                    data_string += "\n";
                    data_panel_label.Content += data_string;
                }
            }
            data_panel_label.UpdateLayout();

            /*
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
        
        //*/

        private void start_button_Click(object sender, RoutedEventArgs e)
        {
            start_button.Content = "Started";
            start_button.IsEnabled = false;

            stop_button.IsEnabled = true;

            dataReceivingThread = new Thread(new ThreadStart(udp_receiver.data_receiving_method));
            dataReceivingThread.IsBackground = true;
            dataReceivingThread.Start();

        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            start_button.Content = "Start";
            start_button.IsEnabled = true;
            stop_button.IsEnabled = false;
            udp_receiver.data_receiving_thread_stop();
            //dataReceivingThread.Abort();
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

        /*
        private void metronome_blink()
        {
            if (metronome.lamp_on)
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
        */

        private void start_metronome_button_Click(object sender, RoutedEventArgs e)
        {
            int tick_length = 1000;
            try
            {
                tick_length = int.Parse(tick_length_textbox.Text);
            }
            catch(Exception ex)
            { }
            int ticks_in_cycle = 2;
            try
            {
                ticks_in_cycle = int.Parse(ticks_in_cycle_textbox.Text);
            }
            catch (Exception ex)
            { }

            // raschet osei grafikov proekciy
            //*********************************************
            {
                int cycle_length = ticks_in_cycle * tick_length;

            }
            //*********************************************

            start_metronome_button1.IsEnabled = false;
            stop_metronome_button1.IsEnabled = true;
            metronome.tick_length = tick_length;
            metronome.ticks_in_cycle = ticks_in_cycle;
            metronome.period_ms = tick_length * ticks_in_cycle;
            metronome.metronome_on = true;
            //metronomeThread.Start();
            metronomeThread = new Thread(new ThreadStart(metronome.metronome_thread_method));
            metronomeThread.IsBackground = true;
            metronomeThread.Start();

            start_registration_button1.IsEnabled = true;
        }

        private void stop_metronome_button_Click(object sender, RoutedEventArgs e)
        {
            start_metronome_button1.IsEnabled = true;
            stop_metronome_button1.IsEnabled = false;
            metronome.metronome_on = false;
            metronomeThread.Abort();
            metronome_lamp_label.Background = System.Windows.Media.Brushes.Black;

            // stop registering (if registering)
            //stop_registration_button_Click(null, null);
        }

        private void start_registration_button_Click(object sender, RoutedEventArgs e)
        {
            //registrator0.start_registering();
            //registrator1.start_registering();
            //registrator2.start_registering();

            start_registration_button1.IsEnabled = false;
            stop_registration_button1.IsEnabled = true;

            foreach(joint_cls joint in skeleton.joints )
            {
                joint.start_register(metronome);
            }
        }

        private void stop_registration_button_Click(object sender, RoutedEventArgs e)
        {
            /*
            registrator0.stop_registering();
            
            aux0_plot_model = new PlotModel();
            draw_charts_of_elementary_cycles(registrator0, channel0_raw_plot_view, aux0_plot_model);
            draw_charts_of_smoothed_elementary_cycles(registrator0, channel0_raw_plot_view, aux0_plot_model);
            draw_mean_cycle_chart(registrator0, channel0_raw_plot_view, aux0_plot_model);
            draw_filtered_mean_cycle_chart(registrator0, channel0_raw_plot_view, aux0_plot_model);
            draw_smoothed_mean_cycle_chart(registrator0, channel0_raw_plot_view, aux0_plot_model);

            registrator1.stop_registering();

            aux1_plot_model = new PlotModel();
            draw_charts_of_elementary_cycles(registrator1, channel1_raw_plot_view, aux1_plot_model);
            draw_charts_of_smoothed_elementary_cycles(registrator1, channel1_raw_plot_view, aux1_plot_model);
            draw_mean_cycle_chart(registrator1, channel1_raw_plot_view, aux1_plot_model);
            draw_filtered_mean_cycle_chart(registrator1, channel1_raw_plot_view, aux1_plot_model);
            draw_smoothed_mean_cycle_chart(registrator1, channel1_raw_plot_view, aux1_plot_model);

            registrator2.stop_registering();

            aux2_plot_model = new PlotModel();
            draw_charts_of_elementary_cycles(registrator2, channel2_raw_plot_view, aux2_plot_model);
            draw_charts_of_smoothed_elementary_cycles(registrator2, channel2_raw_plot_view, aux2_plot_model);
            draw_mean_cycle_chart(registrator2, channel2_raw_plot_view, aux2_plot_model);
            draw_filtered_mean_cycle_chart(registrator2, channel2_raw_plot_view, aux2_plot_model);
            draw_smoothed_mean_cycle_chart(registrator2, channel2_raw_plot_view, aux2_plot_model);
            */

            stop_registration_button1.IsEnabled = false;
            start_registration_button1.IsEnabled = true;

            foreach (joint_cls joint in skeleton.joints)
            {
                joint.stop_register();
            }

            cycle_counter = 0;

            // save data in file
            file_prefix = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__");
            mean_cycle_out = new StreamWriter(@"c:\temp\" + file_prefix + "kinematics.txt", true);

            foreach (joint_cls joint in skeleton.joints)
            {
                mean_cycle_out.WriteLine(joint.name);
                mean_cycle_out.WriteLine("main frontal sagittal horizontal");
                for(int i=0; i<joint.mean_cycle_length; i++)
                {
                    mean_cycle_out.WriteLine(joint.mean_cycle[0, i].ToString() + " " + joint.mean_cycle[1, i].ToString() + " " + joint.mean_cycle[2, i].ToString() + " " + joint.mean_cycle[3, i].ToString()); 
                }
            }
            mean_cycle_out.Close();

            data_result_switch = 1;
            data_result_button.Content = "Результат";
            show_result();

            //double x = skeleton.joints.ToArray()[0].mean_cycle[0, 0];


        }//end private void stop_registration_button_Click(object sender, RoutedEventArgs e)

        //**********************************************************************************************************************************

        private void draw_charts_of_elementary_cycles(registrator_cls registrator, OxyPlot.Wpf.PlotView plotview, PlotModel aux_plotmodel)
        {
            int counter = 0;
            LineSeries[] graphs = new LineSeries[registrator.list_of_cycles.Count];
            foreach (registrator_cls.single_cycle_cls item in registrator.list_of_cycles)
            {
                //mean_cycle_chart0.rewind_graph();
                graphs[counter] = new LineSeries();
                graphs[counter].Color = OxyColor.FromRgb(0, 255, 0);
                if (Math.Abs(item.length - registrator.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator.Base_length_value) && (i >= item.length))
                            value = registrator.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator.Storage.get_data(item.start_index + i);
                        //mean_cycle_chart0.add_stroke(value, 0);
                        graphs[counter].Points.Add(new DataPoint(i * 0.025, value));
                    }
                    aux_plotmodel.Series.Add(graphs[counter]);
                    counter++;
                }
                

            }
            //plotview.Model = aux_plotmodel;
            //plotview.UpdateLayout();
        }


        private void draw_charts_of_smoothed_elementary_cycles(registrator_cls registrator, OxyPlot.Wpf.PlotView plotview, PlotModel aux_plotmodel)
        {
            int counter = 0;
            LineSeries[] smoothed_graphs = new LineSeries[registrator.list_of_cycles.Count];
            foreach (registrator_cls.single_cycle_cls item in registrator.list_of_cycles)
            {
                smoothed_graphs[counter] = new LineSeries();
                smoothed_graphs[counter].Color = OxyColor.FromRgb(0, 0, 0);
                if (Math.Abs(item.length - registrator.Base_length_value) <= 1)
                {
                    for (int i = 0; i < registrator.base_length_value; i++)
                    {
                        // draw stroke of cycle
                        double value;
                        if ((item.length < registrator.Base_length_value) && (i >= item.length))
                            value = registrator.Storage.get_data(item.start_index + item.length - 1);
                        else
                            value = registrator.Storage.get_smoothed_data(item.start_index + i);
                        smoothed_graphs[counter].Points.Add(new DataPoint(i * 0.025, value));
                    }
                    aux_plotmodel.Series.Add(smoothed_graphs[counter]);
                    counter++;
                }
                
            }
            //plotview.Model = aux_plotmodel;
            //plotview.UpdateLayout();
        }

        void draw_mean_cycle_chart(registrator_cls registrator, OxyPlot.Wpf.PlotView plotview, PlotModel aux_plotmodel)
        {
            LineSeries mean_graph = new LineSeries();
            mean_graph.Color = OxyColor.FromRgb(0, 0, 255);
            double value, old_value;

            old_value = registrator.get_mean_cycle_data(0);
            for (int i = 0; i < registrator.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                value = registrator.get_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_graph.Points.Add(new DataPoint(i * 0.025, value));
                else
                    mean_graph.Points.Add(new DataPoint(i * 0.025, old_value));
            }
            aux_plotmodel.Series.Add(mean_graph);
            //plotview.Model = aux_plotmodel;
            //plotview.UpdateLayout();
        }

        void draw_filtered_mean_cycle_chart(registrator_cls registrator, OxyPlot.Wpf.PlotView plotview, PlotModel aux_plotmodel)
        {
            LineSeries mean_graph = new LineSeries();
            mean_graph.Color = OxyColor.FromRgb(255, 255, 0);
            double value, old_value;

            old_value = registrator.get_filtered_mean_cycle_data(0);
            for (int i = 0; i < registrator.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                value = registrator.get_filtered_mean_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_graph.Points.Add(new DataPoint(i * 0.025, value));
                else
                    mean_graph.Points.Add(new DataPoint(i * 0.025, old_value));
            }
            aux_plotmodel.Series.Add(mean_graph);
            //plotview.Model = aux_plotmodel;
            //plotview.UpdateLayout();
        }

        void draw_smoothed_mean_cycle_chart(registrator_cls registrator, OxyPlot.Wpf.PlotView plotview, PlotModel aux_plotmodel)
        {
            LineSeries mean_graph = new LineSeries();
            mean_graph.Color = OxyColor.FromRgb(255, 0, 0);
            double value, old_value;

            old_value = registrator.get_filtered_mean_cycle_data(0);
            for (int i = 0; i < registrator.base_length_value; i++)
            {
                // draw stroke of mean cycle graph
                value = registrator.get_smoothed_cycle_data(i);
                if (!Double.IsNaN(value))
                    mean_graph.Points.Add(new DataPoint(i * 0.025, value));
                else
                    mean_graph.Points.Add(new DataPoint(i * 0.025, old_value));
            }
            aux_plotmodel.Series.Add(mean_graph);
            plotview.Model = aux_plotmodel;
            plotview.UpdateLayout();
        }


        //**********************************************************************************************************************************

        private void wbWinForms_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Title = (sender as System.Windows.Forms.WebBrowser).DocumentTitle;
        }

        void clear_scene()
        {
            LineSeries series = (LineSeries)(main_joint_angle_plot_view.Model.Series.ToArray()[0]);
            series.Points.Clear();
            main_joint_angle_plot_view.InvalidatePlot();
            series = (LineSeries)(frontal_projection_plot_view.Model.Series.ToArray()[0]);
            series.Points.Clear();
            frontal_projection_plot_view.InvalidatePlot();
            series = (LineSeries)(sagittal_projection_plot_view.Model.Series.ToArray()[0]);
            series.Points.Clear();
            sagittal_projection_plot_view.InvalidatePlot();
            series = (LineSeries)(horizontal_projection_plot_view.Model.Series.ToArray()[0]);
            series.Points.Clear();
            horizontal_projection_plot_view.InvalidatePlot();
            time = 0;
        }

        private void mean_graph0_double_click(object sender, MouseButtonEventArgs e)
        {
            clear_scene();

            active_plotview_index = 0;
            skeleton.active_angle_index = 0;
        }

        private void mean_graph1_double_click(object sender, MouseButtonEventArgs e)
        {
            clear_scene();

            active_plotview_index = 1;
            skeleton.active_angle_index = 1;
        }

        //*
        private void mean_graph2_double_click(object sender, MouseButtonEventArgs e)
        {
            clear_scene();

            active_plotview_index = 2;
            skeleton.active_angle_index = 2;
        }

        private void mean_graph3_double_click(object sender, MouseButtonEventArgs e)
        {
            clear_scene();

            active_plotview_index = 3;
            skeleton.active_angle_index = 3;
        }
        //*/

        private void main_joint_angle_timeline_double_click(object sender, MouseButtonEventArgs e)
        {

        }


        //                                        ********************
        //                                            JOINT BUTTONS
        //                                        ********************


        private void joint1_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 0;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[0].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint2_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 1;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[1].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint3_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 2;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[2].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint4_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 3;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[3].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint5_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 4;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[4].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint6_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 5;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[5].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint7_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 6;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[6].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint8_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 7;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[7].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint9_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 8;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[8].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint10_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 9;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[9].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint11_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 10;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[10].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint12_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 11;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[11].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint13_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 12;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[12].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint14_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 13;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[13].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint15_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 14;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[14].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint16_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 15;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[15].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint17_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 16;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[16].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void joint18_button_Click(object sender, RoutedEventArgs e)
        {
            skeleton.active_joint_index = 17;
            main_joint_angle_timeline_plot_view.Model.Title = skeleton.joints.ToArray()[17].name;
            main_joint_angle_timeline_plot_view.Model.InvalidatePlot(true);
        }

        private void data_result_button_Click(object sender, RoutedEventArgs e)
        {
            if (data_result_switch == 0) // data
            {
                data_result_switch = 1; // result
                data_result_button.Content = "Результат";

                show_result();
            }
            else // result
            {
                data_result_switch = 0; // data
                plotviews[0].Model.Axes.ToArray()[0].Minimum = 0;
                plotviews[0].Model.Axes.ToArray()[0].Maximum = 10;
                plotviews[0].Model.Axes.ToArray()[1].Minimum = 0;
                plotviews[0].Model.Axes.ToArray()[1].Maximum = 180;
                ((LineSeries)(plotviews[0].Model.Series.ToArray()[0])).Points.Clear();
                plotviews[0].InvalidatePlot();

            }
        }

        void show_result()
        {
            if (skeleton.joints.ToArray()[0].mean_cycle.Length > 0)
            {
                LineSeries series = (LineSeries)(plotviews[0].Model.Series.ToArray()[0]);
                series.Points.Clear();
                plotviews[0].InvalidatePlot();
                for (int i = 0; i < skeleton.joints.ToArray()[0].mean_cycle_length; i++)
                {
                    series.Points.Add(new DataPoint(i * 0.025, skeleton.joints.ToArray()[skeleton.active_joint_index].mean_cycle[0, i]));
                }
                plotviews[0].InvalidatePlot();
            }
            
        }

    }//end public partial class MainWindow : Window
    //************************************************************************************************
}// end namespace kinematics_20160720
