using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrgDB_WPF.Clients;
using OrgDB_WPF.Products;

namespace OrgDB_WPF.BankAccounts
{
    // Банковский счёт
    public class BankAccount
    {

        #region Поля

        // Номер счёта
        string number;

        // Владелец счёта
        Client owner;

        // Банковские продукты счёта. Первый продукт - основной (депозит, кредит). Остальные - сервисные (обслуживание и т.п.)
        List<BankProduct> products;

        #endregion Поля

        #region Свойства

        // Номер счёта
        public string Number { get { return number; } }

        // Владелец счёта
        public Client Owner { get { return owner; } }

        // Банковские продукты счёта. Первый продукт - основной (депозит, кредит). Остальные - сервисные (обслуживание и т.п.)
        public List<BankProduct> Products { get { return products; } set { products = value; } }

        #endregion Свойства

        #region Конструкторы

        public BankAccount(string accountNumber, Client accountOwner, List<BankProduct> accountProducts)
        {
            number = accountNumber;
            owner = accountOwner;
            products = accountProducts;
        }

        #endregion Конструкторы

        #region API

        #endregion API

        #region Собственные методы

        #endregion Собственные методы

    }
}
