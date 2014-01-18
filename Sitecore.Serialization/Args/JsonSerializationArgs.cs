using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Serialization.Data;

namespace Sitecore.Serialization.Args
{
    class JsonSerializationArgs : SerializationArgs
    {
        public SerializedItem SerializedItem { get; set; } 
        public JsonSerializationArgs()
        {
            ItemExtension = ".item.json";
        }
    }
}
