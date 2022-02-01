using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using LibData;
using Microsoft.Extensions.Configuration;

namespace LibServerSolution
{
    public struct Setting
    {
        public int ServerPortNumber { get; set; }
        public string ServerIPAddress { get; set; }
        public int BookHelperPortNumber { get; set; }
        public string BookHelperIPAddress { get; set; }
        public int ServerListeningQueue { get; set; }
    }


    abstract class AbsSequentialServer
    {
        protected Setting settings;

        /// <summary>
        /// Report method can be used to print message to console in standaard formaat. 
        /// It is not mandatory to use it, but highly recommended.
        /// </summary>
        /// <param name="type">For example: [Exception], [Error], [Info] etc</param>
        /// <param name="msg"> In case of [Exception] the message of the exection can be passed. Same is valud for other types</param>

        protected void report(string type, string msg)
        {
            // Console.Clear();
            Console.Out.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>");
            if (!String.IsNullOrEmpty(msg))
            {
                msg = msg.Replace(@"\u0022", " ");
            }

            Console.Out.WriteLine("[Server] {0} : {1}", type, msg);
        }

        /// <summary>
        /// This methid loads required settings.
        /// </summary>
        protected void GetConfigurationValue()
        {
            settings = new Setting();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                IConfiguration Config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetFullPath(Path.Combine(path, @"../../../../")))
                    .AddJsonFile("appsettings.json")
                    .Build();

                settings.ServerIPAddress = Config.GetSection("ServerIPAddress").Value;
                settings.ServerPortNumber = Int32.Parse(Config.GetSection("ServerPortNumber").Value);
                settings.BookHelperIPAddress = Config.GetSection("BookHelperIPAddress").Value;
                settings.BookHelperPortNumber = Int32.Parse(Config.GetSection("BookHelperPortNumber").Value);
                settings.ServerListeningQueue = Int32.Parse(Config.GetSection("ServerListeningQueue").Value);
                // Console.WriteLine( settings.ServerIPAddress, settings.ServerPortNumber );
            }
            catch (Exception e) { report("[Exception]", e.Message); }
        }

       
        protected abstract void createSocketAndConnectHelpers();

        public abstract void handelListening();

        protected abstract Message processMessage(Message message);
    
        protected abstract Message requestDataFromHelpers(string msg);


    }

    class SequentialServer : AbsSequentialServer
    {
        // check all the required parameters for the server. How are they initialized? 
        Socket serverSocket;
        IPEndPoint listeningPoint;
        Socket bookHelperSocket;

        public SequentialServer() : base()
        {
            GetConfigurationValue();
        }
        
        /// <summary>
        /// Connect socket settings and connec
        /// </summary>
        protected override void createSocketAndConnectHelpers()
        {
            // todo: To meet the assignment requirement, finish the implementation of this method.
            // Extra Note: If failed to connect to helper. Server should retry 3 times.
            // After the 3d attempt the server starts anyway and listen to incoming messages to clients
           
            // try
            // {
                 
            // }
            // catch ()
            // {

            // }

        }

        /// <summary>
        /// This method starts the socketserver after initializion and listents to incoming connections. 
        /// It tries to connect to the book helpers. If it failes to connect to the helper. Server should retry 3 times. 
        /// After the 3d attempt the server starts any way. It listen to clients and waits for incoming messages from clients
        /// </summary>
        public override void handelListening()
        {
            createSocketAndConnectHelpers();
            //todo: To meet the assignment requirement, finish the implementation of this method.


            // try
            // {
            // }
            // catch (Exception e) {

            // }
           
        }

        /// <summary>
        /// Process the message of the client. Depending on the logic and type and content values in a message it may call 
        /// additional methods such as requestDataFromHelpers().
        /// </summary>
        /// <param name="message"></param>
        protected override Message processMessage(Message message)
        {
            Message pmReply = new Message();
            
            //todo: To meet the assignment requirement, finish the implementation of this method .
           



            return pmReply;
        }

        /// <summary>
        /// When data is processed by the server, it may decide to send a message to a book helper to request more data. 
        /// </summary>
        /// <param name="content">Content may contain a different values depending on the message type. For example "a book title"</param>
        /// <returns>Message</returns>
        protected override Message requestDataFromHelpers(string content)
        {
            Message HelperReply = new Message();
            //todo: To meet the assignment requirement, finish the implementation of this method .

            // try
            // {

               
            // }
            // catch () { }

            return HelperReply;

        }

        public void delay()
        {
            int m = 10;
            for (int i = 0; i <= m; i++)
            {
                Console.Out.Write("{0} .. ", i);
                Thread.Sleep(200);
            }
            Console.WriteLine("\n");
            //report("round:","next to start");
        }

    }
}

