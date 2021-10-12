using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF
{
    /// <summary>
    /// Интерфейс для работы объектов сборки с XML
    /// </summary>
    public interface IXmlServices
    {
        
        /// <summary>
        /// Записывает объект в запись XML
        /// </summary>
        /// <param name="writer"></param>
        void WriteXml(XmlWriter writer);

        /// <summary>
        /// Записывает основные свойства объекта в запись XML
        /// </summary>
        /// <param name="writer"></param>
        void WriteXmlBasicProperties(XmlWriter writer);
                
    }

}
