using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Products
{
    public abstract class BankProduct
    {
        #region Поля

        // Наименование продукта
        string name;

        // Описание продукта
        string description;

        // Базовая процентная годовая ставка
        double basicPercentPerYear;

        // Стоимость продукта (например, обслуживания счета)
        double basicPricePerYear;

        #endregion Поля

        #region Свойства

        // Наименование продукта
        public string Name { get { return name; } set { name = value; } }

        // Описание продукта
        public string Description { get { return description; } set { description = value; } }

        // Базовая процентная годовая ставка
        public double BasicPercentPerYear { get { return basicPercentPerYear; } set { basicPercentPerYear = value; } }

        // Стоимость продукта (например, обслуживания счета)
        public double BasicPrice { get { return basicPricePerYear; } set { basicPricePerYear = value; } }

        #endregion Свойства

        #region Конструкторы

        public BankProduct(string productName, double productPercentPerYear = 0, double productPricePerYear = 0)
        {
            name = productName;
            basicPercentPerYear = productPercentPerYear;
            basicPricePerYear = productPricePerYear;
        }

        #endregion Конструкторы

        #region API

        #endregion API

        #region Собственные методы

        #endregion Собственные методы

    }
}
