using System;
using System.IO;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization
{
    class StandardSerializationWriter : ISerializationProcessor
    {
        public void Process(SerializationArgs args)
        {
            if (args.GetType() != typeof(StandardSerializationArgs))
                throw new ArgumentException("args should be of type StandardSerializationArgs", "args");
            using(var writer = new StreamWriter(args.ItemPath + args.ItemExtension))
                ((StandardSerializationArgs)args).SyncItem.Serialize(writer);
        }
    }
}
