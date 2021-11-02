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
            var Booksettings = new Setting();
            byte[] buffer = new byte[1000];
            byte[] msg = Encoding.ASCII.GetBytes("From server : your message has been deliverd.\n");
            string data = null;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ip, 32000);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(localEndPoint);
            sock.Listen(3);
            Console.WriteLine("\n Waiting for clients...");
            Socket newSock = sock.Accept();
            while (true)
            {
                int b = newSock.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, b);
                if (data == "closed")
                {
                    newSock.Close();
                    Console.WriteLine("Closing the socket...");
                    break;
                }

                else if (data == "Hello")
                {
                    Console.WriteLine("Welcome hermano!");
                    break;
                    
                }
                else
                {
                    Console.WriteLine("" + data);
                    data = null;
                    newSock.Send(msg);
                }
            }
            sock.Close();
        }
        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
            LibServerSender();
        }
    }
}