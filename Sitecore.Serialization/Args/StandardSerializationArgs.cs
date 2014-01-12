using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Serialization.ObjectModel;

namespace Sitecore.Serialization.Args
{
    class StandardSerializationArgs : SerializationArgs
    {
        public SyncItem SyncItem { get; set; }
    }
}
