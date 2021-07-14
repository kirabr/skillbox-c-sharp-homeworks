using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrgDB_WPF.BankOperations;
using System.Xml;
using System.Xml.XPath;

namespace OrgDB_WPF.BankAccounts
{
    // Баланс банковского счёта
    public class BankAccountBalance : IXmlServices
    {

        #region Поля

        // Идентификатор
        Guid id;

        // Банковский счёт
        BankAccount bankAccount;

        // Текущее состояние счёта
        double balance;

        // Возможен "уход в минус"
        bool overdraftPossible = false;

        // История операций. Ключ - банковская операция, значение - результат операции.
        SortedList<BankOperation, double> operationsHistory;

        #endregion Поля

        #region Свойства

        // Идентификатор
        public Guid ID { get { return id; } }

        // Банковский счёт
        public BankAccount BankAccount { get { return bankAccount; } }

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
            id = Guid.NewGuid();
            balance = 0;
            operationsHistory = new SortedList<BankOperation, double>(new BankOperationComparer());
        }

        public BankAccountBalance(XPathNavigator xPathNavigator, BankAccount bankAccount)
        {
            
            this.bankAccount = bankAccount;

            XPathNavigator selectedNode = xPathNavigator.SelectSingleNode("//@id");
            if (selectedNode != null) id = new Guid(selectedNode.Value);

            selectedNode = xPathNavigator.SelectSingleNode("//Balance");
            if (selectedNode != null) balance = selectedNode.ValueAsDouble;

            selectedNode = xPathNavigator.SelectSingleNode("//OverdraftPossible");
            if (selectedNode != null) overdraftPossible = selectedNode.ValueAsBoolean;

            operationsHistory = new SortedList<BankOperation, double>(new BankOperationComparer());

            selectedNode = xPathNavigator.SelectSingleNode("//OperationsHistory");
            if (selectedNode!=null)
            {

                XPathNodeIterator opHistoryIterator = selectedNode.SelectChildren(XPathNodeType.Element);
                opHistoryIterator.MoveNext(); // OperationsHistory -> OperationHistoryElement

                do
                {
                    
                    // Новый XPathDocument и новый навигатор.
                    // Вот так "... = opHistoryIterator.Current.Clone()" не работет - кэширует позиции строк в XML-документе
                    // и выбирает не уникальные элементы, а первые в серии (кэшированные)
                    XPathNavigator historyElement = new XPathDocument(opHistoryIterator.Current.ReadSubtree()).CreateNavigator();

                    BankOperation bankOperation = null;
                    double bankOperationResult = 0;

                    historyElement.MoveToFirstChild(); // Root -> OperationHistoryElement
                    historyElement.MoveToFirstChild(); // OperationHistoryElement -> BankOperation

                    switch (historyElement.LocalName)
                    {
                        case "AccrualInterestLoan":
                            bankOperation = new AccrualInterestLoan(historyElement);
                            break;
                        case "ChargeForInterest":
                            bankOperation = new ChargeForInterest(historyElement);
                            break;
                        case "CreditOpening":
                            bankOperation = new CreditOpening(historyElement);
                            break;
                        case "Refill":
                            bankOperation = new Refill(historyElement);
                            break;
                        case "TransferBetweenAccounts":
                            bankOperation = new TransferBetweenAccounts(historyElement);
                            break;
                        case "Withdrawing":
                            bankOperation = new Withdrawing(historyElement);
                            break;
                        default:
                            break;
                    }

                    if (historyElement.MoveToNext() && historyElement.LocalName == "OperationResult")
                        bankOperationResult = historyElement.ValueAsDouble;

                    if (bankOperation != null)
                    {
                        operationsHistory.Add(bankOperation, bankOperationResult);
                    }

                } while (opHistoryIterator.MoveNext());

                // Заполним сторно-операции баланса

                // Операции (ключи operationsHistory) выгрузим в List, там будет удобно искать
                List<BankOperation> bankOperations = new List<BankOperation>();
                foreach (BankOperation opHistoryKey in operationsHistory.Keys) bankOperations.Add(opHistoryKey);

                // Найдём все сторно-операции по признаку isStorno и заполним в них указатели на сторнируемые операции
                List<BankOperation> stornoOperations = bankOperations.FindAll(x => x.IsStorno);
                foreach (BankOperation stornoOperation in stornoOperations) 
                    stornoOperation.StornoOperation = bankOperations.Find(x => x.ID == stornoOperation.StornoOperationID);

            }

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
            
            // Если это сторно-операция, то сторнируемая операция должна предшествовать этой. Т.е. сторнируемая операция должна быть последней в списке операций.
            // Не должно быть "разрывов" - операций между сторно и сторнируемой - иначе может потребоваться пересчёт всех последующих операций после сторнируемой.
            // Таким образом индекс сторнируемой операции должен быть 0.
            if (bankOperation.IsStorno)
            {
                // Все ключи операций
                IList<BankOperation> bankOperations = OperationsHistory.Keys;

                // Индекс ключа - сторнинуемой операции
                int indKey = bankOperations.IndexOf(bankOperation.StornoOperation);

                if (indKey != 0) throw new Exception("Сторно-операция может добавляться только сразу после сторнируемой операции!");
            }
            
            ApplyBankOperation(bankOperation);
            operationsHistory.Add(bankOperation, balance);
        }



        #region Запись в XML

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            WriteXmlBasicProperties(writer);
            writer.WriteEndElement();
        }

        public void WriteXmlBasicProperties(XmlWriter writer)
        {
            writer.WriteAttributeString("id", ID.ToString());
            BankAccount.WriteXml(writer);
            Common.WriteXMLElement(writer, "Balance", Balance);
            Common.WriteXMLElement(writer, "OverdraftPossible", OverdraftPossible);
            writer.WriteStartElement("OperationsHistory");
            foreach (KeyValuePair<BankOperation, double> OpHistoryElement in OperationsHistory)
            {
                writer.WriteStartElement("OperationHistoryElement");
                OpHistoryElement.Key.WriteXml(writer);
                Common.WriteXMLElement(writer, "OperationResult", OpHistoryElement.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion Запись в XML


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
            else return y.Ticks.CompareTo(x.Ticks);
        }

        #endregion Реализация интерфейса IComparer
    }
}
