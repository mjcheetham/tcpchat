using System;
using System.IO;
using System.Reflection;
using Mjcheetham.TcpChat.Messages;

namespace Mjcheetham.TcpChat.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                return;
            }

            string hostname = args[0];
            if (!int.TryParse(args[1], out int port))
            {
                Console.Error.WriteLine("error: Invalid port number");
                PrintUsage();
                return;
            }

            using (var client = new ChatClient(hostname, port))
            {
                client.ServerMessageReceived += OnServerMessageReceived;
                client.MessageReceived += OnMessageReceived;
                client.Connect();

                // Start send message loop
                string line = Console.ReadLine();
                while (!IsQuit(line))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        // do nothing
                    }
                    else if (line.StartsWith("\\name ", StringComparison.OrdinalIgnoreCase))
                    {
                        string newName = line.Substring("\\name ".Length);
                        if (string.IsNullOrWhiteSpace(newName))
                        {
                            Console.Error.WriteLine("[error] Name must not be empty");
                        }
                        else
                        {
                            client.ChangeName(newName);
                        }
                    }
                    else
                    {
                        client.SendMessage(line);
                    }

                    line = Console.ReadLine();
                }
            }
        }

        private static void OnServerMessageReceived(object sender, ServerMessage e)
        {
            Console.WriteLine("[Server] {0}", e.Text);
        }

        private static void OnMessageReceived(object sender, RelayMessage e)
        {
            Console.WriteLine("{0}: {1}", e.SenderName, e.Text);
        }

        private static bool IsQuit(string line)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(line, "\\exit") ||
                   StringComparer.OrdinalIgnoreCase.Equals(line, "\\quit") ||
                   StringComparer.OrdinalIgnoreCase.Equals(line, "\\q");
        }

        private static void PrintUsage()
        {
            string appName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine("usage: {0} <hostname> <port>", appName);
            Console.WriteLine();
            Console.WriteLine("  <hostname>\tchat server hostname");
            Console.WriteLine("  <port>\tchat server port number");
            Console.WriteLine();
        }
    }
}
