using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
using System.Text.Json;
using LibClient;


// NOTE: DO NOT CHANGE THIS FILE
namespace LibClientSimulator
{
    public class InputData
    {
        public string BookName { get; set; }
    }

    public class ClientsSimulator
    {
        private SimpleClient client;
        private readonly SimpleClient[] clients;
        private readonly List<InputData> inputDataList;
        private readonly string inputFile = @"../../../LibInput.json";
        private readonly string outputFile = @"LibOutput.json";
        private readonly Output[] results;

        // paths for debugging
        //private string inputFile = @"../../../LibInput.json";
        //private string outputFile = @"../../../LibOutput.json";

        /// <summary>
        ///     Reads the input file and creats the clients and output objects accordingly
        /// </summary>
        public ClientsSimulator()
        {
            var id = 0;
            try
            {
                var inputContent = File.ReadAllText(inputFile);
                inputDataList = JsonSerializer.Deserialize<List<InputData>>(inputContent);
                clients = new SimpleClient[inputDataList.Count];
                results = new Output[inputDataList.Count];

                // each client is initialized by a client id and a book name (from the input file) to request
                foreach (var d in inputDataList)
                {
                    clients[id] = new SimpleClient(id, d.BookName);
                    id++;
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[ClientSimulator] Exception: {0}", e.Message);
            }
        }

        /// <summary>
        ///     Starts the book requesting process for each client and collects the result for each request
        /// </summary>
        public void startSimulation()
        {
            var numCases = clients.Length;
            for (var i = 0; i < numCases; i++)
            {
                Console.Out.WriteLine("\n *********** \n");
                results[i] = clients[i].start();
            }

            //this is the ending client
            new SimpleClient(-1, "").start();
        }

        /// <summary>
        ///     Prints all the results in console and produces the output file
        /// </summary>
        public void printOutput()
        {
            Console.Out.WriteLine("\n ***************    Final Output       *********** \n");
            for (var i = 0; i < results.Length; i++)
                Console.WriteLine("{0} {1} {2} {3}",
                    results[i].Client_id,
                    results[i].BookName,
                    results[i].BorrowerName,
                    results[i].BorrowerEmail);
            var outputContent = JsonSerializer.Serialize(results);
            Console.WriteLine("Content of the Output file:\n {0}", outputContent);

            File.WriteAllText(outputFile, outputContent);
        }
    }

    internal class Program
    {
        /// <summary>
        ///     Starts the simulation for a set of clients and produces the output results.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            using var listener = new SocketEventListener();
            Console.Clear();
            var simulator = new ClientsSimulator();
            simulator.startSimulation();

            simulator.printOutput();
        }

        internal sealed class SocketEventListener : EventListener
        {
            // Constant necessary for attaching ActivityId to the events.
            public const EventKeywords TasksFlowActivityIds = (EventKeywords) 0x80;

            protected override void OnEventSourceCreated(EventSource eventSource)
            {
                // List of event source names provided by networking in .NET 5.
                if (eventSource.Name == "System.Net.Sockets" ||
                    eventSource.Name == "System.Net.Security" ||
                    eventSource.Name == "System.Net.NameResolution")
                    EnableEvents(eventSource, EventLevel.LogAlways);
                // Turn on ActivityId.
                else if (eventSource.Name == "System.Threading.Tasks.TplEventSource")
                    // Attach ActivityId to the events.
                    EnableEvents(eventSource, EventLevel.LogAlways, TasksFlowActivityIds);
            }

            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                Console.ResetColor();

                var sb = new StringBuilder().Append(
                    $"{eventData.TimeStamp:HH:mm:ss.ff}  {eventData.ActivityId}.{eventData.RelatedActivityId}  {eventData.EventSource.Name}.{eventData.EventName}(");
                for (var i = 0; i < eventData.Payload?.Count; i++)
                {
                    sb.Append(eventData.PayloadNames?[i]).Append(": ").Append(eventData.Payload[i]);
                    if (i < eventData.Payload?.Count - 1) sb.Append(", ");
                }

                sb.Append(")");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(sb.ToString());
                Console.ResetColor();
            }
        }
    }
}