using LibData;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace LibClient
{
    // Note: Do not change this class 
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

    // Note: Do not change this class 
    public class Output
    {
        public string Client_id { get; set; } // the id of the client that requests the book
        public string BookName { get; set; } // the name of the book to be reqyested
        public string Status { get; set; } // final status received from the server

        public string
            BorrowerName { get; set; } // the name of the borrower in case the status is borrowed, otherwise null

        public string
            BorrowerEmail { get; set; } // the email of the borrower in case the status is borrowed, otherwise null
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SimpleClient
    {
        private string bookName;
        public string client_id;

        public Socket clientSocket;

        // all the required settings are provided in this file
        public string configFile = @"../../../../ClientServerConfig.json";

        public IPAddress ipAddress;

        // some of the fields are defined. 
        public Output result;
        public IPEndPoint serverEndPoint;

        public Setting settings;
        //public string configFile = @"../../../../ClientServerConfig.json"; // for debugging

        // todo: add extra fields here in case needed 
    
       
        /// <summary>
        ///     Initializes the client based on the given parameters and seeting file.
        /// </summary>
        /// <param name="id">id of the clients provided by the simulator</param>
        /// <param name="bookName">name of the book to be requested from the server, provided by the simulator</param>
        public SimpleClient(int id, string bookName)
        {
            //todo: extend the body if needed.
            this.bookName = bookName;
            client_id = "Client " + id;
            result = new Output();
            result.BookName = bookName;
            result.Client_id = client_id;
            // read JSON directly from a file
            try
            {
                var configContent = File.ReadAllText(configFile);
                settings = JsonSerializer.Deserialize<Setting>(configContent);
                ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        /// <summary>
        ///     Establishes the connection with the server and requests the book according to the specified protocol.
        ///     Note: The signature of this method must not change.
        /// </summary>
        /// <returns>The result of the request</returns>
        public Output start()
        {
            // todo: implement the body to communicate with the server and requests the book. Return the result as an Output object.
            // Adding extra methods to the class is permitted. The signature of this method must not change.
            byte[] buffer = new byte[1000];
            byte[] msg;
            int b;
            string data = null;
            IPEndPoint serverEndpoint = new IPEndPoint(this.ipAddress, this.settings.ServerPortNumber){};
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(serverEndpoint);

            msg = createMessage(this.client_id, MessageType.Hello);
            sock.Send(msg);

            while (true)
            {
                b = sock.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, b);
                Message mObject = JsonSerializer.Deserialize<Message>(data);
                MessageType mType = (MessageType)Enum.Parse(typeof(MessageType), mObject.Type.ToString());

                switch (mType)
                {
                    case MessageType.Welcome:
                        msg = createMessage(this.bookName, MessageType.BookInquiry);
                        break;
                    case MessageType.BookInquiryReply:
                        sock.Listen(this.settings.ServerListeningQueue);
                        sock.Close();
                        break;
                    default:
                        break;
                }

                sock.Send(msg);
            }
            return result;
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