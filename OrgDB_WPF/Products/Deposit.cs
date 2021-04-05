using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Products
{
    public class Deposit : BankProduct
    {

        #region Поля

        // Капитализация
        bool hasCapitalization = false;

        #endregion Поля

        #region Свойства

        // Капитализация
        public bool HasCapitalization { get { return hasCapitalization; } }

        #endregion Свойства

        #region Конструкторы
        public Deposit(string productName, double productPercentPerYear = 0, double productPricePerYear = 0, bool hasCap = false) : base(productName, productPercentPerYear, productPricePerYear)
        {
            hasCapitalization = hasCap;
        }
        #endregion Конструкторы

    }
}
