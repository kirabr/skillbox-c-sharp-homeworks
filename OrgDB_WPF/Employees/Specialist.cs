using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF
{
    public class Specialist : Employee
    {

        // Основное приложение
        App currApp = (App)App.Current;

        #region Свойства

        public override string Name
        {
            get { return name; }
            set { name = value.Trim(); OnPropertyChanged("Name"); }
        }
        public override int Salary
        {
            get { return salary; }
            // Гарантируем минимальный уровень зарплаты
            set { salary = Math.Max(currApp.DB.Organization.MinSpecSalary, value); OnPropertyChanged("Salary"); }
        }
        public override string Post { get { return post; } }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Surname"></param>
        /// <param name="Age"></param>
        /// <param name="Salary"></param>
        /// <param name="DepartmentID"></param>
        public Specialist(string Name, string Surname, int Age, int Salary, Guid DepartmentID)
            : base(Name, Surname, Age, Salary, post_enum.specialist, DepartmentID) { }

        /// <summary>
        /// Конструктор по XML-чтению. Используется при восстановлении базы из XML-файла
        /// </summary>
        /// <param name="reader"></param>
        public Specialist(XmlReader reader) : base(reader, post_enum.specialist) { }

        /// <summary>
        /// Конструктор по JSON (DTO) - объекту специалиста. Используется при восстановлении базы из JSON-файла
        /// </summary>
        /// <param name="jSpecialist"></param>
        public Specialist(JObject jSpecialist) : base(jSpecialist) { }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Specialist() : this("SpecName", "SpecSurName", 25, 1500, Guid.Empty) { }

        #endregion Конструкторы

        #region Запись в XML

        /// <summary>
        /// Записывает специалиста в XML
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            WriteXmlEmployee(writer, GetType().Name);
        } 
        #endregion Запись в XML

    }
}
