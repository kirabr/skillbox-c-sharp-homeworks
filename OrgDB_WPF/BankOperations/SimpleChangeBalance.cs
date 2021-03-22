using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.BankOperations
{
    public abstract class SimpleChangeBalance : BankOperation
    {

        #region Поля

        // Сумма пополнения
        double sum = 0;

        // Текст исключения некорректной суммы
        protected string unCorrectSumExceptionText;

        #endregion Поля

        #region Свойства

        // Сумма пополнения
        public double Sum 
        { 
            get { return sum; }
            set 
            {
                if (value < 0) throw new Exception(unCorrectSumExceptionText);
                sum = value;
            } 
        }

        // Текст исключения некорректной суммы
        //protected abstract string UnCorrectSumExceptionText { set; }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор по сумме изменения
        /// </summary>
        /// <param name="ChangeSum">Сумма изменения</param>
        protected SimpleChangeBalance(List<BankAccounts.BankAccountBalance> operationAccountBalances, double ChangeSum)
            : this(operationAccountBalances, ChangeSum, "Изменение счёта на отрицательную величину недопустимо!"){}

        /// <summary>
        /// Конструктор по сумме изменения и тексту ошибки суммы изменения
        /// </summary>
        /// <param name="ChangeSum"></param>
        /// <param name="UnCorrectSumExceptionText"></param>
        protected SimpleChangeBalance(List<BankAccounts.BankAccountBalance> operationAccountBalances, double ChangeSum, string UnCorrectSumExceptionText)
            : base(operationAccountBalances)
        {
            this.unCorrectSumExceptionText = UnCorrectSumExceptionText;
            this.Sum = ChangeSum;
            
        }

        #endregion Конструкторы

        #region API
        
        #endregion API

    }
}
