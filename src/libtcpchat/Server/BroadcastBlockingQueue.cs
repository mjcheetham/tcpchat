using System.Collections.Generic;
using System.Linq;

namespace Mjcheetham.TcpChat.Server
{
    internal class BroadcastBlockingQueue<T>
    {
        private readonly IList<BlockingQueue<T>> _queues = new List<BlockingQueue<T>>();

        public void Send(T obj)
        {
            SendExcept(obj, Enumerable.Empty<BlockingQueue<T>>());
        }

        public void SendExcept(T obj, BlockingQueue<T> except)
        {
            SendExcept(obj, new[]{except});
        }

        public void SendExcept(T obj, IEnumerable<BlockingQueue<T>> except)
        {
            foreach (BlockingQueue<T> queue in _queues.Except(except))
            {
                queue.Enqueue(obj);
            }
        }

        public void Register(BlockingQueue<T> queue)
        {
            _queues.Add(queue);
        }

        public void Unregister(BlockingQueue<T> queue)
        {
            _queues.Remove(queue);
        }
    }
}
