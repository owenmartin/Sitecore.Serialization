using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands.Json
{
    public class SerializeTreeCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            ProgressBox.Execute("ItemSync", "Serializa Item", "business/16x16/data_disk.png",
                this.Serialize, new object[1]
                {
                    context.Items
                });

        }

        private void Serialize(params object[] items)
        {
            foreach (Item item in (Item[])items[0])
            {
                SerializeItemAndChildren(item);
            }
        }

        private void SerializeItemAndChildren(Item item)
        {
            CorePipeline.Run("Serialization", new JsonSerializationArgs { Item = item });
            foreach (Item child in item.GetChildren(ChildListOptions.SkipSorting))
            {
                SerializeItemAndChildren(child);
            }
        }
    }
}
