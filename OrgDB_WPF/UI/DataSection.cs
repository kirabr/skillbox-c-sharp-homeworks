using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrgDB_WPF.UI
{
    struct DataSection
    {
        public string name;
        public System.Windows.Controls.MenuItem menuItem;
        public List<FrameworkElement> elements;
    }

    class DataSections
    {
        private List<DataSection> dataSections = new List<DataSection>();

        // индексатор по имени
        public DataSection this[string name]
        {
            get { return dataSections.Find(x => x.name == name); }
            set 
            {

                int Index = dataSections.IndexOf(dataSections.Find(x => x.name == name));
                if (Index == -1)
                {
                    
                    dataSections.Add(value);                    
                }
                else
                {
                    dataSections[Index] = value;
                }
 
            }
        }

        // индексатор по элементу меню
        public DataSection this[System.Windows.Controls.MenuItem menuItem]
        {
            get
            {
                return dataSections.Find(x => x.menuItem == menuItem);
            }
        }

        // Код ниже - Локальная реализация IEnumerable<T> Interface
        // для возможности использовать оператор foreach для перебора
        // элементов коллекции DataSections.
        // Пример реализации см. здесь: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs1579?f1url=%3FappId%3Droslyn%26k%3Dk(CS1579)
        // Документация здесь: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement
        // и здесь: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=net-5.0

        /// <summary>
        /// Собственное расширение GetEnumerator()
        /// </summary>
        /// <returns></returns>
        public DataSectionEnumerator GetEnumerator()
        {
            return new DataSectionEnumerator(this);
        }

        /// <summary>
        /// Перебирает элементы коллекции DataSections
        /// </summary>
        public class DataSectionEnumerator
        {
            int nIndex;
            DataSections collection;
            
            public DataSectionEnumerator(DataSections dataSections)
            {
                collection = dataSections;
                nIndex = -1;
            }

            public bool MoveNext()
            {
                nIndex++;
                return nIndex < collection.dataSections.Count;
            }

            public DataSection Current => collection.dataSections[nIndex];
        }
    }
}
