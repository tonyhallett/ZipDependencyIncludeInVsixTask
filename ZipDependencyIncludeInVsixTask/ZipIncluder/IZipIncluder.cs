using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZipDependencyIncludeInVsixTask
{
    public class ZipInclusion
    {
        public string ZipPath { get; set; }
        public string VSIXSubPath { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal static ZipInclusion FromString(string asString)
        {
            return JsonConvert.DeserializeObject<ZipInclusion>(asString);
        }
    }
    public interface IZipIncluder
    {
        List<ZipInclusion> GetIncludedZips(ZipIncludeSettings zipIncludeSettings, ITaskLogger logger);
    }
}