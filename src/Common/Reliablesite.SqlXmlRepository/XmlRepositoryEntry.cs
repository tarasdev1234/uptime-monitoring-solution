using System;
using System.Xml.Linq;

namespace Reliablesite.SqlXmlRepository
{
    public sealed class XmlRepositoryEntry
    {
        public Guid Id { get; set; }
        public XElement Value { get; set; }
        public string FriendlyName { get; set; }
    }
}
