using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF.BankAccounts
{
    class BankAccountBalanceJsonConverter : JsonConverter<BankAccountBalance>
    {
        public override BankAccountBalance ReadJson(JsonReader reader, Type objectType, BankAccountBalance existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, BankAccountBalance value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("id"); writer.WriteValue(value.Id);
            writer.WritePropertyName("BankAccountNumber"); writer.WriteValue(value.BankAccount.Number);
            writer.WritePropertyName("Balance"); writer.WriteValue(value.Balance);
            writer.WritePropertyName("OverdraftPossible"); writer.WriteValue(value.OverdraftPossible);

            writer.WritePropertyName("OperationHistory");

            writer.WriteStartArray();
                        
            foreach (KeyValuePair<BankOperation, double> keyValuePair in value.OperationsHistory)
            {

                writer.WriteStartObject();

                writer.WritePropertyName("id"); writer.WriteValue(keyValuePair.Key.Id);
                writer.WritePropertyName("Result"); writer.WriteValue(keyValuePair.Value);

                writer.WriteEndObject();

            }

            writer.WriteEndArray();

            writer.WriteEndObject();

        }
    }
}
