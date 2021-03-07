using System.Collections.Generic;

namespace ZipDependencyIncludeInVsixTask
{
    public interface IZipAssetFactory<TZipAsset> where TZipAsset : IZipAsset
    {
        IEnumerable<TZipAsset> Create(string assetSettingsPath, ITaskLogger logger);
    }
}