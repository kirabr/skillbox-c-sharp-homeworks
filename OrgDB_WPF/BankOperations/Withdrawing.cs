using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF.BankOperations
{
    // Списание с баланса счёта
    public class Withdrawing : SimpleChangeBalance
    {


        #region Поля

        #endregion Поля

        #region Свойства

        #endregion Свойства

        #region Конструкторы
        public Withdrawing(List<BankAccounts.BankAccountBalance> operationAccountBalances, double changingSum)
                    : base(operationAccountBalances, changingSum, "Уменьшение счёта на отрицательную сумму невозможно!") { }

        public Withdrawing(BankAccounts.BankAccountBalance operationAccountBalances, double changingSum)
            : this(new List<BankAccounts.BankAccountBalance>() { operationAccountBalances }, changingSum) { }

        public Withdrawing(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        #endregion Конструкторы

        #region API
        public override double Calculate(BankAccounts.BankAccountBalance bankAccountBalance)
        {
            if (IsStorno) return CalculateStorno();

            return AccountBalances[0].Balance - Sum;
        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
        }

        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlSCBProperties(writer);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #endregion API

        #region Собственные методы

        #endregion Собственные методы


    }
}
