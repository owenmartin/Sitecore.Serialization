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
    public class DeserializeTreeCommand :Command
    {
        public override void Execute(CommandContext context)
        {
            foreach (var item in context.Items)
            {
                DeerializeItemAndChildren(item, context.Parameters["revert"] == "1");
            }
            
        }

        private void DeerializeItemAndChildren(Item item, bool forceUpdate)
        {
            CorePipeline.Run("Deserialization", new DeserializationArgs
            {
                Item = item,
                ForceUpdate = forceUpdate
            });
            foreach (Item child in item.GetChildren(ChildListOptions.SkipSorting))
            {
                DeerializeItemAndChildren(child,forceUpdate);
            }
        }
    }
}
