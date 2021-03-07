using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace ZipDependencyIncludeInVsixTask
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ZipIncludeInVsixToolTaskBase : ToolTask, IZipIncludeSettings
    {
        private readonly List<ZipInclusion> zipInclusions = new List<ZipInclusion>();
        private readonly ITaskProxy taskProxy;
        private const string ZipIncludeToolTaskDebugLaunch = "ZipIncludeToolTaskDebugLaunch";

        public ZipIncludeInVsixToolTaskBase():this(new TaskProxy())
        {
            LogStandardErrorAsError = true;
        }
        internal ZipIncludeInVsixToolTaskBase(ITaskProxy taskProxy)
        {
            this.taskProxy = taskProxy;
        }

        protected override string GenerateCommandLineCommands()
        {
            return "\"" + JsonConvert.SerializeObject(this).Replace("\"", "\\\"") + "\"";
        }
        
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            var possibleZip = taskProxy.LogOrReturnZip(singleLine, message => Log.LogMessage(message), warning => Log.LogWarning(warning));
            if(possibleZip != null)
            {
                zipInclusions.Add(possibleZip);
            }
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            TryDebugLaunch();
            var result = base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
            CreateIncludedZipsInVsix();
            return result;
        }

        private void TryDebugLaunch()
        {
            var envVar = Environment.GetEnvironmentVariable(ZipIncludeToolTaskDebugLaunch);
            if (envVar == "true")
            {
                Debugger.Launch();
            }
        }

        private void CreateIncludedZipsInVsix()
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

        
        [JsonProperty]
        public string AssetFolder { get; set; }

        [JsonProperty]
        public string ProjectFolder { get; set; }

        [JsonProperty]
        public bool Force { get; set; }

        [JsonProperty]
        public bool UseExisting { get; set; }

        public string VSIXSubPath { get; set; }

        [Output]
        public ITaskItem[] IncludedZipsInVsix { get; set; }

        [Required]
        [JsonProperty]
        public string AssetSettingsPath { get; set; }

    }
}
