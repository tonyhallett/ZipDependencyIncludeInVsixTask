using System.Collections.Generic;
using System.IO;

namespace ZipDependencyIncludeInVsixTask
{
    public abstract class ZipIncluderBase<TZipAsset> : IZipIncluder where TZipAsset : IZipAsset
    {
        private readonly IPrefixedVersionedZip prefixedVersionedZip;
        protected ITaskLogger logger;
        protected ZipIncludeSettings zipIncludeSettings;

        protected abstract IZipAssetFactory<TZipAsset> ZipAssetFactory { get; }
        public ZipIncluderBase() : this(new PrefixedVersionedZip()) { }

        internal ZipIncluderBase(IPrefixedVersionedZip prefixedVersionedZip)
        {
            this.prefixedVersionedZip = prefixedVersionedZip;
        }

        protected abstract string GetUpdateVersion(TZipAsset zipAsset, string currentVersion);

        private void EnsureAssetFolder(string assetFolder)
        {
            if (!Directory.Exists(assetFolder))
            {
                logger.LogMessage($"Creating AssetFolder {assetFolder}");
                Directory.CreateDirectory(assetFolder);
            }
        }

        private string UpdateVersion(string currentZip, string updateVersion, IZipAsset zipAsset, string assetFolder)
        {
            EnsureAssetFolder(assetFolder);
            if (currentZip != null)
            {
                File.Delete(currentZip);
            }
            var zipPath = Path.Combine(assetFolder, $"{zipAsset.ZipPrefix}.{updateVersion}.zip");
            logger.LogMessage($"Generating zip of {zipAsset.Type} : {zipPath}");
            zipAsset.GenerateZip(zipPath, updateVersion);
            return zipPath;
        }

        private string GetAbsoluteOrProjectRelativeFolder(string folder)
        {
            if (Path.IsPathRooted(folder))
            {
                return folder;
            }
            return Path.Combine(zipIncludeSettings.ProjectFolder, folder);
        }
        private string GetAssetFolder(IZipAsset zipAsset)
        {
            var zipAssetFolder = zipAsset.AssetFolder;
            var globalAssetFolder = zipIncludeSettings.AssetFolder;
            if (string.IsNullOrWhiteSpace(zipAssetFolder))
            {
                if(globalAssetFolder == null)
                {
                    logger.LogError($"AssetFolder not specified for zip asset with prefix {zipAsset.ZipPrefix}.  Specify for all with task AssetFolder or individually for the zip asset.");
                }
                return globalAssetFolder;
            }
            return GetAbsoluteOrProjectRelativeFolder(zipAssetFolder);
            
        }

        public List<ZipInclusion> GetIncludedZips(ZipIncludeSettings zipIncludeSettings, ITaskLogger logger)
        {
            if(zipIncludeSettings.AssetFolder != null)
            {
                zipIncludeSettings.AssetFolder = GetAbsoluteOrProjectRelativeFolder(zipIncludeSettings.AssetFolder);
            }
            
            this.zipIncludeSettings = zipIncludeSettings;

            this.logger = logger;


            List<ZipInclusion> zipsInclusions = new List<ZipInclusion>();

            var zipAssets = ZipAssetFactory.Create(zipIncludeSettings.AssetSettingsPath, logger);
            foreach (var zipAsset in zipAssets)
            {
                var assetFolder = GetAssetFolder(zipAsset);
                var zipDetails = prefixedVersionedZip.Find(assetFolder, zipAsset.ZipPrefix);
                string includedZipInVsix = null;
                string updateVersion;
                string currentVersion = zipDetails?.Version;

                if (zipDetails == null) // no zip in AssetFolder
                {
                    updateVersion = GetUpdateVersion(zipAsset, null);
                }
                else
                {
                    includedZipInVsix = zipDetails.Path;

                    if (zipIncludeSettings.UseExisting)
                    {
                        updateVersion = currentVersion;
                    }
                    else
                    {
                        updateVersion = GetUpdateVersion(zipAsset, zipDetails.Version);
                    }

                }

                var versionChange = currentVersion != updateVersion;

                // implementation can return null for update version to prevent zip update
                if (updateVersion != null && (versionChange || zipIncludeSettings.Force))
                {
                    includedZipInVsix = UpdateVersion(zipDetails?.Path, updateVersion, zipAsset,assetFolder);
                }

                zipsInclusions.Add(new ZipInclusion { ZipPath = includedZipInVsix, VSIXSubPath = zipAsset.VSIXSubPath });

            }
            return zipsInclusions;
        }
    }
}
