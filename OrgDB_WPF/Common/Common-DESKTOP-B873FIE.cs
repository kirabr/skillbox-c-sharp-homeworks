using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Data;

namespace OrgDB_WPF
{
    class Common
    {
        /// <summary>
        /// Генератор авто-имени
        /// Возвращает следующее имя из серии ("Name_1", "Name_2", ...)
        /// </summary>
        /// <param name="curName">предполагаемое имя</param>
        /// <returns></returns>
        public static string NextAutoName(string curName, List<string> busyNames)
        {
            while (busyNames.Contains(curName))
            {
                int j = 0;
                int orderedNameNum = 0;
                for (int i = curName.Length - 1; i >= 0; i--)
                {
                    if (!"0123456789".Contains(curName[i])) break;
                    orderedNameNum += (int)Math.Pow(10, j) * Convert.ToInt32(curName[i].ToString());
                    j++;
                }
                int orderedNum = orderedNameNum + 1;

                curName = curName.Substring(0, curName.Length - j);
                if (curName[curName.Length - 1] == '_') curName = curName.Substring(0, curName.Length - 1);

                curName += "_" + orderedNum.ToString();
            }

            busyNames.Add(curName);

            return curName;
        }

        /// <summary>
        /// Возвращает расширение имени файла в верхнем регистре, включая точку.
        /// Например, ".XML", ".JS"
        /// </summary>
        /// <param name="FilePath">Путь к файлу</param>
        /// <returns></returns>
        public static string FilePathExtension(string FilePath)
        {
            int LastPointPosition = FilePath.LastIndexOf('.');
            return FilePath.Substring(LastPointPosition).ToUpper();
        }

        /// <summary>
        /// Записывает сотрудника в запись XML
        /// </summary>
        /// <param name="Emp">Сотрудник</param>
        /// <param name="writer">Запись XML</param>
        public static void WriteXMLEmployee(Employee Emp, XmlWriter writer)
        {
            writer.WriteStartElement(Emp.post_Enum.ToString());
            writer.WriteAttributeString("id", Emp.id.ToString());
            writer.WriteElementString("Name", Emp.Name);
            writer.WriteElementString("SurName", Emp.Surname);
            writer.WriteStartElement("Age"); writer.WriteValue(Emp.Age); writer.WriteEndElement();
            writer.WriteStartElement("Salary"); writer.WriteValue(Emp.Salary); writer.WriteEndElement();
            writer.WriteElementString("DepartmentId", Emp.DepartmentID.ToString());
            writer.WriteEndElement();
        }
    
        /// <summary>
        /// Возвращает привязку данных. Вариант для департамента.
        /// </summary>
        /// <param name="dbElement">Департамент</param>
        /// <param name="PropertyPath">Путь к данным</param>
        /// <returns></returns>
        public static Binding DBElementbinding(Department dbElement, string PropertyPath)
        {
            Binding binding = new Binding();
            binding.Source = dbElement;
            binding.Path = new System.Windows.PropertyPath(PropertyPath);
            return binding;
        }

        /// <summary>
        /// Возвращает привязку данных. Вариант для сотрудника.
        /// </summary>
        /// <param name="dbElement">Сотрудник</param>
        /// <param name="PropertyPath">Путь к данным</param>
        /// <returns></returns>
        public static Binding DBElementbinding(Employee dbElement, string PropertyPath)
        {
            Binding binding = new Binding();
            binding.Source = dbElement;
            binding.Path = new System.Windows.PropertyPath(PropertyPath);
            return binding;
        }

        public static Binding DBElementbinding(EditingEmployee dbElement, string PropertyPath)
        {
            Binding binding = new Binding();
            binding.Source = dbElement;
            binding.Path = new System.Windows.PropertyPath(PropertyPath);
            return binding;
        }

       public static void WriteXMLElement(XmlWriter writer, string elementName, object value)
       {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
       }



    }
}
