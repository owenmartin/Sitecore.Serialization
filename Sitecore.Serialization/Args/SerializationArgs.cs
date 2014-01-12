using Sitecore.Data.Items;
using Sitecore.Pipelines;

namespace Sitecore.Serialization.Args
{
    public class SerializationArgs : PipelineArgs
    {
        public Item Item { get; set; }
        public string ItemPath { get; set; }
        public string ItemExtension { get; set; }

        public SerializationArgs()
        {
            ItemExtension = ".item";
        }
    }
}
