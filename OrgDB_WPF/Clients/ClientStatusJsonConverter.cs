using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF.Clients
{
    class ClientStatusJsonConverter : JsonConverter<ClientStatus>
    {
        public override ClientStatus ReadJson(JsonReader reader, Type objectType, ClientStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer,  ClientStatus value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("kind"); writer.WriteValue(value.GetType().Name);
            writer.WritePropertyName("id"); writer.WriteValue(value.ID);
            writer.WritePropertyName("Name"); writer.WriteValue(value.Name);
            writer.WritePropertyName("PreviousClientStatusId"); writer.WriteValue(value.PreviousClientStatusId);
            writer.WritePropertyName("NextClientStatusId"); writer.WriteValue(value.NextClientStatusId);
            writer.WritePropertyName("CreditDiscountPercent"); writer.WriteValue(value.CreditDiscountPercent);
            writer.WritePropertyName("DepositAdditionalPercent"); writer.WriteValue(value.DepositAdditionalPercent);

            writer.WriteEndObject();
        }
    }
}
