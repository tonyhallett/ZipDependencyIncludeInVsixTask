namespace ZipDependencyIncludeInVsixTask
{
    public class ZipIncludeSettings : IZipIncludeSettings
    {
        public string AssetFolder { get; set; }

        public bool Force { get; set; }

        public bool UseExisting { get; set; }

        public string AssetSettingsPath { get; set; }

        public string ProjectFolder { get; set; }
    }

}
