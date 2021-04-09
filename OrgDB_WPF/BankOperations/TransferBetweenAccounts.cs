using OrgDB_WPF.BankAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrgDB_WPF.BankOperations
{
    // Перевод между счетами
    class TransferBetweenAccounts : BankOperation
    {

        #region Поля

        // Сумма перечисления
        double sum;
                
        #endregion Поля

        #region Свойства

        // Сумма перечисления
        double Sum 
        {
            get { return sum; }
            set
            {
                if (value <= 0) throw new Exception("Сумма перечисления должна быть положительной");
                sum = value;
            }
        }

        #endregion Свойства

        #region Конструкторы

        public TransferBetweenAccounts(List<BankAccountBalance> operationAccountBalances, double transferSum)
                    : base(operationAccountBalances) 
        {
            Sum = transferSum;
        }

        #endregion Конструкторы

        #region API

        public override double Calculate(BankAccountBalance bankAccountBalance)
        {
            if (bankAccountBalance == AccountBalances[0])
                return bankAccountBalance.Balance - sum;
            else
                return bankAccountBalance.Balance + sum;

        }

        public override void Apply()
        {
            AccountBalances[0].AddBankOperation(this);
            AccountBalances[1].AddBankOperation(this);
        }
        #region Запись в XML

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            Common.WriteXMLElement(writer, "Sum", Sum);
            writer.WriteEndElement();
        }

        #endregion Запись в XML

        #endregion API

        #region Собственные методы

        #endregion Собственные методы

    }
}
