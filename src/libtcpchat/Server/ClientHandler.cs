using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Mjcheetham.TcpChat.Messages;

namespace Mjcheetham.TcpChat.Server
{
    internal class ClientHandler
    {
        private static readonly Random Random = new Random();

        private readonly Socket _socket;
        private readonly BroadcastBlockingQueue<Message> _broadcastQueue;
        private readonly BlockingQueue<Message> _messageQueue;
        private readonly Thread _sendThread;
        private readonly Thread _receiveThread;

        public ClientHandler(Socket socket, BroadcastBlockingQueue<Message> broadcastQueue)
        {
            _socket = socket;
            _broadcastQueue = broadcastQueue;

            Name = $"Anonymous{Random.Next(10000, 99999)}";

            _messageQueue = new BlockingQueue<Message>();
            _broadcastQueue.Register(_messageQueue);

            _sendThread = new Thread(SendProc) {IsBackground = true};
            _sendThread.Start();

            _receiveThread = new Thread(ReceiveProc) {IsBackground = true};
            _receiveThread.Start();
        }

        public string Name { get; private set; }

        private void SendProc()
        {
            using (var stream = new NetworkStream(_socket, FileAccess.Write, ownsSocket: false))
            {
                // Broadcast arrival to others
                _broadcastQueue.SendExcept(new ServerMessage($"{Name} has joined."), _messageQueue);

                // Queue a welcome message
                // TODO: include most recent messages in welcome message
                _messageQueue.Enqueue(new WelcomeMessage(Name, $"Welcome! Your name is {Name}."));

                while (true)
                {
                    Message message = _messageQueue.Dequeue();

                    stream.WriteMessage(message);
                }
            }
        }

        private void ReceiveProc()
        {
            using (var stream = new NetworkStream(_socket, FileAccess.Read, ownsSocket: false))
            {
                while (true)
                {
                    Message message = stream.ReadMessage();

                    switch (message)
                    {
                        case ChatMessage chatMessage:
                            _broadcastQueue.Send(new RelayMessage(Name, chatMessage.Text));
                            break;

                        case ChangeNameMessage nameMessage:
                            string oldName = Name;
                            Name = nameMessage.NewName;
                            _broadcastQueue.Send(new ServerMessage($"{oldName} changed their name to {Name}"));
                            break;
                    }
                }
            }
        }
    }
}
