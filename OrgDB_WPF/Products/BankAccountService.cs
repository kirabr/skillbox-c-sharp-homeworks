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
    
    // Обслуживание банковского счёта
    public class BankAccountService : BankProduct
    {
        public BankAccountService(string productName, double productPercentPerYear = 0, double productPricePerYear = 0) 
            : base(productName, productPercentPerYear, productPricePerYear)
        {
        }

        public BankAccountService(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        public BankAccountService(JObject jBankProduct) : base(jBankProduct) { }

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Credit");
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #region Запись в JSON
        public override void WriteJsonParticularProperties(JsonWriter writer) {}

        #endregion Запись в JSON

    }
}
