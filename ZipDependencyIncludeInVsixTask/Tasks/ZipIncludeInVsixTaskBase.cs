using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ZipDependencyIncludeInVsixTask
{
    public abstract class ZipIncludeInVsixTaskBase : Task, ITaskLogger
    {
        protected abstract IZipIncluder ZipIncluder {get;}

        private ZipIncludeSettings GetZipIncludeSettings()
        {
            return new ZipIncludeSettings
            {
                AssetFolder = AssetFolder,
                Force = Force,
                UseExisting = UseExisting,
                AssetSettingsPath = AssetSettingsPath,
                ProjectFolder = ProjectFolder
            };
        }
        public override bool Execute()
        {
            var zipInclusions = ZipIncluder.GetIncludedZips(GetZipIncludeSettings(), this);
            CreateIncludedZipsInVsix(zipInclusions);
            
            return true;
        }

        private void CreateIncludedZipsInVsix(IEnumerable<ZipInclusion> zipInclusions)
        {
            var globalVsixSubPath = GetVsixSubPath();

            IncludedZipsInVsix = zipInclusions.Select(zip =>
            {
                Log.LogMessage($"Including zip in vsix : {zip.ZipPath}");
                var ti = new TaskItem(zip.ZipPath);
                ti.SetMetadata("IncludeInVSIX", "true");
                var vsixSubPath = string.IsNullOrEmpty(zip.VSIXSubPath) ? globalVsixSubPath : zip.VSIXSubPath;
                ti.SetMetadata("VSIXSubPath", vsixSubPath);
                return ti;
            }).ToArray();
        }

        private string GetVsixSubPath()
        {
            if (!string.IsNullOrEmpty(VSIXSubPath))
            {
                return VSIXSubPath;
            }
            return new FileInfo(AssetFolder).Name;
        }

        public void LogMessage(string message)
        {
            Log.LogMessage(message);
        }

        public void LogError(string errorMessage)
        {
            Log.LogError(errorMessage);
        }

        public void LogWarning(string warningMessage)
        {
            Log.LogWarning(warningMessage);
        }

        
        public string AssetFolder { get; set; }
        public string ProjectFolder { get; set; }

        public bool Force { get; set; }

        public bool UseExisting { get; set; }

        public string VSIXSubPath { get; set; }

        [Output]
        public ITaskItem[] IncludedZipsInVsix { get; set; }

        [Required]
        public string AssetSettingsPath { get; set; }
}

}
