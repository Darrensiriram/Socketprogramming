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
    
        public void clientReceiver()
        {
            byte[] buffer = new byte[1000];
            byte[] msg = new byte[1000];
            string data = null;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEndpoint = new IPEndPoint(ip,32000);
            ConsoleKeyInfo key;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(serverEndpoint);
            
            while (true)
            {
                Console.WriteLine("\nDo you want to talk to the user or helper? ");
                data = Console.ReadLine();
                if (data.Length != 0){
                    msg =  Encoding.ASCII.GetBytes(data);
                    sock.Send(msg);
                    int b = sock.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0 , b);
                    Console.WriteLine("" + data);
                    data = null;
                }
                // make a desision if you want to talk to the user or book
                ConsoleKeyInfo choice;
                Console.WriteLine("\n << Book 'b', User 'u' >>");
                choice = Console.ReadKey();
                if (choice.KeyChar == 'b')
                {
                    sock.Send(Encoding.ASCII.GetBytes(bookName)); // placing for the data you send to the server
                    Console.WriteLine(" " + bookName); // double check if the book name is correct! 
                    break;
                    //todo make a connection to wards book helper!
                }
                
                Console.WriteLine("\n<< Continue 'y' , Exit 'e'>>\n");
                key = Console.ReadKey();
                
                if (key.KeyChar == 'e')
                {
                    sock.Send(Encoding.ASCII.GetBytes("Closed"));
                    Console.WriteLine("\nExiting.. Press any key to continue");
                    key = Console.ReadKey();
                    sock.Close();
                    break;
                }
            }
        }
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
            clientReceiver();
            return result;
        }
    }
}