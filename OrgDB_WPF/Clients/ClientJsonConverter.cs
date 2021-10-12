using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF.Clients
{
    class ClientJsonConverter : JsonConverter<Client>
    {
        public override Client ReadJson(JsonReader reader, Type objectType, Client existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Client value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("FullTypeName"); writer.WriteValue(value.GetType().FullName);
            writer.WritePropertyName("id"); writer.WriteValue(value.ID);
            writer.WritePropertyName("Name"); writer.WriteValue(value.Name);
            writer.WritePropertyName("ClientManagerId"); writer.WriteValue(value.ClientManagerId);
            writer.WritePropertyName("IsResident"); writer.WriteValue(value.IsResident);
            writer.WritePropertyName("ClientStatusId"); writer.WriteValue(value.ClientStatusId);
            value.WriteJsonParticularProperties(writer);

            writer.WriteEndObject();        }
    }
}
