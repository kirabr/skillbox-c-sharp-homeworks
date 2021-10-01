using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF.Products
{
    class BankProductJsonConverter : JsonConverter<BankProduct>
    {
        public override BankProduct ReadJson(JsonReader reader, Type objectType, BankProduct existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, BankProduct value, JsonSerializer serializer)
        {

            writer.WriteStartObject();

            writer.WritePropertyName("kind"); writer.WriteValue(value.GetType().Name);
            writer.WritePropertyName("id"); writer.WriteValue(value.ID);
            writer.WritePropertyName("Name"); writer.WriteValue(value.Name);
            writer.WritePropertyName("Description"); writer.WriteValue(value.Description);
            writer.WritePropertyName("BasicPercentPerYear"); writer.WriteValue(value.BasicPercentPerYear);
            writer.WritePropertyName("BasicPrice"); writer.WriteValue(value.BasicPrice);
            value.WriteJsonParticularProperties(writer);

            writer.WriteEndObject();
        }
    }
}
