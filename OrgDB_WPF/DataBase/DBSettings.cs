using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrgDB_WPF
{
    public struct DBSettings : IXmlServices
    {

        #region  Поля

        // Путь к файлу базы
        private string dbFilePath;

        #endregion Поля

        #region Свойства
        
        // Путь к файлу базы
        public string DBFilePath 
        { 
            get { return dbFilePath; }
            set
            {
                dbFilePath = value;
            }
        }
        
        #endregion Свойства

        #region Основные методы

        /// <summary>
        /// Инициализирует настройки 
        /// (заполняет минимально допустимыми значениями по умолчанию)
        /// </summary>
        public void Initialize()
        {
            DBFilePath = "";
        }

        public void SetDBFilePath (string value)
        {
            DBFilePath = value;
        }

        #endregion Основные методы

        #region Запись / чтение XML

        /// <summary>
        /// Записывает настройки в XML
        /// </summary>
        /// <param name="writer">
        /// XML-запись
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteElementString("dbFilePath", DBFilePath);
        }

        /// <summary>
        /// Читает настройки из XML
        /// </summary>
        /// <param name="reader">
        /// XML-читалка
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            // Перемещаемся к началу элемента
            reader.ReadStartElement();

            // Анализируем имя узла, устанавливаем соответствующее свойство или поле
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.Name)
                {
                    case "dbFilePath":
                        DBFilePath = reader.ReadElementContentAsString();
                        break;
                }
            }

            reader.ReadEndElement();
        }

        #endregion Запись / чтение XML

    }
}
