using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Xml;

namespace OrgDB_WPF
{
    public class Intern : Employee
    {

        // Основное приложение
        App currApp = (App)App.Current;

        #region Свойства

        public override string Name
        {
            get { return this.name; }
            // Гарантируем уникальность имени
            set { name = value.Trim(); OnPropertyChanged("Name"); }
        }

        public override int Salary 
        { 
            get { return salary; } 
            // Гарантируем зарплату не ниже 250
            set { salary = Math.Max(currApp.DB.dbSettings.MinInternSalary, value); OnPropertyChanged("Salary"); } 
        }

        public override string Post { get { return post; } }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Surname">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Salary">Зарплата</param>
        /// <param name="DepartmentID">Идентификатор департамента</param>
        public Intern(string Name, string Surname, int Age, int Salary, Guid DepartmentID)
            : base(Surname, Age, post_enum.intern, DepartmentID)
        {
            // Через свойства гарантируем соответствие к требованиям свойств (уникальность имени, уровень з/п)
            this.Name = Name;
            this.Salary = Salary;
            //this.post_Enum = post_enum.intern;
        }

        /// <summary>
        /// Конструктор по XML-читалке
        /// </summary>
        /// <param name="reader"></param>
        public Intern(XmlReader reader) : base(reader, post_enum.intern){ }

        /// <summary>
        /// Конструктор по JSON (DTO) - объекту интерна
        /// </summary>
        /// <param name="jIntern"></param>
        public Intern(JObject jIntern) : base(jIntern) { }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Intern() : this("InternName", "InternSurname", 20, 200, Guid.Empty) { }

        #endregion Конструкторы 

        #region Запись в XML

        /// <summary>
        /// Записывает интерна в XML
        /// </summary>
        /// <param name="writer">Запись XML</param>
        public override void WriteXml(XmlWriter writer)
        {
            Common.WriteXMLEmployee(this, writer);
        }

        #endregion Запись в XML

    }

}
