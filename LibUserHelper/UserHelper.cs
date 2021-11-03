using System;
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
     
    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SequentialHelper
    {
        private Setting settings;
        private IPAddress localIpAddress;
        private IPAddress ServerIPAddress;
        public string configFile = @"../../../../ClientServerConfig.json";

        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
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
        //TODO: userinfo extracte van de json file! 
        public byte[] createMessage(string content, MessageType type)
        {
            Message m = new Message();
            m.Content = content;
            m.Type = type;

            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
        }

        public void ServerConnection()
        {
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int MsgCounter = 0;
            int b = 0;
            string data;
            IPEndPoint LEP = new IPEndPoint(this.localIpAddress, this.settings.UserHelperPortNumber);
            IPEndPoint sender = new IPEndPoint(this.ServerIPAddress, this.settings.ServerPortNumber);
            EndPoint remoteEp = (EndPoint) sender;

            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(LEP);
                while (MsgCounter < this.settings.ServerListeningQueue)
                {
                    b = sock.ReceiveFrom(buffer, ref remoteEp);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType) Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                    switch (mType)
                    {
                        case MessageType.UserInquiryReply:
                            Console.WriteLine("Yeey ik ben bij de user gekomen");
                            msg = createMessage(mObject.Content.ToString(), MessageType.UserInquiryReply);
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEp);
                            break;
                    }
                    // not sure if needed.... 
                    // sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEp);
                    MsgCounter++;
                }
                sock.Close();
            }
            catch 
            {
                Console.WriteLine("\n Socket Error. Terminating");
            }

        }

        public void start()
        {
            ServerConnection();            
        }
    }
}