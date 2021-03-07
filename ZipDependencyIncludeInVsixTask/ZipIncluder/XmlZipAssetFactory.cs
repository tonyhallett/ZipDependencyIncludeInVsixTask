using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ZipDependencyIncludeInVsixTask
{
    public abstract class XmlZipAssetFactory<TXmlReflect, TZipAsset> : IZipAssetFactory<TZipAsset> where TZipAsset : IZipAsset where TXmlReflect:new()
    {
        private readonly PropertyInfo[] properties;
        protected ITaskLogger logger;
        public XmlZipAssetFactory()
        {
            properties = typeof(TXmlReflect).GetProperties();
        }
        protected abstract string TagName { get; }

        public IEnumerable<TZipAsset> Create(string assetSettingsPath, ITaskLogger logger)
        {
            this.logger = logger;
            if (!File.Exists(assetSettingsPath))
            {
                logger.LogError($"Asset settings {assetSettingsPath} does not exist");
            }
            var root = XDocument.Load(assetSettingsPath).Root.RemoveAllNamespaces();
            var elements = root.Descendants(TagName);
            var config = elements.Select(e =>
            {
                TXmlReflect instance = (TXmlReflect)Activator.CreateInstance(typeof(TXmlReflect));
                foreach (var property in properties)
                {
                    var propertyElement = e.Element(property.Name);
                    if (propertyElement != null)
                    {
                        property.SetValue(instance, propertyElement.Value);
                    }
                }
                return instance;
            });
            return CreateFrom(config);
        }

        protected abstract IEnumerable<TZipAsset> CreateFrom(IEnumerable<TXmlReflect> assets);
        
    }
}