using Newtonsoft.Json;

namespace ZipDependencyIncludeInVsixTask
{
    internal class ZipIncludeSettingsDeserializer : IZipIncludeSettingsDeserializer
    {
        public ZipIncludeSettings Deserialize(string payload)
        {
            return JsonConvert.DeserializeObject<ZipIncludeSettings>(payload);
        }
    }

}
