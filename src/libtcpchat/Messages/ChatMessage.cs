using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    internal class ChatMessage : Message
    {
        internal ChatMessage() { }

        public ChatMessage(string text)
        {
            Text = text;
        }

        internal override string Type => "chat";

        [JsonProperty("t")]
        public string Text { get; set; }
    }
}
