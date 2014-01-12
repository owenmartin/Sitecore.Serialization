using System;
using Sitecore.Data.Serialization;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization
{
    public class StandardSerializationProcessor : ISerializationProcessor
    {
        public void Process(SerializationArgs args)
        {
            if (args.GetType() != typeof(StandardSerializationArgs))
                throw new ArgumentException("args should be of type StandardSerializationArgs", "args");
            ((StandardSerializationArgs)args).SyncItem = ItemSynchronization.BuildSyncItem(args.Item);
        }
    }
}
