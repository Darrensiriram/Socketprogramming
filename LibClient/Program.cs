using System.Text;
using System.Diagnostics.Tracing;
using System.Drawing;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
// using LibClient;
// using LibClientSolution;


// NOTE: DO NOT CHANGE THIS FILE
namespace LibClient
{
    public class InputData
    {
        public string BookName { get; set; }
    }
    public class ClientsSimulator
    {
        private SequentialClient client;
        private List<InputData> inputDataList;
        private SequentialClient[] clients;
        private Output[] results;

        //private string inputFile = Directory.GetCurrentDirectory() + @"/LibInput.json";
        private string inputFile = AppDomain.CurrentDomain.BaseDirectory + @"../../../LibInput.json";
        private string outputFile = AppDomain.CurrentDomain.BaseDirectory + @"../../../LibOutput.json";


        /// <summary>
        /// Reads the input file and creats the clients and output objects accordingly
        /// </summary>
        public ClientsSimulator()
        {
            int id = 0;
            try
            {
                string inputContent = File.ReadAllText(inputFile);
                this.inputDataList = JsonSerializer.Deserialize<List<InputData>>(inputContent);
                clients = new SequentialClient[this.inputDataList.Count];
                results = new Output[this.inputDataList.Count];

                // each client is initialized by a client id and a book name (from the input file) to request
                foreach (InputData d in this.inputDataList)
                {
                    clients[id] = new SequentialClient(id, d.BookName);
                    id++;
                }
            }
            catch (Exception e) { Console.Out.WriteLine("[ClientSimulator] Exception: {0}", e.Message); }
        }
        /// <summary>
        /// Starts the book requesting process for each client and collects the result for each request
        /// </summary>
        public void startSimulation()
        {
            int numCases = clients.Length;
            for (int i = 0; i < numCases; i++)
            {
                Console.Out.WriteLine("\n *********** \n");
                results[i] = clients[i].handleConntectionAndMessagesToServer();

            }
            //this is the ending client
            // new SequentialClient(-1, "").handleConntectionAndMessagesToServer();
        }

        /// <summary>
        /// Prints all the results in console and produces the output file 
        /// </summary>
        public void printOutput()
        {
            Console.Out.WriteLine("\n ***************    Final Output       *********** \n");
            for (int i = 0; i < results.Length; i++)
            {
                Console.WriteLine("{0} {1} {2} {3} {4} {5}",
                    results[i].Client_id,
                    results[i].BookName,
                    results[i].BorrowerName,
                    results[i].Status,
                    results[i].ReturnDate,
                    results[i].Error);
            string outputContent = JsonSerializer.Serialize<Output[]>(this.results);
            // Console.WriteLine("Content of the Output file:\n {0}", outputContent);
            File.WriteAllText(outputFile, outputContent);

        }
    }
}

class Program
{
    /// <summary>
    /// Starts the simulation for a set of clients and produces the output results.
    /// </summary>
    /// <param name="args"></param>

    static void Main(string[] args)
    {

        // using var listener = new SocketEventListener();
        Console.Clear();
        ClientsSimulator simulator = new ClientsSimulator();
        simulator.startSimulation();
        simulator.printOutput();
    }

}

}
