using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    public class RelayMessage : Message
    {
        internal RelayMessage() { }

        public RelayMessage(string senderName, string text)
        {
            SenderName = senderName;
            Text = text;
        }

        internal override string Type => "relay";

        [JsonProperty("n")]
        public string SenderName { get; set; }

        [JsonProperty("t")]
        public string Text { get; set; }
    }
}
