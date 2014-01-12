using System;
using System.IO;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Deserialization
{
    public class LocationProcessor : IDeserializationProcessor
    {
        public void Process(DeserializationArgs args)
        {
         
            var folder = Configuration.Settings.SerializationFolder;
            var database = args.Item.Database.Name;
            args.ItemPath = string.Format("{0}/{1}/{2}", folder, database, GetPath(args.Item));
            var itemFolder = args.ItemPath.Substring(0, args.ItemPath.LastIndexOf('/'));
            if (!Directory.Exists(itemFolder))
                Directory.CreateDirectory(itemFolder);
        }

        private string GetPath(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if (item.Parent == null)
                return "/" + item.Name;
            var path = GetPath(item.Parent) + "/" + item.Name;
            foreach (Item obj in item.Parent.GetChildren(ChildListOptions.SkipSorting))
            {
                if (obj.Name != item.Name || obj.ID == item.ID) continue;
                path += "_" + item.ID.ToShortID();
                break;
            }
            return path;
        }
    }
}
