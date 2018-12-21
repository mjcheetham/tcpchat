using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Mjcheetham.TcpChat.Messages;

namespace Mjcheetham.TcpChat.Server
{
    public class ChatServer
    {
        private const string DefaultServerName = "TCP Chat Server";

        private readonly IList<ClientHandler> _clients;
        private readonly BroadcastBlockingQueue<Message> _broadcastQueue;

        private bool _started;
        private Thread _listenerThread;

        public ChatServer(int port, string name = DefaultServerName)
            : this(IPAddress.Any, port, name) { }

        public ChatServer(IPAddress address, int port, string name = DefaultServerName)
        {
            Address = address;
            Port = port;
            Name = name;
            _clients = new List<ClientHandler>();
            _broadcastQueue = new BroadcastBlockingQueue<Message>();
        }

        public string Name { get; }

        public IPAddress Address { get; }

        public int Port { get; }

        public void Start()
        {
            if (_started)
            {
                throw new InvalidOperationException("Server has already been started.");
            }

            _listenerThread = new Thread(ListenerProc) {IsBackground = true};
            _listenerThread.Start();
            _started = true;
        }

        public void Stop()
        {
            if (!_started)
            {
                throw new InvalidOperationException("Server has not been started.");
            }

            // TODO: gracefully shutdown
            _started = false;
        }

        private void ListenerProc()
        {
            var tcpListener = new TcpListener(Address, Port);
            tcpListener.Start();

            while (true)
            {
                Socket socket = tcpListener.AcceptSocket();

                var handler = new ClientHandler(socket, _broadcastQueue);

                _clients.Add(handler);
            }
        }

        public ICollection<string> GetConnectedClients()
        {
            return _clients.Select(x => x.Name).ToList();
        }
    }
}
