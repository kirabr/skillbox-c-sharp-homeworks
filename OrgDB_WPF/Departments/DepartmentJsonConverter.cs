using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF
{
    class DepartmentJsonConverter : JsonConverter<Department>
    {

        #region Реализация JsonConverter
        
        /// <summary>
        /// Чтение JSON. Здесь не используется, читаем через токены в DataBase
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override Department ReadJson(JsonReader reader, Type objectType, Department existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Запись департамента в JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, Department value, JsonSerializer serializer)
        {

            writer.WriteStartObject();

            writer.WritePropertyName("id"); writer.WriteValue(value.id);
            writer.WritePropertyName("Name"); writer.WriteValue(value.Name);
            writer.WritePropertyName("Location"); writer.WriteValue(value.Location);
            writer.WritePropertyName("ParentId"); writer.WriteValue(value.ParentId);

            writer.WriteEndObject();

        }

        #endregion Реализация JsonConverter

    }
}
