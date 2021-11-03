using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using LibData;

namespace UserHelper
{
    // Note: Do not change this class.
    public class Setting
    {
        public int ServerPortNumber { get; set; }
        public int BookHelperPortNumber { get; set; }
        public int UserHelperPortNumber { get; set; }
        public string ServerIPAddress { get; set; }
        public string BookHelperIPAddress { get; set; }
        public string UserHelperIPAddress { get; set; }
        public int ServerListeningQueue { get; set; }
    }


    public class UserOutput
    {
        public string json = @"../../../Users.json";
        public List<UserData> output;

        public UserOutput()
        {
            try
            {
                string file = File.ReadAllText(json);

                output = JsonSerializer.Deserialize<List<UserData>>(file);

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        public UserData getOutputById(string user_id)
        {
            UserData x = new UserData();
            x.User_id = user_id;
            foreach (UserData item in output)
            {
                if (item.User_id == user_id)
                {
                    x = item;
                }
            }
            return x;
        }
    }


    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SequentialHelper
    {
        private Setting settings;
        private IPAddress localIpAddress;
        private IPAddress ServerIPAddress;
        public string configFile = @"../../../../ClientServerConfig.json";

        public SequentialHelper()
        {
            
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.localIpAddress = IPAddress.Parse(settings.UserHelperIPAddress);
                this.ServerIPAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }
       
        public byte[] createMessage(string content, MessageType type)
        {
            Message m = new Message();
            m.Content = content;
            m.Type = type;

            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
        }


        public void start()
        {
            UserOutput uHelper = new UserOutput();

            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int MsgCounter = 0;
            int b = 0;
            string data;
            IPEndPoint localEndpoint = new IPEndPoint(this.localIpAddress, this.settings.UserHelperPortNumber);
            IPEndPoint sender = new IPEndPoint(this.ServerIPAddress, this.settings.ServerPortNumber);
            EndPoint remoteEP = (EndPoint)sender;
            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(localEndpoint);
                Console.WriteLine("Userhelper: waiting for messages from server");
                while (true)
                {

                    b = sock.ReceiveFrom(buffer, ref remoteEP);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());
                    Console.WriteLine("****************");
                    Console.WriteLine("Message: " + mType + " Content: " + mObject.Content.ToString());
                    switch (mType)
                    {
                        case MessageType.UserInquiryReply:
                            UserData content = uHelper.getOutputById(mObject.Content.ToString());
                           // string content = uHelper.getOuputById(mObject.Content.ToString());
                            msg = createMessage(JsonSerializer.Serialize(content), MessageType.UserInquiryReply);
                            break;
                    }

                    sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                    break;
                    MsgCounter++;
                }
                sock.Close();
            }
            catch
            {
                Console.WriteLine("\n Socket Error. Terminating");
            }
        }
    }
}