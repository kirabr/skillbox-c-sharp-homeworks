using System;

namespace OrgDB_WPF
{
    /// <summary>
    /// Интерфейс, обеспечивающий наличие свойства Id у объекта
    /// </summary>
    public interface IIdentifyedObject
    {
        Guid Id { get; }
    }
}
