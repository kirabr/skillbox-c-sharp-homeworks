using Newtonsoft.Json.Linq;

namespace OrgDB_WPF
{
    /// <summary>
    /// Интерфейс для работы объектов сборки с JSON
    /// </summary>
    public interface IJsonServices
    {

        /// <summary>
        /// Заполняет объект по данным jObject
        /// </summary>
        /// <param name="jObject">Json DTO - представление объекта</param>
        void SetDetails(JObject jObject);
    }

}
