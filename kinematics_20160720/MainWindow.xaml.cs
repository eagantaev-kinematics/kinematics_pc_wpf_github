﻿using System;
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
        raw_kinematics_data_cls raw_data;
        model_cls model;

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

            local_kinematics_endpoint.Address = IPAddress.Parse("192.168.1.1");
            local_kinematics_endpoint.Port = 112;

            kinematics_listener = new UdpClient();
            kinematics_listener.Client.Bind(local_kinematics_endpoint);

            raw_data = new raw_kinematics_data_cls();
            model = new model_cls(raw_data);
            // fill model
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[2]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[1], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[3]));
            model.add_channel(new angle_cls(model.Segments[2], model.Segments[4]));
            model.add_channel(new angle_cls(model.Segments[3], model.Segments[4]));
            
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
            (model.Channels.ToArray())[0].Angle.calculate();
            (model.Channels.ToArray())[1].Angle.calculate();
            (model.Channels.ToArray())[2].Angle.calculate();
            Double angle1 = (model.Channels.ToArray())[0].Angle.Angle;
            Double angle2 = (model.Channels.ToArray())[1].Angle.Angle;
            Double angle3 = (model.Channels.ToArray())[2].Angle.Angle;

            /*
            segment_x_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[1].get_xl()[0], model.Segments[1].get_xl()[1], model.Segments[1].get_xl()[2]);
            segment_y_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[1].get_yl()[0], model.Segments[1].get_yl()[1], model.Segments[1].get_yl()[2]);
            segment_z_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[1].get_zl()[0], model.Segments[1].get_zl()[1], model.Segments[1].get_zl()[2]);
            segment_x_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[2].get_xl()[0], model.Segments[2].get_xl()[1], model.Segments[2].get_xl()[2]);
            segment_y_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[2].get_yl()[0], model.Segments[2].get_yl()[1], model.Segments[2].get_yl()[2]);
            segment_z_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", model.Segments[2].get_zl()[0], model.Segments[2].get_zl()[1], model.Segments[2].get_zl()[2]);

            Double X1 = model.Segments[1].get_X();
            Double X2 = model.Segments[2].get_X();
            Double Y1 = model.Segments[1].get_Y();
            Double Y2 = model.Segments[2].get_Y();
            Double Z1 = model.Segments[1].get_Z();
            Double Z2 = model.Segments[2].get_Z();
            Double n1 = Math.Sqrt(X1*X1 + Y1*Y1 + Z1*Z1);
            Double n2 = Math.Sqrt(X2*X2 + Y2*Y2 + Z2*Z2);
            Double angle_1_2 = Math.Acos((X1*X2 + Y1*Y2 + Z1*Z2)/n1/n2);
            angle_1_2 = angle_1_2 * 180.0 / Math.PI;
            //*/

            segment1_axis.Content = String.Format("{0,10:F3}", angle1);
            segment2_axis.Content = String.Format("{0,10:F3}", angle2);
            segment_1_2_angle.Content = String.Format("{0,10:F3}", angle3);
            //********************************************************************
            
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
            Environment.Exit(0);
        }


    }
}// end namespace kinematics_20160720