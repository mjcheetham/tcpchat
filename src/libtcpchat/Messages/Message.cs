using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    public abstract class Message
    {
        [JsonProperty("type")]
        internal abstract string Type { get; }
    }
}
