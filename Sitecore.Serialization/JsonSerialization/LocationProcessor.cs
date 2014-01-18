using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.JsonSerialization
{
    public class LocationProcessor : Serialization.LocationProcessor
    {

        public override void Process(SerializationArgs args)
        {
            base.Process(args);
            if (args.ItemExtension == ".item")
                args.ItemExtension = ".item.json";
        }
    }
}
