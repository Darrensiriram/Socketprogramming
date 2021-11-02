using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using LibData;

namespace BookHelper
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

    public class Output
    {
        public string Client_id { get; set; } // the id of the client that requests the book
        public string BookName { get; set; } // the name of the book to be reqyested
        public string Status { get; set; } // final status received from the server

        public string
            BorrowerName
        { get; set; } // the name of the borrower in case the status is borrowed, otherwise null

        public string
            BorrowerEmail
        { get; set; } // the email of the borrower in case the status is borrowed, otherwise null
        
    }

    public class BookOutput
    {
        private Output Output; 
        public string json = @"../../../../Books.json";


        public BookOutput()
        {
            try
            {
                string configContent = File.ReadAllText(json);

                Output mObject = JsonSerializer.Deserialize<Output>(json);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }  
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
            //todo: implement the body. Add extra fields and methods to the class if needed
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.localIpAddress = IPAddress.Parse(settings.BookHelperIPAddress);
                this.ServerIPAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        
        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            Socket sock;
            int MsgCounter = 0;
            int b = 0;
            string data;
            IPEndPoint localEndpoint = new IPEndPoint(this.localIpAddress, this.settings.BookHelperPortNumber);
            IPEndPoint sender = new IPEndPoint(this.ServerIPAddress, this.settings.ServerPortNumber);
            EndPoint remoteEP = (EndPoint)sender;
            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                sock.Bind(localEndpoint);
                while (MsgCounter < this.settings.ServerListeningQueue)
                {
                    Console.WriteLine("\n Waiting for the next server message..");
                    //debugging purpose
                    Console.WriteLine(this.settings.ServerListeningQueue.ToString());

                    b = sock.ReceiveFrom(buffer, ref remoteEP);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                    switch (mType)
                    {
                        case MessageType.BookInquiry:
                            Console.WriteLine("A message received from server");
                            Console.WriteLine("Message: " + mType);
                            Console.WriteLine("Content: " + mObject.Content.ToString());
                            msg = createMessage("this is a test", MessageType.BookInquiry);
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
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

        public byte[] createMessage(string content, MessageType type)
        {
            Message m = new Message();
            m.Content = content;
            m.Type = type;

            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(m));
        }
    }
}