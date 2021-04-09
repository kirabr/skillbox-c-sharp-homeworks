using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF
{
    public class Organization : INotifyPropertyChanged, IXmlServices
    {

        #region Поля
                
        // Процент от зарплаты подчинённых сотрудников
        private double managerSalaryPercent;

        // Минимально гарантированная зарплата управляющего, специалиста, интерна
        private int minManagerSalary;
        private int minSpecSalary;
        private int minInternSalary;

        #endregion Поля

        #region Свойства

        // Процент от зарплаты подчинённых сотрудников
        public double ManagerSalaryPercent
        {
            get { return Math.Max(10, managerSalaryPercent); }

            set { managerSalaryPercent = value; OnPropertyChanged("ManagerSalaryPercent"); }
        }

        public int MinManagerSalary
        {
            get { return Math.Max(1300, minManagerSalary); }

            set { minManagerSalary = value; OnPropertyChanged("MinManagerSalary"); }
        }

        public int MinSpecSalary
        {
            get { return Math.Max(750, minSpecSalary); }

            set { minSpecSalary = value; OnPropertyChanged("MinSpecSalary"); }
        }

        public int MinInternSalary
        {
            get { return Math.Max(300, minInternSalary); }

            set { minInternSalary = value; OnPropertyChanged("MinInternSalary"); }
        }

        #endregion Свойства

        #region Запись / чтение XML

        /// <summary>
        /// Записывает организацию в XML
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
            Common.WriteXMLElement(writer, "ManagerSalaryPercent", ManagerSalaryPercent);
            Common.WriteXMLElement(writer, "MinManagerSalary", MinManagerSalary);
            Common.WriteXMLElement(writer, "MinSpecSalary", MinSpecSalary);
            Common.WriteXMLElement(writer, "MinInternSalary", MinInternSalary);
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

        #region Очистка

        public void Flush()
        {
            managerSalaryPercent = 0;
            minInternSalary = 0;
            minSpecSalary = 0;
            minInternSalary = 0;
        }

        #endregion Очистка

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
