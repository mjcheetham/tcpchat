using System;
using System.IO;
using System.Text;
using Mjcheetham.TcpChat.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mjcheetham.TcpChat
{
    internal static class StreamExtensions
    {
        private const int BufferSize = 4096;
        private static readonly Encoding Utf8NoBomEncoding = new UTF8Encoding(false);

        public static void WriteMessage(this Stream stream, Message message)
        {
            var serializer = new JsonSerializer();

            using (var writer = new StreamWriter(stream, Utf8NoBomEncoding, BufferSize, true))
            using (var jsonWriter = new JsonTextWriter(writer){CloseOutput = false})
            {
                serializer.Serialize(jsonWriter, message);
            }
        }

        public static Message ReadMessage(this Stream stream)
        {
            var serializer = new JsonSerializer
            {
                Converters = { new MessageDeserializeConverter() }
            };

            using (var reader = new StreamReader(stream, Utf8NoBomEncoding, false, BufferSize, true ))
            using (var jsonReader = new JsonTextReader(reader){CloseInput = false})
            {
                return serializer.Deserialize<Message>(jsonReader);
            }
        }
    }

    internal abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    internal class MessageDeserializeConverter : JsonCreationConverter<Message>
    {
        protected override Message Create(Type objectType, JObject jsonObject)
        {
            string typeName = jsonObject["type"].ToString();
            switch (typeName)
            {
                case "changename":
                    return new ChangeNameMessage();
                case "chat":
                    return new ChatMessage();
                case "relay":
                    return new RelayMessage();
                case "server":
                    return new ServerMessage();
                case "welcome":
                    return new WelcomeMessage();
                default:
                    return null;
            }
        }
    }
}
