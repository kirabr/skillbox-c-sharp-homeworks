using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF.BankOperations
{
    // Пополненение баланса счёта
    public class Refill : SimpleChangeBalance
    {


        #region Поля

        #endregion Поля

        #region Свойства

        #endregion Свойства

        #region Конструкторы
        public Refill(List<BankAccounts.BankAccountBalance> operationAccountBalances, double changingSum)
                    : base(operationAccountBalances, changingSum, "Пополнение счёта на отрицательную сумму невозможно!") { }

        public Refill(BankAccounts.BankAccountBalance operationAccountBalances, double changingSum)
            : this(new List<BankAccounts.BankAccountBalance>() { operationAccountBalances }, changingSum) { }

        public Refill(XPathNavigator xPathNavigator) : base(xPathNavigator) { }

        #endregion Конструкторы

        #region API
        public override double Calculate(BankAccounts.BankAccountBalance bankAccountBalance)
        {
            if (IsStorno) return CalculateStorno();

            return AccountBalances[0].Balance + Sum;
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
