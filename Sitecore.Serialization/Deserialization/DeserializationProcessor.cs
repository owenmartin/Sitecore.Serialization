using System.IO;
using System.Linq;
using Sitecore.Data.Serialization;
using Sitecore.Pipelines;
using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Deserialization
{
    public class DeserializationProcessor : SerializationBase
    {
        public void Process(DeserializationArgs arg)
        {
            var fileName = arg.ItemPath;
            if (!fileName.EndsWith(arg.ItemExtension))
                fileName += arg.ItemExtension;
            using (
                TextReader reader =
                    new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {

                LogMessage(string.Format("Deserializing: {0}", fileName));
                bool disabledLocally = ItemHandler.DisabledLocally;
                try
                {
                    ItemHandler.DisabledLocally = true;
                    ItemSynchronization.ReadItem(reader, new LoadOptions() { ForceUpdate = arg.ForceUpdate });
                    if (!arg.ForceUpdate) return;
                    var dir = new DirectoryInfo(arg.ItemPath);
                    if (!dir.Exists)
                        return;
                    foreach (var file in dir.GetFiles().Where(x => x.Extension == ".item"))
                        CorePipeline.Run("Deserialization",
                            new DeserializationArgs
                            {
                                ItemPath = file.FullName.Replace(arg.ItemExtension, string.Empty),
                                ForceUpdate = true
                            });
                    var childDirectories = dir.GetDirectories();
                    foreach (var child in childDirectories)
                    {
                        foreach (var file in child.GetFiles().Where(x => x.Extension == ".item"))
                            CorePipeline.Run("Deserialization",
                                new DeserializationArgs() {ItemPath = file.FullName, ForceUpdate = true});
                    }
                }
                finally
                {
                    ItemHandler.DisabledLocally = disabledLocally;
                }
            }
        }
    }
}
