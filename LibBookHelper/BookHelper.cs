using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
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
        public string configFile = @"../../../../ClientServerConfig.json";

        public SequentialHelper()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.localIpAddress = IPAddress.Parse(settings.BookHelperIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        
        public void start()
        {
            //todo: implement the body. Add extra fields and methods to the class if needed
            
        }
    }
}