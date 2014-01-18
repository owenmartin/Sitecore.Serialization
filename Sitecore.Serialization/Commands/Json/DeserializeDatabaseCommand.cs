using System.Linq;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands.Json
{
    public class DeserializeDatabaseCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            if (!context.Items.Any())
                return;
            var item = context.Items[0];
            while (item.Parent != null)
                item = item.Parent;
            DeserializeItemAndChildren(item, context.Parameters["revert"] == "1");
        }

        private void DeserializeItemAndChildren(Item item, bool forceUpdate)
        {
            CorePipeline.Run("Deserialization", new DeserializationArgs
            {
                Item = item,
                ForceUpdate = forceUpdate
            });
            foreach (Item child in item.GetChildren(ChildListOptions.SkipSorting))
            {
                DeserializeItemAndChildren(child, forceUpdate);
            }
        }
    }
}
