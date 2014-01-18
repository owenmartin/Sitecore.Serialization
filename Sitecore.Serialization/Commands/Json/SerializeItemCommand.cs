using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands.Json
{
    public class SerializeItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            ProgressBox.Execute("ItemSync", "Serializa Item", "business/16x16/data_disk.png",
                Serialize, new object[]
                {
                    context.Items[0]
                });
        }

        private void Serialize(params object[] item)
        {
            CorePipeline.Run("Serialization", new JsonSerializationArgs { Item = (Item)item[0] });
        }
    }
}
