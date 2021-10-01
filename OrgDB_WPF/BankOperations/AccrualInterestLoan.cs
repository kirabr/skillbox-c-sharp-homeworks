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
    // Начисление процентов по кредиту
    class AccrualInterestLoan : BankOperation
    {


        #region Поля

        #endregion Поля

        #region Свойства

        #endregion Свойства

        #region Конструкторы
        public AccrualInterestLoan(List<BankAccountBalance> operationAccountBalances) : base(operationAccountBalances)
        {
        }

        public AccrualInterestLoan(BankOperation operationStorno) : base(operationStorno)
        {
        }

        public AccrualInterestLoan(List<BankAccountBalance> operationAccountBalances, DateTime operationDateTime) : base(operationAccountBalances, operationDateTime)
        {
        }

        public AccrualInterestLoan(DateTime operationDateTime, BankOperation operationStorno) : base(operationDateTime, operationStorno)
        {
        }

        public AccrualInterestLoan(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        //public AccrualInterestLoan(JObject jBankOperation) : base(jBankOperation) { }

        public AccrualInterestLoan() { }

        #endregion Конструкторы

        #region API

        public override double Calculate(BankAccountBalance bankAccountBalance)
        {

            if (IsStorno) return CalculateStorno();
            
            // Все ключи операций
            IList<BankOperation> bankOperations = AccountBalances[0].OperationsHistory.Keys;

            // Ближайшая в прошлом операция выдачи кредита - ключ к получению суммы для начислений процентов
            CreditOpening KeyOperation = null;
            foreach (BankOperation bankOperation in bankOperations)
            {
                if (bankOperation.GetType() == typeof(CreditOpening))
                {
                    KeyOperation = (CreditOpening)bankOperation;
                    break;
                }
            }
            if (KeyOperation == null) throw new Exception("Не найдена операция выдачи кредита, начисление процентов невозможно!");

            // Баланс для начисления процентов
            double SumToInterest = KeyOperation.CreditSum;

            // Начисление процентов
            return SumToInterest - bankAccountBalance.BankAccount.Products[0].BasicPercentPerYear / 1200;
        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
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
