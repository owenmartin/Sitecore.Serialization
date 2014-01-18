using System;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Serialization
{
    public class StandardSerializationProcessor
    {
        public void Process(SerializationArgs args)
        {
            if (args.GetType() != typeof(StandardSerializationArgs))
                throw new ArgumentException("args should be of type StandardSerializationArgs", "args");
            ((StandardSerializationArgs)args).SyncItem = Sitecore.Data.Serialization.ItemSynchronization.BuildSyncItem(args.Item);
        }
    }
}
