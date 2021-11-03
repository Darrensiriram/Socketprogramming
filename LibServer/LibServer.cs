using LibData;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BookHelper;

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
            
            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(localEndpoint);
                Console.WriteLine("\n Waiting for the next client message..");
                while (true)
                {
                    //debugging purpose
                    b = sock.ReceiveFrom(buffer, ref remoteEP);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                    Console.WriteLine("****************");
                    Console.WriteLine("Message: " + mType + " Content: " + mObject.Content.ToString());
                    switch (mType)
                    {
                        case MessageType.Hello:
                            msg = createMessage("", MessageType.Welcome);
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                            break;
                        case MessageType.BookInquiry:
                            BookData bData = JsonSerializer.Deserialize<BookData>(LibBookSender(mObject.Content.ToString()));
                            Output newOutput = new Output();
                            newOutput.Status = bData.Status;
                            newOutput.BookName = bData.Title;
                            MessageType mt = MessageType.BookInquiryReply;
                            Console.WriteLine("Message: " + mType + " Content: " + mObject.Content.ToString());
                            if (newOutput.Status == "Borrowed") {
                                UserData uData = JsonSerializer.Deserialize<UserData>(LibUserSender(bData.BorrowedBy.ToString()));
                                newOutput.BorrowerEmail = uData.Email;
                                newOutput.BorrowerName = uData.Name;
                            }
                            if (bData.Status == "NotFound") {
                                mt = MessageType.NotFound;
                            }
                            msg = createMessage(JsonSerializer.Serialize(newOutput), mt);
                            break;
                    }
                    sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                    break;
                   
                }
                sock.Close();
            }
            catch
            {
                Console.WriteLine("\n Socket Error. Terminating");
            }
        }

        public string LibUserSender(string content)
        {
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int b = 0;
            string data;
            IPEndPoint ServerEndpoint = new IPEndPoint(this.userHelperIpAddress, this.settings.UserHelperPortNumber);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, this.settings.UserHelperPortNumber);
            EndPoint remoteEP = (EndPoint)sender;

            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

                msg = createMessage(content, MessageType.UserInquiry);
                sock.SendTo(msg, msg.Length, SocketFlags.None, ServerEndpoint);
                
                b = sock.ReceiveFrom(buffer, ref remoteEP);
                data = Encoding.ASCII.GetString(buffer, 0, b);
                Message mObject = JsonSerializer.Deserialize<Message>(data);
                MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                content = mObject.Content.ToString();
                   
                sock.Close();
            }
            catch
            {
                Console.WriteLine("\n Socket Error. Terminating");
            }

            return content;
        }
        public string LibBookSender(string content)
        {
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int b = 0;
            string data;
            IPEndPoint ServerEndpoint = new IPEndPoint(this.bookHelperIpAddress, this.settings.BookHelperPortNumber);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, this.settings.BookHelperPortNumber);
            EndPoint remoteEP = (EndPoint)sender;

            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

                msg = createMessage(content, MessageType.BookInquiryReply);
                sock.SendTo(msg, msg.Length, SocketFlags.None, ServerEndpoint);
                
                b = sock.ReceiveFrom(buffer, ref remoteEP);
                data = Encoding.ASCII.GetString(buffer, 0, b);
                Message mObject = JsonSerializer.Deserialize<Message>(data);
                MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                content = mObject.Content.ToString();
                   
                sock.Close();
            }
            catch
            {
                Console.WriteLine("\n Socket Error. Terminating");
            }

            return content;
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