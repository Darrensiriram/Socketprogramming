using LibData;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace LibServer
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
    public class SequentialServer
    {
        public Socket clientSocket;
        public IPAddress localIpAddress;
        public IPAddress bookHelperIpAddress;
        public IPAddress userHelperIpAddress;
        public Setting settings;
        // all the required settings are provided in this file
        public string configFile = @"../../../../ClientServerConfig.json";

        public SequentialServer()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.localIpAddress = IPAddress.Parse(settings.ServerIPAddress);
                this.bookHelperIpAddress = IPAddress.Parse(settings.BookHelperIPAddress);
                this.userHelperIpAddress = IPAddress.Parse(settings.UserHelperIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }
        

        public void LibServerSender()
        {
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int MsgCounter = 0;
            int b = 0;
            string data;
            IPEndPoint localEndpoint = new IPEndPoint(this.localIpAddress, this.settings.ServerPortNumber);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, this.settings.ServerPortNumber);
            EndPoint remoteEP = (EndPoint)sender;
            IPEndPoint senderBook = new IPEndPoint(this.bookHelperIpAddress, 0);
            EndPoint remoteEPBook = (EndPoint)senderBook;
            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(localEndpoint);
                while (MsgCounter < this.settings.ServerListeningQueue)
                {
                    Console.WriteLine("\n Waiting for the next client message..");
                    //debugging purpose
                    Console.WriteLine(this.settings.ServerListeningQueue.ToString());

                    b = sock.ReceiveFrom(buffer, ref remoteEP);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                    switch (mType)
                    {
                        case MessageType.Hello:
                            Console.WriteLine("A message received. Message: " + mType);
                            msg = createMessage("", MessageType.Welcome);
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                            break;
                        case MessageType.BookInquiry:
                            //TODO SEND MESSAGE TO BOOKHELPER
                            Console.WriteLine(mObject.Content.ToString());
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEPBook);
                            msg = createMessage(mObject.Content.ToString(), MessageType.BookInquiryReply);
                            // todo bookinfo naar de client forwarden
                            break;
                        
                    }

                    sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                   
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
            
            LibServerSender();
        }
        public byte[] createMessage(string content, MessageType type)
        {
            Message m = new Message();
            m.Content = content;
            m.Type = type;

            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
        }
    }
}