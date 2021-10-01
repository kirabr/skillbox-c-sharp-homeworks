using OrgDB_WPF.BankAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF.BankOperations
{
    // Начисление процентов (по депозиту)
    public class ChargeForInterest : BankOperation
    {


        #region Поля

        #endregion Поля

        #region Свойства

        #endregion Свойства

        #region Конструкторы
        public ChargeForInterest(List<BankAccountBalance> operationAccountBalances) : base(operationAccountBalances)
        {
        }

        public ChargeForInterest(BankAccountBalance operationAccountBalance) : base(new List<BankAccountBalance>() { operationAccountBalance }) { }

        public ChargeForInterest(BankOperation operationStorno) : base(operationStorno)
        {
        }

        public ChargeForInterest(List<BankAccountBalance> operationAccountBalances, DateTime operationDateTime) : base(operationAccountBalances, operationDateTime)
        {
        }

        public ChargeForInterest(DateTime operationDateTime, BankOperation operationStorno) : base(operationDateTime, operationStorno)
        {
        }

        public ChargeForInterest(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        //public ChargeForInterest(JObject jBankOperation) : base(jBankOperation) { }

        public ChargeForInterest() { }

        #endregion Конструкторы

        #region API

        public override double Calculate(BankAccountBalance bankAccountBalance)
        {

            if (IsStorno) return CalculateStorno();

            Products.Deposit deposit = (Products.Deposit)bankAccountBalance.BankAccount.Products[0];

            // Если основной банковский продукт с капитализацией, то просто начисляем процент на актуальное состояние баланса
            if (deposit.HasCapitalization)
                return bankAccountBalance.Balance + (bankAccountBalance.Balance * deposit.BasicPercentPerYear / 1200);

            // Если без капитализации - найдём минимальное состояние баланса до первого применения операции и начислим процент на него
            double balanceToCharge = bankAccountBalance.Balance;

            IEnumerator<KeyValuePair<BankOperation, double>> en = bankAccountBalance.OperationsHistory.GetEnumerator();
            if (en.MoveNext())
            {
                // Первая операция начисления процентов - ключ для сдвига к балансу до этой операции
                BankOperation KeyBefore = en.Current.Key;

                // Признак наличия в истории таких же операций
                bool HasSameOperation = en.Current.Key.GetType() == GetType();

                // Двигаемся по истории в прошлое, ищем такие же операции.
                // Если нашли, переустанавливаем операцию-ключ
                while (en.MoveNext())
                {
                    if (en.Current.Key.GetType() == GetType())
                    {
                        KeyBefore = en.Current.Key;
                        HasSameOperation = true;
                    }
                }

                // Если в истории есть такие же операции, позиционируемся на моменте перед первой такой операцией и берём баланс из этого момента.
                if (HasSameOperation)
                {
                    en.Reset();
                    while (en.MoveNext() && en.Current.Key != (BankOperation)KeyBefore) { balanceToCharge = Math.Min(balanceToCharge, en.Current.Value); }
                    if (en.MoveNext()) balanceToCharge = Math.Min(balanceToCharge, en.Current.Value);
                }

            }

            return bankAccountBalance.Balance + (balanceToCharge * deposit.BasicPercentPerYear / 1200);

        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
        }

        new public void SetDetails(JObject jBankOperation)
        {

        }

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #region Запись в JSON

        public override void WriteJsonParticularProperties(JsonWriter writer)
        {

        }

        #endregion Запись в JSON

        #endregion API

        #region Собственные методы


        #endregion Собственные методы
    }
}
