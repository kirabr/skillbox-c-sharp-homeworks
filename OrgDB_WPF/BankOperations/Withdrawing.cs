using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.BankOperations
{
    public class Withdrawing : SimpleChangeBalance
    {
        public Withdrawing(List<BankAccounts.BankAccountBalance> operationAccountBalances, double changingSum) 
            : base(operationAccountBalances, changingSum, "Уменьшение счёта на отрицательную сумму невозможно!") { }

        public Withdrawing(BankAccounts.BankAccountBalance operationAccountBalances, double changingSum)
            : this(new List<BankAccounts.BankAccountBalance>() { operationAccountBalances }, changingSum) { }

        public override double Calculate(BankAccounts.BankAccountBalance bankAccountBalance)
        {
            return bankAccountBalance.Balance - Sum;
        }
    }
}
