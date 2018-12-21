using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    public class ServerMessage : Message
    {
        internal ServerMessage() { }

        public ServerMessage(string text)
        {
            Text = text;
        }

        internal override string Type => "server";

        [JsonProperty("t")]
        public string Text { get; set; }
    }
}
