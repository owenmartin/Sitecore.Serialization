using Sitecore.Serialization.Args;

namespace Sitecore.Serialization.Deserialization
{
    interface IDeserializationProcessor
    {
        void Process(DeserializationArgs arg);
    }
}
