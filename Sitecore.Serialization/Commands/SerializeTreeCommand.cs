using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands
{
    public class SerializeTreeCommand :Command
    {
        public override void Execute(CommandContext context)
        {
            foreach (var item in context.Items)
            {
                SerializeItemAndChildren(item);
            }
            
        }

        private void SerializeItemAndChildren(Item item)
        {
            CorePipeline.Run("Serialization", new StandardSerializationArgs() {Item = item});
            foreach (Item child in item.GetChildren(ChildListOptions.SkipSorting))
            {
                SerializeItemAndChildren(child);
            }
        }
    }
}
