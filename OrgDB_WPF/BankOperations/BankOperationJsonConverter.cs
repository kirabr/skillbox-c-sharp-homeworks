using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF
{
    class BankOperationJsonConverter : JsonConverter<BankOperation>
    {
        public override BankOperation ReadJson(JsonReader reader, Type objectType, BankOperation existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, BankOperation value, JsonSerializer serializer)
        {

            writer.WriteStartObject();

            // запишем тип операции
            writer.WritePropertyName("FullTypeName"); writer.WriteValue(value.GetType().FullName);

            // запишем общие для всех операций свойства
            writer.WritePropertyName("id"); writer.WriteValue(value.ID);
            writer.WritePropertyName("Ticks"); writer.WriteValue(value.Ticks);
            writer.WritePropertyName("IsStorno"); writer.WriteValue(value.IsStorno);
            if (value.IsStorno)
            {
                writer.WritePropertyName("StornoOperationID"); writer.WriteValue(value.StornoOperationID);
            }
            writer.WritePropertyName("AccountBalancesIds");
            writer.WriteStartArray();
            foreach (Guid ID in value.AccountBalancesIds) writer.WriteValue(ID);
            writer.WriteEndArray();

            // запишем индивидуальные (для вида операции) свойства
            value.WriteJsonParticularProperties(writer);

            writer.WriteEndObject();
        }
    }
}
