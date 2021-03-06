﻿using Sitecore.Data.Items;
using Sitecore.Pipelines;

namespace Sitecore.Serialization.Args
{
    public class DeserializationArgs : PipelineArgs
    {
        public Item Item { get; set; }
        public string ItemPath { get; set; }

        public string ItemExtension { get; set; }

        public bool ForceUpdate { get; set; }

        public DeserializationArgs()
        {
            ItemExtension = ".item";
            ForceUpdate = false;
        }
    }
}
