using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrgDB_WPF.BankOperations;

namespace OrgDB_WPF.BankAccounts
{
    // Баланс банковского счёта
    public class BankAccountBalance
    {

        #region Поля

        // Банковский счёт
        BankAccount bankAccount;

        // Текущее состояние счёта
        double balance;

        // Возможен "уход в минус"
        bool overdraftPossible = false;

        // История операций
        SortedList<BankOperation, double> operationsHistory;

        #endregion Поля

        #region Свойства

        // Банковский счёт
        public BankAccount BalanceBankAccount { get { return bankAccount; } }

        // Текущее состояние счёта
        public double Balance { get { return balance; } }

        // Возможен "уход в минус"
        public bool OverdraftPossible { get { return overdraftPossible; } set { overdraftPossible = value; } }

        // История операций
        public SortedList<BankOperation, double> OperationsHistory { get { return operationsHistory; } }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// По банковскому счёту
        /// </summary>
        /// <param name="balanceBankAccount"></param>
        public BankAccountBalance(BankAccount balanceBankAccount)
        {
            bankAccount = balanceBankAccount;
            balance = 0;
            operationsHistory = new SortedList<BankOperation, double>(new BankOperationComparer());
        }

        #endregion Конструкторы

        #region API

        /// <summary>
        /// Применяет операцию к банковскому счёту и добавляет историю операций
        /// </summary>
        /// <param name="bankOperation"></param>
        public void AddBankOperation(BankOperation bankOperation)
        {
            
            // Проверим, можно ли добавить банковскую операцию.
            // Если есть более поздние операции, чем добавляемая, вызываем исключение
            if (operationsHistory.Count > 0)
            {
                IEnumerator<KeyValuePair<BankOperation, double>> en = operationsHistory.GetEnumerator();
                en.MoveNext();

                if (new BankOperationComparer().Compare(bankOperation, en.Current.Key) == 1)
                {
                    throw new Exception("Банковская операция не может быть выполнена, т.к. существуют более поздние операции!");
                }                
            }
            
            ApplyBankOperation(bankOperation);
            operationsHistory.Add(bankOperation, balance);
        }

        #endregion API

        #region Собственные методы

        /// <summary>
        /// Применяет операцию (рассчитывает новое состояние банковского счёта)
        /// </summary>
        /// <param name="bankOperation"></param>
        void ApplyBankOperation(BankOperation bankOperation)
        {
            double operationResult = bankOperation.Calculate(this);
            if (operationResult < 0 && !overdraftPossible) throw new Exception("Операция привела бы к отрицательному состоянию баланса!");
            balance = operationResult;
        }

        #endregion Собственные методы
    }

    class BankOperationComparer : IComparer<BankOperation>
    {
        #region Реализация интерфейса IComparer

        public int Compare(BankOperation x, BankOperation y)
        {
            // сортировка нужна по убыванию дат
            if (x == null && y == null) return 0;
            else if (x == null) return 1;
            else if (y == null) return -1;
            else return y.DateTime.CompareTo(x.DateTime);
        }

        #endregion Реализация интерфейса IComparer
    }
}
