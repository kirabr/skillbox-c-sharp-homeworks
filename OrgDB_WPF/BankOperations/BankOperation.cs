﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OrgDB_WPF.BankOperations
{
    // Банковская операция
    public abstract class BankOperation : IXmlServices
    {

        #region Поля

        // Идентификатор
        Guid id;

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

        // Идентификатор
        public Guid ID { get { return id; } }

        // Дата и время операции
        public DateTime DateTime { get { return dateTime; } }

        // Обслуживаемые операцией балансы счетов
        protected List<BankAccounts.BankAccountBalance> AccountBalances { get { return accountBalances; } }

        // Признак сторно операции
        public bool IsStorno { get { return isStorno; } }

        // Сторнируемая операция
        public BankOperation StornoOperation { get { return stornoOperation; } }
        
        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор по банковским балансам
        /// </summary>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances) : this(operationAccountBalances, DateTime.Now) { }

        /// <summary>
        /// Конструктор по дате / времени
        /// </summary>
        /// <param name="operationDateTime">Дата / время операции</param>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances, DateTime operationDateTime)
        {
            id = new Guid();
            accountBalances = new List<BankAccounts.BankAccountBalance>();
            foreach (BankAccounts.BankAccountBalance bankAccountBalance in operationAccountBalances) AccountBalances.Add(bankAccountBalance);
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
            id = new Guid();
            dateTime = operationDateTime;
            isStorno = true;
            stornoOperation = operationStorno; 
        }

        #endregion Конструкторы

        #region API

        public abstract void Apply();

        public abstract double Calculate(BankAccounts.BankAccountBalance bankAccountBalance);

        #region Запись в XML
        abstract public void WriteXml(XmlWriter writer);

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", ID.ToString());
            Common.WriteXMLElement(writer, "DateTime", DateTime);
            writer.WriteStartElement("AccountBalancesIds");
            foreach (BankAccounts.BankAccountBalance bankAccountBalance in AccountBalances)
                writer.WriteElementString("AccountBalanceId", bankAccountBalance.ID.ToString());
            writer.WriteEndElement();
            Common.WriteXMLElement(writer, "IsStorno", IsStorno);
            if (IsStorno)
                writer.WriteElementString("StornoOperationID", StornoOperation.ID.ToString());

        }

        #endregion Запись в XML

        #endregion API

        #region Собственные методы

        #endregion Собственные методы



    }

}