using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
    
    
    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SequentialHelper
    {
        public void BookConnectionSender()
        {
            
            var BookSetting = new Setting();
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
                Console.WriteLine("\nEnter Hello to send it to the server: ");
                data = Console.ReadLine();
                if (data.Length != 0){
                    msg =  Encoding.ASCII.GetBytes(data);
                    sock.Send(msg);
                    int b = sock.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0 , b);
                    Console.WriteLine("" + data);
                    data = null;
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
        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            BookConnectionSender();
        }
    }
}