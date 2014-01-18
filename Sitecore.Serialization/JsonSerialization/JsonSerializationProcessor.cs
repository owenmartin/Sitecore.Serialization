using System;
using Sitecore.Serialization.Args;
using Sitecore.Serialization.JsonDeserialization;

namespace Sitecore.Serialization.JsonSerialization
{
    public class JsonSerializationProcessor
    {
        public void Process(SerializationArgs args)
        {

            if (args.GetType() != typeof(JsonSerializationArgs))
                throw new ArgumentException("args should be of type JsonSerializationArgs", "args");
            var itemSerialization = new ItemSynchronization();
            ((JsonSerializationArgs) args).SerializedItem = itemSerialization.BuildSyncItem(args.Item);
        }
    }
}
