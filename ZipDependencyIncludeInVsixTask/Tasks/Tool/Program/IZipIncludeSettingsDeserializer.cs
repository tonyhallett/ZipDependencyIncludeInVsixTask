namespace ZipDependencyIncludeInVsixTask
{
    internal interface IZipIncludeSettingsDeserializer
    {
        ZipIncludeSettings Deserialize(string payload);
    }

}
