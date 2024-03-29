﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrgDB_WPF.Clients;
using OrgDB_WPF.Products;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.BankAccounts
{
    // Банковский счёт
    public class BankAccount : IXmlServices
    {

        #region Поля

        // Номер счёта
        string number;

        // Владелец счёта
        Client owner;

        // Банковские продукты счёта. Первый продукт - основной (депозит, кредит). Остальные - сервисные (обслуживание и т.п.)
        List<BankProduct> products;

        #endregion Поля

        #region Свойства

        // Номер счёта
        public string Number { get { return number; } }

        // Владелец счёта
        public Client Owner { get { return owner; } }

        // Банковские продукты счёта. Первый продукт - основной (депозит, кредит). Остальные - сервисные (обслуживание и т.п.)
        public List<BankProduct> Products { get { return products; } set { products = value; } }

        #endregion Свойства

        #region Конструкторы

        public BankAccount(string accountNumber, Client accountOwner, List<BankProduct> accountProducts)
        {
            number = accountNumber;
            owner = accountOwner;
            products = accountProducts;
        }

        #endregion Конструкторы

        #region API

        #region Запись в XML

        public void WriteXml(XmlWriter writer)
        {
                        
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            string EmptyID = Common.EmptyIDString();

            writer.WriteAttributeString("Number", Number);
            writer.WriteElementString("OwnerID", Owner == null ? EmptyID : Owner.Id.ToString());
            writer.WriteStartElement("Products");
            foreach (BankProduct product in Products)
                writer.WriteElementString("ProductID", product.Id.ToString());
            writer.WriteEndElement();
        }
        
        #endregion Запись в XML

        #endregion API

        #region Собственные методы

        #endregion Собственные методы

    }
}
