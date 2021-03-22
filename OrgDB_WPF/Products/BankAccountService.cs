using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.Products
{
    public class BankAccountService : BankProduct
    {
        public BankAccountService(string productName, double productPercentPerYear = 0, double productPricePerYear = 0) : base(productName, productPercentPerYear, productPricePerYear)
        {
        }
    }
}
