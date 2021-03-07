using System.Linq;
using System.Xml.Linq;

namespace ZipDependencyIncludeInVsixTask
{
    internal static class XElementUtil
    {
        public static XElement RemoveAllNamespaces(this XElement @this)
        {
            return new XElement(@this.Name.LocalName,
                from n in @this.Nodes()
                select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n),
                @this.HasAttributes ? (from a in @this.Attributes() select a) : null);
        }
    }
}