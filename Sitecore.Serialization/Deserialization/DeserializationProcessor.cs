using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.Exceptions;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Deserialization
{
    public class DeserializationProcessor : IDeserializationProcessor
    {
        public void Process(DeserializationArgs arg)
        {
            using (
                TextReader reader =
                    new StreamReader(File.Open(arg.ItemPath + arg.ItemExtension, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {

                bool disabledLocally = ItemHandler.DisabledLocally;
                try
                {
                    ItemHandler.DisabledLocally = true;
                    ItemSynchronization.ReadItem(reader, new LoadOptions() {ForceUpdate = arg.ForceUpdate});

                }
                finally
                {
                    ItemHandler.DisabledLocally = disabledLocally;
                }
            }
        }

    }
}
