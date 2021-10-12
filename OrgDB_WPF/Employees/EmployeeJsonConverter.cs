using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF
{
    class EmployeeJsonConverter : JsonConverter<Employee>
    {

        #region Реализация JsonConverter
        /// <summary>
        /// Чтение из JSON. Здесь не используется, читаем по токенам в DataBase
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override Employee ReadJson(JsonReader reader, Type objectType, Employee existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Записывает сотрудника в JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, Employee value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            // имя класса для определения при загрузке
            writer.WritePropertyName("FullTypeName"); writer.WriteValue(value.GetType().FullName);
            
            // свойства
            writer.WritePropertyName("id"); writer.WriteValue(value.Id);
            writer.WritePropertyName("Name"); writer.WriteValue(value.Name);
            writer.WritePropertyName("Surname"); writer.WriteValue(value.Surname);
            writer.WritePropertyName("Age"); writer.WriteValue(value.Age);
            writer.WritePropertyName("Salary"); writer.WriteValue(value.Salary);
            writer.WritePropertyName("DepartmentID"); writer.WriteValue(value.DepartmentID);
            writer.WritePropertyName("Post"); writer.WriteValue((int)value.post_Enum);

            writer.WriteEndObject();
        }
        
        #endregion Реализация JsonConverter
    }
}
