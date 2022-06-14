using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF.BankAccounts
{
    class BankAccountJsonConverter : JsonConverter<BankAccount>
    {
        public override BankAccount ReadJson(JsonReader reader, Type objectType, BankAccount existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, BankAccount value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Number"); writer.WriteValue(value.Number);
            writer.WritePropertyName("OwnerID"); writer.WriteValue(value.Owner.Id);
            writer.WritePropertyName("ProductIDs");
            writer.WriteStartArray();
            foreach (Products.BankProduct bankProduct in value.Products)
            {
                writer.WriteValue(bankProduct.Id);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
