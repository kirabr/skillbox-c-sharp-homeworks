using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgDB_WPF.BankOperations
{
    public abstract class BankOperation
    {

        #region Поля

        // Дата и время операции
        DateTime dateTime;

        // Обслуживаемые операцией балансы счетов
        protected List<BankAccounts.BankAccountBalance> accountBalances;

        // Признак сторно операции
        bool isStorno;
        
        // Сторнируемая операция
        BankOperation stornoOperation;

        #endregion Поля

        #region Свойства

        // Дата и время операции
        public DateTime DateTime { get { return dateTime; } }

        // Обслуживаемые операцией балансы счетов
        List<BankAccounts.BankAccountBalance> AccountBalances { get { return accountBalances; } }

        // Признак сторно операции
        public bool IsStorno { get { return isStorno; } }

        // Сторнируемая операция
        public BankOperation StornoOperation { get { return stornoOperation; } }
        
        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор по банковским операциям
        /// </summary>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances) : this(operationAccountBalances, DateTime.Now) { }

        /// <summary>
        /// Конструктор по дате / времени
        /// </summary>
        /// <param name="operationDateTime">Дата / время операции</param>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances, DateTime operationDateTime)
        {
            dateTime = operationDateTime;
        }

        /// <summary>
        /// Конструктор сторно-операции
        /// </summary>
        /// <param name="operationStorno">Сторнируеамая операция</param>
        public BankOperation(BankOperation operationStorno):this(DateTime.Now, operationStorno) { }

        /// <summary>
        /// Конструктор сторно-операции по дате / времени и сторнируемой операции
        /// </summary>
        /// <param name="operationDateTime">Дата / время сторно операции (этой операции)</param>
        /// <param name="operationStorno">Сторнируеамая операция</param>
        public BankOperation(DateTime operationDateTime, BankOperation operationStorno)
        {
            dateTime = operationDateTime;
            isStorno = true;
            stornoOperation = operationStorno; 
        }

        #endregion Конструкторы

        #region API

        public abstract double Calculate(BankAccounts.BankAccountBalance bankAccountBalance);

        #endregion API

        #region Собственные методы

        #endregion Собственные методы


        
    }

}
