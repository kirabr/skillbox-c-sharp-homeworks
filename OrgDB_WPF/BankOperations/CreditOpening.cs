using OrgDB_WPF.BankAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF.BankOperations
{
    // Открытие кредита
    public class CreditOpening : BankOperation
    {

        // Сумма кредита
        double creditSum;

        public double CreditSum { get { return creditSum; } }
        
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
    }
}
