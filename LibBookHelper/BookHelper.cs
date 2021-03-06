using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using LibData;
using System.Collections.Generic;

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

        public string BorrowerName{ get; set; } // the name of the borrower in case the status is borrowed, otherwise null

        public string BorrowerEmail{ get; set; } // the email of the borrower in case the status is borrowed, otherwise null
        
    }

    public class BookOutput
    {
        public string json = @"../../../Books.json";
        public List<BookData> output;
        public Output newOutput;

        public BookOutput()
        {
            try
            {
                string file = File.ReadAllText(json);

                output = JsonSerializer.Deserialize<List<BookData>>(file);

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }  
        }

        public BookData getOuputByName(string name) {
            BookData n = new BookData();
            n.Title = name;
            n.Status = "NotFound";

            foreach (BookData item in output) {
                if (item.Title == name) {
                    n = item;
                }
            }

            return n;
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
            
            BookOutput bHelper = new BookOutput();

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
                Console.WriteLine("BookHelper: Waiting for messages from server");
                bool run = true;
                while (run)
                {

                    b = sock.ReceiveFrom(buffer, ref remoteEP);
                    data = Encoding.ASCII.GetString(buffer, 0, b);
                    Message mObject = JsonSerializer.Deserialize<Message>(data);
                    MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());
                    Console.WriteLine("****************");
                    Console.WriteLine("Message: " + mType + " Content: " + mObject.Content.ToString());
                    switch (mType)
                    {
                        case MessageType.BookInquiryReply:
                            BookData content = bHelper.getOuputByName(mObject.Content.ToString());
                            MessageType mt = MessageType.BookInquiryReply;
                            if (content.Status == "NotFound") {
                                mt = MessageType.NotFound;
                            }
                            msg = createMessage(JsonSerializer.Serialize(content), mt);
                            sock.SendTo(msg, msg.Length, SocketFlags.None, remoteEP);
                            break;
                        case MessageType.EndCommunication:
                            run = false;
                            break;
                    }
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