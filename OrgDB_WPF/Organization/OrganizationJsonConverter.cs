using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrgDB_WPF
{
    class OrganizationJsonConverter : JsonConverter<Organization>
    {
        public override Organization ReadJson(JsonReader reader, Type objectType, Organization existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Organization value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("ManagerSalaryPercent"); writer.WriteValue(value.ManagerSalaryPercent);
            writer.WritePropertyName("MinManagerSalary"); writer.WriteValue(value.MinManagerSalary);
            writer.WritePropertyName("MinSpecSalary"); writer.WriteValue(value.MinSpecSalary);
            writer.WritePropertyName("MinInternSalary"); writer.WriteValue(value.MinInternSalary);

            writer.WriteEndObject();
        }
    }
}
