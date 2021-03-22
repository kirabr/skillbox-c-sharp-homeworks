using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Products
{
    public class Credit : BankProduct
    {
        #region Поля



        #endregion Поля


        #region Свойства

        #endregion Свойства


        #region Конструкторы
        public Credit(string productName, double productPercentPerYear = 0, double productPricePerYear = 0) : base(productName, productPercentPerYear, productPricePerYear)
        {
        }
        #endregion Конструкторы

    }
}
