using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF.Products
{
    public class Credit : BankProduct
    {
        #region Поля

        #endregion Поля

        #region Свойства

        #endregion Свойства


        #region Конструкторы

        public Credit(string productName, double productPercentPerYear = 0, double productPricePerYear = 0) 
            : base(productName, productPercentPerYear, productPricePerYear)
        {
        }

        public Credit(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        #endregion Конструкторы


        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {

            writer.WriteStartElement("Credit");
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();

        }
        
        #endregion Запись в XML

    }
}
