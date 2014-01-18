using System;
using System.IO;
using Newtonsoft.Json;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.JsonSerialization
{
    public class JsonSerializationWriter : SerializationBase
    {
        public void Process(SerializationArgs args)
        {
            if (args.GetType() != typeof(JsonSerializationArgs))
                throw new ArgumentException("args should be of type JsonSerializationArgs", "args");

            LogMessage(string.Format("Serializing: {0}", args.Item.Paths.FullPath));
            var json = JsonConvert.SerializeObject(((JsonSerializationArgs)args).SerializedItem);
            using (var writer = new StreamWriter(args.ItemPath + args.ItemExtension))
                writer.Write(json);
        }
    }
}
