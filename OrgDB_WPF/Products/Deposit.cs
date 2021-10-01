using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.Products
{
    public class Deposit : BankProduct
    {

        #region Поля

        // Капитализация
        bool hasCapitalization = false;

        #endregion Поля

        #region Свойства

        // Капитализация
        public bool HasCapitalization { get { return hasCapitalization; } }

        #endregion Свойства

        #region Конструкторы
        public Deposit(string productName, double productPercentPerYear = 0, double productPricePerYear = 0, bool hasCap = false) 
            : base(productName, productPercentPerYear, productPricePerYear)
        {
            hasCapitalization = hasCap;
        }

        public Deposit(XPathNavigator xPathNavigator) : base(xPathNavigator)
        {
            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//HasCapitalization");
            if (selectedNode != null) hasCapitalization = selectedNode.ValueAsBoolean;
        }

        public Deposit(JObject jBankProduct) : base(jBankProduct)
        {
            hasCapitalization = (bool)jBankProduct.SelectToken("HasCapitalization");
        }

        #endregion Конструкторы

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            Common.WriteXMLElement(writer, "HasCapitalization", HasCapitalization);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #region Запись в JSON
        public override void WriteJsonParticularProperties(JsonWriter writer)
        {

            writer.WritePropertyName("HasCapitalization"); writer.WriteValue(HasCapitalization);

        }

        #endregion Запись в JSON

    }
}
