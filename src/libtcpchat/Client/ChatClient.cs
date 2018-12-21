using System;
using System.Net.Sockets;
using System.Threading;
using Mjcheetham.TcpChat.Messages;

namespace Mjcheetham.TcpChat.Client
{
    public class ChatClient : IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;

        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _listeningThread;

        public ChatClient(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }

        public event EventHandler<ServerMessage> ServerMessageReceived;
        public event EventHandler<RelayMessage> MessageReceived;

        public string Name { get; private set; }

        public void Connect()
        {
            _client = new TcpClient(_hostname, _port);
            _stream = _client.GetStream();

            // Start listening thread
            _listeningThread = new Thread(ListeningProc) {IsBackground = true};
            _listeningThread.Start();
        }

        public void SendMessage(string message)
        {
            _stream.WriteMessage(new ChatMessage(message));
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException();
            }

            _stream.WriteMessage(new ChangeNameMessage(name));
        }

        private void ListeningProc()
        {
            while (true)
            {
                Message message = _stream.ReadMessage();

                switch (message)
                {
                    case ServerMessage serverMessage:
                        if (serverMessage is WelcomeMessage welcomeMessage)
                        {
                            Name = welcomeMessage.Name;
                        }
                        ServerMessageReceived?.Invoke(this, serverMessage);
                        break;

                    case RelayMessage relayMessage:
                        MessageReceived?.Invoke(this, relayMessage);
                        break;

                    // TODO: support other messages
                }
            }
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
        }
    }
}
