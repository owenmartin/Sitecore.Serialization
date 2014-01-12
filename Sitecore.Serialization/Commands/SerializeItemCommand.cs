using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Serialization.Commands
{
    public class SerializeItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            CorePipeline.Run("Serialization", new StandardSerializationArgs() {Item = context.Items.First()});
        }
    }
}
