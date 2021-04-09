using System;
using System.Xml;
using System.Xml.Serialization;

namespace OrgDB_WPF.Clients
{
    public class LegalEntity : Client
    {

        #region Поля

        // Полное наименование юридического лица
        string fullName;

        // ИНН, КПП
        string inn;
        string kpp;

        // Корпоративный клиент
        bool isCorporate;

        #endregion Поля


        #region Свойства

        // Полное наименование юридического лица
        public string FullName { get { return fullName; } set { fullName = value; } }

        // ИНН, КПП
        public string INN { get { return inn; } set { inn = value; } }
        public string KPP { get { return kpp; } set { kpp = value; } }

        // Корпоративный клиент
        public bool IsCorporate  { get { return isCorporate; } set { isCorporate = value; } }

        #endregion Свойства


        #region Конструкторы

        public LegalEntity(string Name, Guid Id, bool IsResident = true) : base(Name, Id, IsResident)
        {
        }

        #endregion Конструкторы

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            string EmptyID = Common.EmptyIDString();

            writer.WriteStartElement(GetType().Name);

            WriteXmlBasicProperties(writer);

            writer.WriteElementString("FullName", FullName);
            writer.WriteElementString("INN", INN);
            writer.WriteElementString("KPP", KPP);
            Common.WriteXMLElement(writer, "IsCorporate", IsCorporate);

            writer.WriteEndElement();
        }

        #endregion Запись в XML
    }


}
