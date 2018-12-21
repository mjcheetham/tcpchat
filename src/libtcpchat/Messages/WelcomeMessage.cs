using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    internal class WelcomeMessage : ServerMessage
    {
        internal WelcomeMessage() { }

        public WelcomeMessage(string name, string text) : base(text)
        {
            Name = name;
        }

        internal override string Type => "welcome";

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("last")]
        public RelayMessage[] RecentMessages { get; set; }
    }
}
