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

        public static void WriteXmlList<T, T1>(XmlWriter writer, T listT1, string NodeName) where T:List<T1> where T1 : IXmlServices
        {

            if (listT1 == null || listT1.Count == 0) return;

            writer.WriteStartElement(NodeName);
            foreach (T1 t1 in listT1) t1.WriteXml(writer);
            writer.WriteEndElement();
        }

        public static void WriteXmlReadOnlyList<T, T1>(XmlWriter writer, T readOnlyListT1, string NodeName) where T : ReadOnlyCollection<T1> where T1 : IXmlServices
        {
            if (readOnlyListT1 == null || readOnlyListT1.Count == 0) return;

            writer.WriteStartElement(NodeName);
            foreach (T1 t1 in readOnlyListT1) t1.WriteXml(writer);
            writer.WriteEndElement();
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
