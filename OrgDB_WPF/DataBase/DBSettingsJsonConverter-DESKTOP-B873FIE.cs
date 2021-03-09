using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF
{
    class DBSettingsJsonConverter : JsonConverter<DBSettings>
    {

        #region Реализация JsonConverter

        /// <summary>
        /// Чтение JSON, не используем здесь. Читаем через токены в DataBase
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override DBSettings ReadJson(JsonReader reader, Type objectType, DBSettings existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Запись настроек в JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, DBSettings value, JsonSerializer serializer)
        {

            writer.WriteStartObject();

            writer.WritePropertyName("ManagerSalaryPercent"); writer.WriteValue(value.ManagerSalaryPercent);
            writer.WritePropertyName("dbFilePath"); writer.WriteValue(value.DBFilePath);
            writer.WritePropertyName("MinManagerSalary"); writer.WriteValue(value.MinManagerSalary);
            writer.WritePropertyName("MinSpecSalary"); writer.WriteValue(value.MinSpecSalary);
            writer.WritePropertyName("MinInternSalary"); writer.WriteValue(value.MinInternSalary);

            writer.WriteEndObject();

        }

        #endregion Реализация JsonConverter
    }
}
