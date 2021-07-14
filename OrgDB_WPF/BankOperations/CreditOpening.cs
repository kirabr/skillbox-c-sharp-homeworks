using OrgDB_WPF.BankAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF.BankOperations
{
    // Открытие кредита
    public class CreditOpening : BankOperation
    {


        #region Поля

        // Сумма кредита
        double creditSum;

        #endregion Поля

        #region Свойства

        // Сумма кредита
        public double CreditSum { get { return creditSum; } }

        #endregion Свойства

        #region Конструкторы

        public CreditOpening(List<BankAccountBalance> operationAccountBalances, double operationCreditSum) : base(operationAccountBalances)
        {
            CreditStart(operationCreditSum);
        }

        public CreditOpening(BankOperation operationStorno) : base(operationStorno)
        {
        }

        public CreditOpening(List<BankAccountBalance> operationAccountBalances, DateTime operationDateTime, double operationCreditSum) : base(operationAccountBalances, operationDateTime)
        {
            CreditStart(operationCreditSum);
        }

        public CreditOpening(DateTime operationDateTime, BankOperation operationStorno) : base(operationDateTime, operationStorno)
        {
        }

        public CreditOpening(XPathNavigator xPathNavigator) : base(xPathNavigator)
        {
            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//CreditSum");
            if (selectedNode != null) creditSum = selectedNode.ValueAsDouble;
        }

        #endregion Конструкторы

        #region API

        private void CreditStart(double operationCreditSum)
        {
            if (!AccountBalances[0].OverdraftPossible) throw new Exception("Открытие кредита возможно только на баланс с овердрафтом");
            creditSum = operationCreditSum;
        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
        }

        public override double Calculate(BankAccountBalance bankAccountBalance)
        {
            if (IsStorno) return CalculateStorno();

            return AccountBalances[0].Balance - creditSum;
        }
        
        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            Common.WriteXMLElement(writer, "CreditSum", CreditSum);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #endregion API

        #region Собственные методы


        #endregion Собственные методы
    }
}
