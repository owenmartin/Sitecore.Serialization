using Sitecore.Diagnostics;
using Sitecore.Jobs;

namespace Sitecore.Serialization
{
    public class SerializationBase
    {
        protected void LogMessage(string message)
        {
            Job job = Context.Job;
            if (job != null)
                job.Status.LogInfo(message);
            else
                Log.Info(message, new object());
        }
    }
}
