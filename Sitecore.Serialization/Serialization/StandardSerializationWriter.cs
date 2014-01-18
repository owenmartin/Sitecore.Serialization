using System;
using System.IO;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Serialization
{
    class StandardSerializationWriter : SerializationBase
    {
        public void Process(SerializationArgs args)
        {
            if (args.GetType() != typeof(StandardSerializationArgs))
                throw new ArgumentException("args should be of type StandardSerializationArgs", "args");

            LogMessage(string.Format("Serializing: {0}", args.Item.Paths.FullPath));
            using(var writer = new StreamWriter(args.ItemPath + args.ItemExtension))
                ((StandardSerializationArgs)args).SyncItem.Serialize(writer);
        }
    }
}
