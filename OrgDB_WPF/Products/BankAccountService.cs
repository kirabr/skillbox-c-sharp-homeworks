using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF.Products
{
    
    // Обслуживание банковского счёта
    public class BankAccountService : BankProduct
    {
        public BankAccountService(string productName, double productPercentPerYear = 0, double productPricePerYear = 0) 
            : base(productName, new Guid(), productPercentPerYear, productPricePerYear)
        {
        }


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
