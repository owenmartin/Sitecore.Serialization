using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands
{
    public class DeserializeTreeCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            ProgressBox.Execute("ItemSync", "Serializa Item", "business/16x16/data_disk.png",
                Serialize, new object[]
                {
                    context.Items,
                    context.Parameters["revert"]
                });
            

        }

        private void Serialize(params object[] parameters)
        {
            foreach (var item in ((Item[])parameters[0]))
            {
                var force = (string)parameters[1] == "1";
                if (force)
                    DeerializeItemAndChildrenFromFileSystem(item);
                else
                {
                    DeerializeItemAndChildren(item);
                }
            }
        }

        private void DeerializeItemAndChildrenFromFileSystem(Item item)
        {
            CorePipeline.Run("Deserialization", new DeserializationArgs
            {
                Item = item,
                ForceUpdate = true
            });
        }

        private void DeerializeItemAndChildren(Item item)
        {
            CorePipeline.Run("Deserialization", new DeserializationArgs
            {
                Item = item,
                ForceUpdate = false
            });
            foreach (Item child in item.GetChildren(ChildListOptions.SkipSorting))
            {
                DeerializeItemAndChildren(child);
            }
        }
    }
}
