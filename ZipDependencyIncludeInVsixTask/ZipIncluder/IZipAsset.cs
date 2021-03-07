namespace ZipDependencyIncludeInVsixTask
{
    public interface IZipAsset
    {
        string ZipPrefix { get; set; }
        void GenerateZip(string newZipPath, string updateVersion);
        string Type { get; }

        string AssetFolder { get; set; }
        string VSIXSubPath { get; set; }
    }
}
