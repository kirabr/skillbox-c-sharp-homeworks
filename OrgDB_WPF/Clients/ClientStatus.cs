namespace OrgDB_WPF.Clients
{
    public abstract class ClientStatus
    {

        #region Поля

        // Название статуса ("Gold", "Silver", "Basic", etc)
        string name;

        // Ранжирование статуса - статутсы ступенью ниже и выше
        ClientStatus previousClientStatus;
        ClientStatus nextClientStatus;

        // Уменьшение базовой ставки по кредиту
        double creditDiscountPercent;

        // Увеличение базовой ставки по вкладу
        double depositAdditionalPercent;

        #endregion Поля

        #region Свойства

        // Название статуса ("Gold", "Silver", "Basic", etc)
        public string Name { get { return name; } set { name = value; } }

        // Ранжирование статуса - статутсы ступенью ниже и выше
        public ClientStatus PreviousClientStatus { get { return previousClientStatus; } set { previousClientStatus = value; } }
        public ClientStatus NextClientStatus { get { return nextClientStatus; } set { nextClientStatus = value; } }

        // Уменьшение базовой ставки по кредиту
        public double CreditDiscountPercent { get { return creditDiscountPercent; } set { creditDiscountPercent = value; } }

        // Увеличение базовой ставки по вкладу
        public double DepositAdditionalPercent { get { return depositAdditionalPercent; } set { depositAdditionalPercent = value; } }

        #endregion Свойства

        #region Конструкторы

        public ClientStatus(string Name)
        {
            name = Name;
        }

        #endregion Конструкторы

    }


}
