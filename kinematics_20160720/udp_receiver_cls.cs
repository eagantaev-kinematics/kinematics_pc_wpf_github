using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace kinematics_20160720
{
    class udp_receiver_cls
    {
        private int DATA_PACKAGE_LENGTH = 19 * 9 * 2;   // 19 sensors 9 axises 2 byte values

        EventArgs ev_args;
        public event EventHandler udp_data_received_event;

        raw_data_frame_cls Raw_data_frame = new raw_data_frame_cls();
        public raw_data_frame_cls raw_data_frame
        {
            get { return Raw_data_frame; }
        }

        byte[] Data_package;
        public byte[] data_package
        {
            get { return Data_package; }
        }

        String Debug_string;
        public String debug_string
        {
            get { return Debug_string; }
        }

        private Boolean do_job = false;

        //udp***
        UdpClient kinematics_listener;
        IPEndPoint remote_endpoint = new IPEndPoint(0, 0);
        IPEndPoint local_kinematics_endpoint = new IPEndPoint(0, 0);

        public String debug_message;

        public udp_receiver_cls(debug_log callback_delegate)
        {
            string Host = System.Net.Dns.GetHostName();
            debug_message = "my host -> " + Host + "\r\n";
            callback_delegate(debug_message);
            string IP1 = System.Net.Dns.GetHostByName(Host).AddressList[0].ToString();
            debug_message = "my ip -> " + IP1 + "\r\n";
            callback_delegate(debug_message);

            //local_kinematics_endpoint.Address = IPAddress.Parse("192.168.1.1");
            local_kinematics_endpoint.Address = IPAddress.Any;
            //local_kinematics_endpoint.Address = IPAddress.Parse(IP1);
            local_kinematics_endpoint.Port = 112;

            kinematics_listener = new UdpClient();

            try
            {
                kinematics_listener.Client.Bind(local_kinematics_endpoint);
            }
            catch (Exception excp)
            {
                //...
            }
        }



        public void data_receiving_method()
        {
            int counter = 0;
            ev_args = new EventArgs();
            do_job = true;


            while (do_job)
            {

                try
                {
                    // udp
                    Data_package = kinematics_listener.Receive(ref remote_endpoint); // block this thread on receive operation
                    //Data_package = fake_receive();

                    Raw_data_frame.fill_frame(Data_package);
                    // Event will be null if there are no subscribers
                    if (udp_data_received_event != null)
                        udp_data_received_event(this, ev_args);         // raise "data received" event
                    counter++;
                    Debug_string = "packet counter - " + counter.ToString() + "\n";
                }
                catch (Exception excp)
                {
                    Debug_string = "udp data read fail!!!  " + excp.ToString() + "\n";
                }


            }

        }// end data_receiving_method

        // fake receive method for debug purposes
        public void data_receiving_thread_stop()
        {
            do_job = false;
        }

        private byte[] fake_receive()
        {
            byte[] fake_data_pakage = new byte[DATA_PACKAGE_LENGTH];
            int number_of_values = (int)(DATA_PACKAGE_LENGTH / 2);
            // fill array with test data
            for (int i = 0; i < number_of_values; i++ )
            {
                fake_data_pakage[i * 2] = (byte)i;  // low byte
                fake_data_pakage[i * 2 + 1] = 0;    // high byte
            }

            number_of_values = 0;
            Thread.Sleep(25);
            number_of_values = 1;

            return fake_data_pakage;
        }
    }
}
