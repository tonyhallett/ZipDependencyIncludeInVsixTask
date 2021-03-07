using System.IO;
using System.Linq;

namespace ZipDependencyIncludeInVsixTask
{
    internal class PrefixedVersionedZip : IPrefixedVersionedZip
    {
        public ZipDetails Find(string folder, string zipPrefix)
        {
            var matchingZip = Directory.GetFiles(folder, $"{zipPrefix}.*.zip").FirstOrDefault();
            if (matchingZip != null)
            {
                var version = Path.GetFileName(matchingZip).Replace($"{zipPrefix}.", "").Replace(".zip", "");
                return new ZipDetails { Path = matchingZip, Version = version };
            }
            return null;
        }

        public string GetPath(string folder, string zipPrefix, string version)
        {
            return Path.Combine(folder, $"{zipPrefix}.{version}.zip");
        }
    }

}
