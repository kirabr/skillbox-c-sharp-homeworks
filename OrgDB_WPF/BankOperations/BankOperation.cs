using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF
{
    // Банковская операция
    public abstract class BankOperation : IXmlServices, IJsonServices
    {

        #region Поля

        // Идентификатор
        Guid id;

        // Отметка времени
        long ticks;

        // Обслуживаемые операцией балансы счетов
        protected List<BankAccounts.BankAccountBalance> accountBalances = new List<BankAccounts.BankAccountBalance>();

        // Идентификаторы обслуживаемых операцией балансов счетов
        protected List<Guid> accountBalancesIds = new List<Guid>();

        // Признак сторно операции
        bool isStorno;

        // Сторнируемая операция
        BankOperation stornoOperation;

        // Идентификатор сторнируемой операции
        Guid stornoOperationId;

        #endregion Поля

        #region Свойства

        // Идентификатор
        public Guid ID { get { return id; } }

        // Отметка времени
        public long Ticks { get { return ticks; } }

        // Обслуживаемые операцией балансы счетов
        protected ReadOnlyCollection<BankAccounts.BankAccountBalance> AccountBalances { get { return accountBalances.AsReadOnly(); } }

        // Идентификаторы обслуживаемых операцией балансов счетов
        public ReadOnlyCollection<Guid> AccountBalancesIds
        {
            get
            {
                // при первом обращении к свойству заполняем его
                if (accountBalancesIds.Count == 0)
                    foreach (BankAccounts.BankAccountBalance bankAccountBalance in accountBalances) accountBalancesIds.Add(bankAccountBalance.ID);

                return accountBalancesIds.AsReadOnly();

            }
        }

        // Признак сторно операции
        public bool IsStorno { get { return isStorno; } }

        // Сторнируемая операция
        public BankOperation StornoOperation
        {
            get { return stornoOperation; }
            set
            {
                if (value.id != stornoOperationId) throw new Exception("Сторнируемая операция имеет другой ID.");
                stornoOperation = value;
            }
        }

        // Идентификатор сторнируемой операции
        public Guid StornoOperationID
        { get { return stornoOperationId; } }

        #endregion Свойства

        #region Конструкторы

        /// <summary>
        /// Конструктор по банковским балансам
        /// </summary>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances) : this(operationAccountBalances, DateTime.Now)
        { }

        /// <summary>
        /// Конструктор по дате / времени
        /// </summary>
        /// <param name="operationDateTime">Дата / время операции</param>
        public BankOperation(List<BankAccounts.BankAccountBalance> operationAccountBalances, DateTime operationDateTime)
        {
            id = Guid.NewGuid();
            accountBalances = new List<BankAccounts.BankAccountBalance>();
            foreach (BankAccounts.BankAccountBalance bankAccountBalance in operationAccountBalances) accountBalances.Add(bankAccountBalance);
            ticks = operationDateTime.Ticks;
        }

        /// <summary>
        /// Конструктор сторно-операции
        /// </summary>
        /// <param name="operationStorno">Сторнируеамая операция</param>
        public BankOperation(BankOperation operationStorno) : this(DateTime.Now, operationStorno) { }

        /// <summary>
        /// Конструктор сторно-операции по дате / времени и сторнируемой операции
        /// </summary>
        /// <param name="operationDateTime">Дата / время сторно операции (этой операции)</param>
        /// <param name="operationStorno">Сторнируеамая операция</param>
        public BankOperation(DateTime operationDateTime, BankOperation operationStorno)
        {
            id = Guid.NewGuid();
            isStorno = true;
            stornoOperation = operationStorno;
            stornoOperationId = operationStorno.ID;
            ticks = operationDateTime.Ticks;
            accountBalances = operationStorno.accountBalances;
        }

        /// <summary>
        /// Конструктор по XPathNavigator. Используется при загрузке из XML
        /// </summary>
        /// <param name="xPathNavigator"></param>
        public BankOperation(XPathNavigator xPathNavigator)
        {
            id = new Guid(xPathNavigator.GetAttribute("id", ""));

            accountBalances = new List<BankAccounts.BankAccountBalance>();

            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//Ticks");
            if (selectedNode != null)
            {
                ticks = selectedNode.ValueAsLong;

            }

            selectedNode = xPathNavigator.SelectSingleNode("//AccountBalancesIds");
            if (selectedNode != null && selectedNode.MoveToFirstChild())
            {
                accountBalancesIds = new List<Guid>();
                do
                {
                    accountBalancesIds.Add(new Guid(selectedNode.Value));
                } while (selectedNode.MoveToNext());
            }

            selectedNode = xPathNavigator.SelectSingleNode("//IsStorno");
            if (selectedNode != null) isStorno = selectedNode.ValueAsBoolean;

            selectedNode = xPathNavigator.SelectSingleNode("//StornoOperationID");
            if (selectedNode != null) stornoOperationId = new Guid(selectedNode.Value);
        }

        /// <summary>
        /// Конструктор по умолчанию. НЕ УДАЛЯТЬ!
        /// На него может не быть явных ссылок, применяется при загрузке из файлов через Assembly.CreateInstance
        /// </summary>
        public BankOperation() { }

        #endregion Конструкторы

        #region API

        /// <summary>
        /// Применить операцию к банковским балансам
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// Вычисляет результат операции при применении к банковскому балансу
        /// </summary>
        /// <param name="bankAccountBalance">Банковский баланс, к которому применяется операция</param>
        /// <returns>Результат применения банковской операции</returns>
        public abstract double Calculate(BankAccounts.BankAccountBalance bankAccountBalance);

        /// <summary>
        /// Вычисляет результат сторно-операции. Метод применяется только для операций с одним банковским балансом.
        /// Вычисление результата сторно-операции, содержащей более одного банковского баланса реализуется в этих операциях отдельно
        /// </summary>
        /// <returns></returns>
        public double CalculateStorno()
        {
            return CalculateStorno(AccountBalances[0]);
        }

        /// <summary>
        /// Вычисляет результат сторно-операции для банковского баланса
        /// </summary>
        /// <param name="bankAccountBalance">Банковский баланс, к которому применяется сторно-операция</param>
        /// <returns>Результат применения банковской сторно-операции</returns>
        public double CalculateStorno(BankAccounts.BankAccountBalance bankAccountBalance)
        {

            // Найдём операцию (1), предшествующую сторнируемой операции и вернём её (1) результат

            // Все ключи операций
            IList<BankOperation> bankOperations = bankAccountBalance.OperationsHistory.Keys;

            // Индекс ключа операции, предшествующей сторнинуемой операции
            int indKeyBefore = bankOperations.IndexOf(StornoOperation) + 1;

            // Если индекс сторнируемой операции не нашли или это индекс последней операции, возвращаем 0, т.к. нет сторнируемой операции
            if (indKeyBefore == 0 || indKeyBefore > bankOperations.Count - 2) return 0;

            // Возвращаем результат операции, предшествующей сторнируемой
            return AccountBalances[0].OperationsHistory[bankOperations[indKeyBefore]];
        }

        /// <summary>
        /// Добавляет банковский баланс к операции
        /// </summary>
        /// <param name="bankAccountBalance"></param>
        public void AddAccountBalance(BankAccounts.BankAccountBalance bankAccountBalance)
        {
            // У операции не может быть более 2 балансов
            if (AccountBalances.Count > 1)
                throw new Exception("У операции уже более одного баланса, добавление невозможно.");

            accountBalances.Add(bankAccountBalance);

        }

        #region Реализация интерфейса ISerializeServices

        /// <summary>
        /// Заполняет детали операции по JSON DTO представлению.
        /// </summary>
        /// <param name="jBankOperation">JSON DTO представление операции</param>
        public virtual void SetDetails(JObject jBankOperation)
        {
            id = (Guid)jBankOperation.SelectToken("id");
            ticks = (long)jBankOperation.SelectToken("Ticks");
            isStorno = (bool)jBankOperation.SelectToken("IsStorno");
            if (isStorno) stornoOperationId = (Guid)jBankOperation.SelectToken("StornoOperationID");
            JToken jToken = jBankOperation["AccountBalancesIds"];
            if (jToken.HasValues)
            {
                JArray jAccountBalancesIds = (JArray)jToken;
                for (int i = 0; i < jAccountBalancesIds.Count; i++)
                    accountBalancesIds.Add((Guid)jAccountBalancesIds[i]);
            }
        }

        #endregion Реализация интерфейса ISerializeServices

        #region Запись в XML

        /// <summary>
        /// Записывает операцию в XML запись 
        /// </summary>
        /// <param name="writer">XML запись</param>
        abstract public void WriteXml(XmlWriter writer);

        /// <summary>
        /// Записывает общие для всех банковских операций свойства в XML запись
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", ID.ToString());
            Common.WriteXMLElement(writer, "Ticks", ticks);
            writer.WriteStartElement("AccountBalancesIds");
            foreach (BankAccounts.BankAccountBalance bankAccountBalance in AccountBalances)
                writer.WriteElementString("AccountBalanceId", bankAccountBalance.ID.ToString());
            writer.WriteEndElement();
            Common.WriteXMLElement(writer, "IsStorno", IsStorno);
            if (IsStorno)
                writer.WriteElementString("StornoOperationID", StornoOperation.ID.ToString());

        }

        #endregion Запись в XML

        #region Запись в JSON

        /// <summary>
        /// Записывает частные свойства операции в запись JSON
        /// </summary>
        /// <param name="writer"></param>
        public abstract void WriteJsonParticularProperties(JsonWriter writer);

        #endregion Запись в JSON

        #endregion API


    }

}
