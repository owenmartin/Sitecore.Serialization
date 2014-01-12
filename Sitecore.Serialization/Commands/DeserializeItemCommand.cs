using System.Linq;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands
{
    public class DeserializeItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            CorePipeline.Run("Deserialization", new DeserializationArgs()
            {
                Item = context.Items.First(),
                ForceUpdate = context.Parameters["revert"] == "1"
            
            });
        }
    }
}
