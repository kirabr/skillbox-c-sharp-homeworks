using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace OrgDB_WPF.Products
{
    public abstract class BankProduct : IXmlServices
    {
        #region Поля

        // Идентификатор
        Guid id;

        // Наименование продукта
        string name;

        // Описание продукта
        string description;

        // Базовая процентная годовая ставка
        double basicPercentPerYear;

        // Стоимость продукта (например, обслуживания счета)
        double basicPricePerYear;

        #endregion Поля

        #region Свойства

        // Идентификатор
        public Guid ID { get { return id; } }

        // Наименование продукта
        public string Name { get { return name; } set { name = value; } }

        // Описание продукта
        public string Description { get { return description; } set { description = value; } }

        // Базовая процентная годовая ставка
        public double BasicPercentPerYear { get { return basicPercentPerYear; } set { basicPercentPerYear = value; } }

        // Стоимость продукта (например, обслуживания счета)
        public double BasicPrice { get { return basicPricePerYear; } set { basicPricePerYear = value; } }

        #endregion Свойства

        #region Конструкторы

        public BankProduct(string productName, double productPercentPerYear = 0, double productPricePerYear = 0)
        {
            name = productName;
            id = Guid.NewGuid();
            basicPercentPerYear = productPercentPerYear;
            basicPricePerYear = productPricePerYear;
        }

        public BankProduct(XPathNavigator xPathNavigator)
        {
            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//@id");
            if (selectedNode != null) id = new Guid(selectedNode.Value);

            selectedNode = xPathNavigator.SelectSingleNode("//Name");
            if (selectedNode != null) name = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//Description");
            if (selectedNode != null) description = selectedNode.Value;

            selectedNode = xPathNavigator.SelectSingleNode("//BasicPercentPerYear");
            if (selectedNode != null) basicPercentPerYear = selectedNode.ValueAsDouble;

            selectedNode = xPathNavigator.SelectSingleNode("//BasicPrice");
            if (selectedNode != null) basicPricePerYear = selectedNode.ValueAsDouble;

        }

        #endregion Конструкторы

        #region API


        #region Запись в XML

        abstract public void WriteXml(XmlWriter writer);

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", ID.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Description", Description);
            writer.WriteStartElement("BasicPercentPerYear"); writer.WriteValue(BasicPercentPerYear); writer.WriteEndElement();
            writer.WriteStartElement("BasicPrice"); writer.WriteValue(BasicPrice); writer.WriteEndElement();
        }

        #endregion Запись в XML


        #endregion API

        #region Собственные методы

        #endregion Собственные методы

    }
}
