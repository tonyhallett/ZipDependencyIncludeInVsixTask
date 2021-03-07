namespace ZipDependencyIncludeInVsixTask
{
    internal class ZipDetails
    {
        public string Path { get; set; }
        public string Version { get; set; }
    }

    internal interface IPrefixedVersionedZip
    {
        ZipDetails Find(string folder, string zipPrefix);
        string GetPath(string folder, string zipPrefix, string version);
    }
}