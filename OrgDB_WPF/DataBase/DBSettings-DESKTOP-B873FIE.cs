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
    public struct DBSettings : INotifyPropertyChanged
    {

        #region  Поля

        // Путь к файлу базы
        private string dbFilePath;

        // Процент от зарплаты подчинённых сотрудников
        private double managerSalaryPercent;

        // Минимально гарантированная зарплата управляющего, специалиста, интерна
        private int minManagerSalary;
        private int minSpecSalary;
        private int minInternSalary;

        #endregion Поля

        #region Свойства
        
        // Путь к файлу базы
        public string DBFilePath 
        { 
            get { return dbFilePath; }
            set
            {
                dbFilePath = value;
                OnPropertyChanged("DBFilePath");
            }
        }

        // Процент от зарплаты подчинённых сотрудников
        public double ManagerSalaryPercent 
        { 
            get { return managerSalaryPercent; }
            
            set 
            { 
                // Гарантируем минимальный процент
                managerSalaryPercent = Math.Max(10, value);
                OnPropertyChanged("ManagerSalaryPercent");
            } 
        }

        public int MinManagerSalary
        {
            get { return minManagerSalary; }

            set 
            { 
                minManagerSalary = Math.Max(1300, value);
                OnPropertyChanged("MinManagerSalary");
            }
        }

        public int MinSpecSalary
        {
            get { return minSpecSalary; }

            set
            {
                minSpecSalary = Math.Max(750, value);
                OnPropertyChanged("MinSpecSalary");
            }
        }

        public int MinInternSalary
        {
            get { return minInternSalary; }

            set
            {
                minInternSalary = Math.Max(350, value);
                OnPropertyChanged("MinInternSalary");
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
            ManagerSalaryPercent = 0;
            MinManagerSalary = 0;
            MinSpecSalary = 0;
            MinInternSalary = 0;
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
            Common.WriteXMLElement(writer, "ManagerSalaryPercent", ManagerSalaryPercent);
            Common.WriteXMLElement(writer, "MinManagerSalary", MinManagerSalary);
            Common.WriteXMLElement(writer, "MinSpecSalary", MinSpecSalary);
            Common.WriteXMLElement(writer, "MinInternSalary", MinInternSalary);
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
                    case "ManagerSalaryPercent":
                        ManagerSalaryPercent = reader.ReadElementContentAsDouble();
                        break;
                    case "dbFilePath":
                        DBFilePath = reader.ReadElementContentAsString();
                        break;
                    case "MinManagerSalary":
                        MinManagerSalary = reader.ReadElementContentAsInt();
                        break;
                    case "MinSpecSalary":
                        MinSpecSalary = reader.ReadElementContentAsInt();
                        break;
                    case "MinInternSalary":
                        MinInternSalary = reader.ReadElementContentAsInt();
                        break;
                }
            }

            reader.ReadEndElement();
        }

        #endregion Запись / чтение XML

        #region Реализация INotifyPropertyChanged

        /// <summary>
        /// Обработчик изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion Реализация INotifyPropertyChanged

    }
}
