using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Data;
using System.Collections.ObjectModel;

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
        /// Возвращает привязку данных.
        /// </summary>
        /// <param name="dbElement">Элемент привязки - департамент, сотрудник, редактируемый сотрудник, и т.п.</param>
        /// <param name="PropertyPath">Путь к данным</param>
        public static Binding DBElementbinding<T>(T dbElement, string PropertyPath)
        {
            Binding binding = new Binding();
            binding.Source = dbElement;
            binding.Path = new System.Windows.PropertyPath(PropertyPath);
            return binding;
        }

        /// <summary>
        /// Записывает узел XML-элемента (для которого нет штатного метода записи)
        /// </summary>
        /// <param name="writer">Запись XML</param>
        /// <param name="elementName">Строка, име элемента</param>
        /// <param name="value">Значение элемента</param>
        public static void WriteXMLElement(XmlWriter writer, string elementName, object value)
        {
            writer.WriteStartElement(elementName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Записывает коллекцию List элементов произвольного типа (T1), при условии реализации интерфейса IXmlServices
        /// </summary>
        /// <typeparam name="T">Тип - Коллекция List элементов типа T1</typeparam>
        /// <typeparam name="T1">Тип - класс, реализующий интерфейс IXmlServices</typeparam>
        /// <param name="writer">запись XML</param>
        /// <param name="listT1">Коллекция List элементов типа T1</param>
        /// <param name="NodeName">имя узла XML, в который требуется записать коллекцию</param>
        public static void WriteXmlList<T, T1>(XmlWriter writer, T listT1, string NodeName) where T:List<T1> where T1 : IXmlServices
        {

            if (listT1 == null || listT1.Count == 0) return;

            writer.WriteStartElement(NodeName);
            foreach (T1 t1 in listT1) t1.WriteXml(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Записывает ReadOnlyCollection произвольного типа (T1), при условии реализации интерфейса IXmlServices
        /// </summary>
        /// <typeparam name="T">Тип - ReadOnlyCollection элементов типа T1</typeparam>
        /// <typeparam name="T1">Тип - класс, реализующий интерфейс IXmlServices</typeparam>
        /// <param name="writer">запись XML</param>
        /// <param name="readOnlyListT1">ReadOnlyCollection элементов типа T1</param>
        /// <param name="NodeName">имя узла XML, в который требуется записать коллекцию</param>
        public static void WriteXmlReadOnlyList<T, T1>(XmlWriter writer, T readOnlyListT1, string NodeName) where T : ReadOnlyCollection<T1> where T1 : IXmlServices
        {
            if (readOnlyListT1 == null || readOnlyListT1.Count == 0) return;

            writer.WriteStartElement(NodeName);
            foreach (T1 t1 in readOnlyListT1) t1.WriteXml(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Преобразует ReadOnlyCollection в List
        /// </summary>
        /// <typeparam name="T">Тип элемента ReadOnlyCollection</typeparam>
        /// <param name="ro">экземпляр ReadOnlyCollection</param>
        /// <returns></returns>
        public static List<T> ListFromReadOnlyCollection<T>(ReadOnlyCollection<T> ro)
        {
            List<T> result = new List<T>();
            foreach (T elem in ro) result.Add(elem);
            return result;
        }

        /// <summary>
        /// Возвращает "00000000-0000-0000-0000-000000000000" - строковое представление пустого GUID'а
        /// </summary>
        /// <returns></returns>
        public static string EmptyIDString()
        {
            return "00000000-0000-0000-0000-000000000000";
        }

    }
}
