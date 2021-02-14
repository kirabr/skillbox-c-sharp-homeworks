using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;

namespace OrgDB_WPF
{
    public class EmployeePostDescriptionConverter
    {
        
        public static string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
                return enumObj.ToString();
            else
            {
                DescriptionAttribute attrib = null;

                foreach (var att in attribArray)
                {
                    if (att is DescriptionAttribute)
                    {
                        attrib = att as DescriptionAttribute;
                        break;
                    }
                }

                if (attrib != null)
                    return attrib.Description;

                return enumObj.ToString();

            }

        }
        
    }
}
