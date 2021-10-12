using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF
{
    public class Manager : Employee
    {

        // Основное приложение
        App currApp = (App)App.Current;

        #region Свойства

        public override string Name
        {
            get { return name; }
            set { name = value.Trim(); ; OnPropertyChanged("Name"); }
        }
        public override int Salary 
        { 
            get { return salary; } 
            // Гарантируем минимальный уровень зарплаты
            set { salary = Math.Max(currApp.DB.Organization.MinManagerSalary, value); OnPropertyChanged("Salary"); } 
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
        /// <param name="dataBase"></param>
        public Manager(string Name, string Surname, int Age, int Salary, Guid DepartmentID)
            : base(Surname, Age, post_enum.manager, DepartmentID)
        {
            this.Name = Name;
            this.Salary = Salary;
        }

        /// <summary>
        /// Конструктор по XML-чтению. Используется при восстановлении базы из XML-файла
        /// </summary>
        /// <param name="reader"></param>
        public Manager(XmlReader reader) : base(reader, "Manager") { }

        /// <summary>
        /// Конструктор по JSON (DTO) - объекту менеджера. Используется при восстановлении базы из JSON-файла
        /// </summary>
        /// <param name="jManager"></param>
        public Manager(JObject jManager) : base(jManager) { }

        public Manager() { }

        #endregion Конструкторы

        #region Запись в XML

        /// <summary>
        /// Записывает менеджера в XML-запись
        /// </summary>
        /// <param name="writer">XML-запись</param>
        public override void WriteXml(XmlWriter writer)
        {
            WriteXmlEmployee(writer, GetType().Name);
        }

        #endregion Запись в XML

    }
}
