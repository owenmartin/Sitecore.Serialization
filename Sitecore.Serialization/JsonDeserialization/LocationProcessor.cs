using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.JsonDeserialization
{
    public class LocationProcessor : Deserialization.LocationProcessor
    {
        public override void Process(DeserializationArgs args)
        {
            base.Process(args);
            if (args.ItemExtension == ".item")
                args.ItemExtension = ".item.json";
        }
    }
}
