using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Mjcheetham.TcpChat.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                PrintUsage();
                return;
            }

            if (!int.TryParse(args[0], out int port))
            {
                Console.Error.WriteLine("error: Invalid port number");
                PrintUsage();
                return;
            }

            var server = new ChatServer(port);
            server.Start();

            while (true)
            {
                string line = Console.ReadLine();

                if (StringComparer.OrdinalIgnoreCase.Equals(line, "\\quit") ||
                    StringComparer.OrdinalIgnoreCase.Equals(line, "\\q") ||
                    StringComparer.OrdinalIgnoreCase.Equals(line, "\\exit")||
                    StringComparer.OrdinalIgnoreCase.Equals(line, "\\stop"))
                {
                    break;
                }

                if (StringComparer.OrdinalIgnoreCase.Equals(line, "\\info"))
                {
                    Console.WriteLine("Server name: {0}", server.Name);
                    Console.WriteLine("IP Address: {0}", server.Address);
                    Console.WriteLine("Port number: {0}", server.Port);

                    ICollection<string> clients = server.GetConnectedClients();
                    if (clients.Count == 0)
                    {
                        Console.WriteLine("Connected clients: [None]");
                    }
                    else
                    {
                        Console.WriteLine("Connected clients:");
                        foreach (string client in clients)
                        {
                            Console.WriteLine("  {0}", client);
                        }
                    }

                    Console.WriteLine();
                }
            }

            server.Stop();
        }

        private static void PrintUsage()
        {
            string appName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine("usage: {0} <port>", appName);
            Console.WriteLine();
            Console.WriteLine("  <port>\tchat server port number");
            Console.WriteLine();
        }
    }
}
