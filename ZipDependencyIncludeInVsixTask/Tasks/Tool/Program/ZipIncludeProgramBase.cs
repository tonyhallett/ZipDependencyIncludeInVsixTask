using System;
using System.Diagnostics;

namespace ZipDependencyIncludeInVsixTask
{
    public abstract class ZipIncludeProgramBase
    {
        private readonly IZipIncludeAnnouncer zipIncludeAnnouncer;
        private readonly IZipIncludeSettingsDeserializer zipIncludeSettingsDeserializer;
        private readonly ITaskLogger logger;
        private const string ZipIncludeProgramDebugLaunch = "ZipIncludeProgramDebugLaunch";
        public ZipIncludeProgramBase() {

            var taskProxy = new TaskProxy();
            zipIncludeAnnouncer = taskProxy;
            logger = taskProxy;
            zipIncludeSettingsDeserializer = new ZipIncludeSettingsDeserializer();
        }
        internal ZipIncludeProgramBase(
            IZipIncludeAnnouncer zipIncludeAnnouncer,
            IZipIncludeSettingsDeserializer zipIncludeSettingsDeserializer,
            ITaskLogger logger)
        {
            this.zipIncludeAnnouncer = zipIncludeAnnouncer;
            this.zipIncludeSettingsDeserializer = zipIncludeSettingsDeserializer;
            this.logger = logger;
        }
        protected abstract IZipIncluder ZipIncluder { get; }
        public void Generate(string[] args)
        {
            TryDebugLaunch();

            var zipIncludeSettings = zipIncludeSettingsDeserializer.Deserialize(args[0]);
            var zipInclusions = ZipIncluder.GetIncludedZips(zipIncludeSettings, logger);
            foreach(var includedZip in zipInclusions)
            {
                zipIncludeAnnouncer.IncludeZip(includedZip);
            }
        }

        private void TryDebugLaunch()
        {
            var envVar = Environment.GetEnvironmentVariable(ZipIncludeProgramDebugLaunch);
            if(envVar == "true")
            {
                Debugger.Launch();
            }
        }
    }

}
