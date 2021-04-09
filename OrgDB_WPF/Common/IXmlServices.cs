using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OrgDB_WPF
{
    public interface IXmlServices
    {
        void WriteXml(XmlWriter writer);
        void WriteXmlBasicProperties(XmlWriter writer);
    }
}
