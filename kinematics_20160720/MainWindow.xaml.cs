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
        Segment_cls segment_1;

        private Int32 packet_counter = 0;
        private delegate void NoArgDelegate();

        //udp***
        UdpClient kinematics_listener;
        IPEndPoint remote_endpoint = new IPEndPoint(0, 0);

        IPEndPoint local_kinematics_endpoint = new IPEndPoint(0, 0);


        //udp***
        byte[] kinematics_data;

        string debug_string = "no data\n";

        public MainWindow()
        {
            InitializeComponent();

            local_kinematics_endpoint.Address = IPAddress.Parse("192.168.1.1");
            local_kinematics_endpoint.Port = 112;

            kinematics_listener = new UdpClient();
            kinematics_listener.Client.Bind(local_kinematics_endpoint);

            kinematics_data = new byte[342];

            segment_1 = new Segment_cls(1);
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
                        kinematics_data = kinematics_listener.Receive(ref remote_endpoint);
                        packet_counter++;
                        //debug_string = "received - " + kinematics_data.Length + " udp data\n";
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

            
            //kinematics_data = new byte[342];
            //if(true)
            if (kinematics_data.Length == 342)
            {
                data_panel_label.Content = "";
                //data_panel_label.Content += "    гироскоп Х      гироскоп Y      гироскоп Z  акселерометр X  акселерометр Y  акселерометр Z   магнетометр X   магнетометр Y   магнетометр Z\n";
                //data_panel_label.UpdateLayout();

                for(int i=0; i<19; i++)
                {
                    string data_string = "";
                    for(int j=0; j<9; j++)
                    {
                        Int16 data = (Int16)((Int16)kinematics_data[i*18 + j*2] + ((Int16)(kinematics_data[i*18 + j*2 + 1])<<8));
                        //data_string += data.ToString() + "  ";
                        data_string += String.Format("{0, 10}  ", data);
                    }
                    data_string += "\n";
                    data_panel_label.Content += data_string;
                }
            }
            data_panel_label.UpdateLayout();

            segment_x_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_x()[0], segment_1.get_x()[1], segment_1.get_x()[2]);
            segment_y_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_y()[0], segment_1.get_y()[1], segment_1.get_y()[2]);
            segment_z_loc.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_z()[0], segment_1.get_z()[1], segment_1.get_z()[2]);
            segment_x_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_xl()[0], segment_1.get_xl()[1], segment_1.get_xl()[2]);
            segment_y_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_yl()[0], segment_1.get_yl()[1], segment_1.get_yl()[2]);
            segment_z_glob.Content = String.Format("<{0,7:F3}, {1,7:F3}, {2,7:F3}>", segment_1.get_zl()[0], segment_1.get_zl()[1], segment_1.get_zl()[2]);


            segment_1.calculate_segment_position(1, kinematics_data);
            String x_str = segment_1.get_X().ToString();
            if (x_str.Length > 7) x_str = x_str.Substring(0, 7);
            String y_str = segment_1.get_Y().ToString();
            if (y_str.Length > 7) y_str = y_str.Substring(0, 7);
            String z_str = segment_1.get_Z().ToString();
            if (z_str.Length > 7) z_str = z_str.Substring(0, 7);

            String segment_axis_string = "<" + x_str + ", " + y_str + ", " + z_str + ">";
            segment_axis.Content = segment_axis_string;
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
