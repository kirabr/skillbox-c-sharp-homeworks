using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.BankOperations
{
    // Пополненение баланса счёта
    public class Refill : SimpleChangeBalance
    {
        
        public Refill(List<BankAccounts.BankAccountBalance> operationAccountBalances, double changingSum) 
            : base(operationAccountBalances, changingSum, "Пополнение счёта на отрицательную сумму невозможно!") {}

        public Refill(BankAccounts.BankAccountBalance operationAccountBalances, double changingSum)
            : this(new List<BankAccounts.BankAccountBalance>() { operationAccountBalances }, changingSum) { }

        
        public override double Calculate(BankAccounts.BankAccountBalance bankAccountBalance)
        {
            return AccountBalances[0].Balance + Sum;
        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
        }


    }
}
