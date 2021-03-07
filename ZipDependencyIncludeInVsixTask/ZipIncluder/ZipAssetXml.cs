namespace ZipDependencyIncludeInVsixTask
{
    public class ZipAssetXml
    {
        public string ZipPrefix { get; set; }
        public string AssetFolder { get; set; }
        public string VSIXSubPath { get; set; }

        public void Populate(IZipAsset zipAsset)
        {
            zipAsset.ZipPrefix = ZipPrefix;
            zipAsset.AssetFolder = AssetFolder;
            zipAsset.VSIXSubPath = VSIXSubPath;
        }
    }
}
