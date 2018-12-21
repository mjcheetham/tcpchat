using Newtonsoft.Json;

namespace Mjcheetham.TcpChat.Messages
{
    internal class ChangeNameMessage : Message
    {
        internal ChangeNameMessage() { }

        public ChangeNameMessage(string newName)
        {
            NewName = newName;
        }

        internal override string Type => "changename";

        [JsonProperty("n")]
        public string NewName { get; set; }
    }
}
