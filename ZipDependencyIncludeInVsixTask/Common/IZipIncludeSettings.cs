namespace ZipDependencyIncludeInVsixTask
{
    public interface IZipIncludeSettings
    {
        string AssetFolder { get; set; }

        bool Force { get; set; }

        bool UseExisting { get; set; }

        string AssetSettingsPath { get; set; }

        string ProjectFolder { get; set; }
    }

}
