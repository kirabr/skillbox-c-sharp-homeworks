using System;
using System.Collections.Generic;
using OrgDB_WPF.Products;
using OrgDB_WPF.Clients;
using OrgDB_WPF.BankAccounts;
using System.Net.NetworkInformation;

namespace OrgDB_WPF
{
    internal class Tests
    {
        
        #region Поля

        DataBase db;
        Random random = new Random();
        Dictionary<string, Guid> testObjects = new Dictionary<string, Guid>();
        Dictionary<string, BankAccount> testBankAccounts = new Dictionary<string, BankAccount>();

        #endregion Поля

        #region Свойства

        public DataBase DB { get { return db; } set { db = value; } }

        #endregion Свойства

        #region Конструкторы

        public Tests(DataBase dataBase) 
        { 
            db = dataBase; 
        }

        #endregion Конструкторы

        #region Тестовые данные

        /// <summary>
        /// Создаёт департаменты с иерархией
        /// </summary>
        public void AddTestDepartments() 
        {

            Department d_1 = new Department("d_1", "Podolsk", Guid.Empty); DB.AddDepartment(d_1); testObjects.Add("Departments.d_1", d_1.id);
            Department d_2 = new Department("d_2", "Podolsk", Guid.Empty); DB.AddDepartment(d_2); testObjects.Add("Departments.d_2", d_2.id);

            Department d_1_1 = new Department("d_1_1", "Orehovo", d_1.id); DB.AddDepartment(d_1_1); testObjects.Add("Departments.d_1_1", d_1_1.id);
            Department d_1_2 = new Department("d_1_2", "Borisovo", d_1.id); DB.AddDepartment(d_1_2); testObjects.Add("Departments.d_1_2", d_1_2.id);

            Department d_1_1_1 = new Department("d_1_1_1", "Kukuevo", d_1_1.id); DB.AddDepartment(d_1_1_1); testObjects.Add("Departments.d_1_1_1", d_1_1_1.id);
            Department d_1_1_2 = new Department("d_1_1_2", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_2); testObjects.Add("Departments.d_1_1_2", d_1_1_2.id);
            Department d_1_1_3 = new Department("d_1_1_3", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_3); testObjects.Add("Departments.d_1_1_3", d_1_1_3.id);

            Department d_1_1_1_1 = new Department("d_1_1_1_1", "Zvezduevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_1); testObjects.Add("Departments.d_1_1_1_1", d_1_1_1_1.id);
            Department d_1_1_1_2 = new Department("d_1_1_1_2", "Yasenevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_2); testObjects.Add("Departments.d_1_1_1_2", d_1_1_1_2.id);

        }        

        /// <summary>
        /// Для каждого департамента создаёт по одному интерну, специалисту и менеджеру
        /// </summary>
        public void AddTestEmployees() 
        {

            // 1. Определяем глубину дерева департаментов.
            // 2. Обходим элементы-департаменты, начиная с максимальной глубины, заканчивая верхнеуровневыми.
            // Для каждого элемента рассчитывем зарплату управляющего, добавляем по одному интерну, специалисту и управляющему.

            // Департаменты и их уровни
            List<depLevel> depLevels = new List<depLevel>();
            // Максимальная глубина уровней департаментов
            int maxLevel = 0;

            foreach (Department dep in db.Departments)
            {
                int level = db.DepartmentLevel(dep);
                depLevels.Add(new depLevel(dep, level));
                maxLevel = Math.Max(maxLevel, level);
            }

            for(int i = maxLevel; i > -1; i--)
            {
                List<depLevel> curDepartmentsLevel = depLevels.FindAll(x => x.Level == i);
                foreach(depLevel DepLevel in curDepartmentsLevel)
                {
                    string[] NameSurname = GenerateHumanFullName().Split(' ');

                    Intern intern = new Intern(
                        NameSurname[1],
                        NameSurname[0],
                        random.Next(18, 30),
                        random.Next(100, 300),
                        DepLevel.Department.id);
                    db.AddEmployee(intern);
                    testObjects.Add($"Employees.InternOf.{DepLevel.Department.Name}", intern.Id);

                    NameSurname = GenerateHumanFullName().Split(' ');

                    NameSurname = GenerateHumanFullName().Split(' ');

                    Specialist specialist = new Specialist(
                        NameSurname[1],
                        NameSurname[0],
                        random.Next(20, 60),
                        random.Next(intern.Salary + 100, intern.Salary + 300),
                        DepLevel.Department.id);
                    db.AddEmployee(specialist);
                    testObjects.Add($"Employees.SpecialistOf.{DepLevel.Department.Name}", intern.Id);

                    NameSurname = GenerateHumanFullName().Split(' ');

                    NameSurname = GenerateHumanFullName().Split(' ');

                    int managerSalary = db.CalculateManagerSalary(DepLevel.Department);
                    Manager manager = new Manager(
                        NameSurname[1],
                        NameSurname[0],
                        random.Next(18, 60), 
                        managerSalary, 
                        DepLevel.Department.id);
                    db.AddEmployee(manager);
                    testObjects.Add($"Employees.ManagerOf.{DepLevel.Department.Name}", intern.Id);

                }
            }
        
        }
                
        /// <summary>
        /// Добавляет цепочки статусов (Начинающий [партнёр] - постоянный [партнёр] - опытный [партнёр])
        /// клиентов - юридических и физических лиц
        /// </summary>
        public void AddClientStatuses()
        {
            ClientStatus individualBeginer = new IndividualStatus("Начинающий");
            individualBeginer.CreditDiscountPercent = 0;
            individualBeginer.DepositAdditionalPercent = 0;
            db.AddClientStatus(individualBeginer);
            testObjects.Add("ClientStatuses.individualBeginer", individualBeginer.Id);

            ClientStatus individualRegular = new IndividualStatus("Постоянный");
            individualRegular.PreviousClientStatus = individualBeginer;
            individualBeginer.NextClientStatus = individualRegular;
            individualRegular.CreditDiscountPercent = 3;
            individualRegular.DepositAdditionalPercent = 3;
            db.AddClientStatus(individualRegular);
            testObjects.Add("ClientStatuses.individualRegular", individualRegular.Id);

            ClientStatus individualMaster = new IndividualStatus("Опытный");
            individualMaster.PreviousClientStatus = individualRegular;
            individualRegular.NextClientStatus = individualMaster;
            individualMaster.CreditDiscountPercent = 5;
            individualMaster.DepositAdditionalPercent = 5;
            db.AddClientStatus(individualMaster);
            testObjects.Add("ClientStatuses.individualMaster", individualMaster.Id);

            LegalEntityStatus legalEntityBeginer = new LegalEntityStatus("Начинающий партнёр");
            legalEntityBeginer.CreditDiscountPercent = 2;
            legalEntityBeginer.DepositAdditionalPercent = 2;
            db.AddClientStatus(legalEntityBeginer);
            testObjects.Add("ClientStatuses.legalEntityBeginer", legalEntityBeginer.Id);

            LegalEntityStatus legalEntityRegular = new LegalEntityStatus("Постоянный партнёр");
            legalEntityRegular.PreviousClientStatus=legalEntityBeginer;
            legalEntityBeginer.NextClientStatus = legalEntityRegular;
            legalEntityRegular.CreditDiscountPercent = 4;
            legalEntityRegular.DepositAdditionalPercent = 4;
            db.AddClientStatus(legalEntityRegular);
            testObjects.Add("ClientStatuses.legalEntityRegular", legalEntityRegular.Id);

            LegalEntityStatus legalEntityMaster = new LegalEntityStatus("Опытный партнёр");
            legalEntityMaster.PreviousClientStatus = legalEntityRegular;
            legalEntityRegular.NextClientStatus = legalEntityMaster;
            legalEntityMaster.CreditDiscountPercent = 6;
            legalEntityMaster.DepositAdditionalPercent = 6;
            db.AddClientStatus(legalEntityMaster);
            testObjects.Add("ClientStatuses.legalEntityMaster", legalEntityMaster.Id);
        
        }

        /// <summary>
        /// Добавляет банковские продукты - депозиты (без и с капитализацией), кредит, обслуживание счёта
        /// </summary>
        public void AddBankProducts()
        {
            Deposit deposit = new Deposit("Мастер годового дохода", 5, 10);
            Deposit capDeposit = new Deposit("Повышенный доход", 5, 10, true);
            Credit credit = new Credit("Не отказывай себе", 10, 15);
            BankAccountService bankAccountService = new BankAccountService("Обслуживание счета", 0, 15);

            db.AddBankProduct(deposit);
            db.AddBankProduct(capDeposit);
            db.AddBankProduct(credit);
            db.AddBankProduct(bankAccountService);

            testObjects.Add("Products.deposit", deposit.Id);
            testObjects.Add("Products.capDeposit", capDeposit.Id);
            testObjects.Add("Products.credit", credit.Id);
            testObjects.Add("Products.bankAccountService", bankAccountService.Id);

        }

        /// <summary>
        /// Добавляет клиентов в базу
        /// </summary>
        public void AddClients()
        {

            bool[] bools = new bool[] {false, true};

            foreach(Clients.ClientStatus clientStatus in db.ClientStatuses)
            {
                if (clientStatus is Clients.IndividualStatus)
                {

                    foreach (bool isVip in bools)
                    {

                        string fullName = GenerateHumanFullName();
                        string[] partsOfName = fullName.Split(' ');

                        Clients.Individual individual = new Clients.Individual(fullName);
                        individual.ClientStatus = clientStatus;
                        individual.ClientManager = db.Employees[random.Next(0, db.Employees.Count)];
                        individual.SurName = partsOfName[0];
                        individual.FirstName = partsOfName[1];
                        individual.Patronymic = partsOfName[2];
                        individual.IsVIP = isVip;
                        db.AddClient(individual);
                        testObjects.Add($"Clients.Individuals.{clientStatus.Name}.{isVip}", individual.Id);

                    }
                }
                else
                {
                    
                    foreach(bool isResident in bools)
                    {
                        string[] INNKPP = GenerateINNKPP(typeof(Clients.LegalEntity)).Split('/');

                        Clients.LegalEntity legalEntity = new Clients.LegalEntity(GenerateLegalEntityName());
                        legalEntity.ClientStatus = clientStatus;
                        legalEntity.ClientManager = db.Employees[random.Next(0, db.Employees.Count)];
                        legalEntity.IsResident = isResident;
                        legalEntity.IsCorporate = random.Next(0, 2)==0?false:true;
                        legalEntity.INN = INNKPP[0];
                        legalEntity.KPP = INNKPP[1];
                        db.AddClient(legalEntity);
                        testObjects.Add($"Clients.LegalEntityies.{clientStatus.Name}.{isResident}", legalEntity.Id);
                    }
                }
            }    
        }

        /// <summary>
        /// Добавляет банковские счета в базу
        /// </summary>
        public void AddBankAccounts()
        {
            
            //List<List<BankProduct>> productKits = new List<List<BankProduct>>();
            Dictionary<string, List<BankProduct>> productKits = new Dictionary<string, List<BankProduct>>();
            
            List<BankProduct> depositKit = new List<BankProduct>();
            depositKit.Add(TestBankProduct("Products.deposit"));
            depositKit.Add(TestBankProduct("Products.bankAccountService"));
            productKits.Add("depositKit", depositKit);

            List<BankProduct> creditKit = new List<BankProduct>();
            creditKit.Add(TestBankProduct("Products.credit"));
            creditKit.Add(TestBankProduct("Products.bankAccountService"));
            productKits.Add("creditKit", creditKit);

            List<BankProduct> capDepositKit = new List<BankProduct>();
            capDepositKit.Add(TestBankProduct("Products.capDeposit"));
            capDepositKit.Add(TestBankProduct("Products.bankAccountService"));
            productKits.Add("capDepositKit", capDepositKit);

            int accNumber = 0;

            foreach (KeyValuePair<string, List<BankProduct>> productKit in productKits)
            {
                
                bool[] bools = new bool[] { false, true };

                foreach (Clients.ClientStatus clientStatus in db.ClientStatuses)
                {
                    if (clientStatus is Clients.IndividualStatus)
                    {

                        foreach (bool isVip in bools)
                        {

                            string clientKey = $"Clients.Individuals.{clientStatus.Name}.{isVip}";
                            string accountKey = clientKey + $".{productKit.Key}";

                            CreateAddTestBankAccount(ref accNumber, productKit.Value, accountKey, clientKey);

                        } 
                    }
                    else
                    {

                        foreach (bool isResident in bools)
                        {
                            string clientKey = $"Clients.LegalEntityies.{clientStatus.Name}.{isResident}";
                            string accountKey = clientKey + $".{productKit.Key}";

                            CreateAddTestBankAccount(ref accNumber, productKit.Value, accountKey, clientKey);

                        }
                    }
                }
            }

        }

        /// <summary>
        /// Создаёт и добавляет в базу тестовый банковский счёт
        /// </summary>
        /// <param name="previousAccNumber">
        /// Число, номер предыдущего счёта. Новый счёт создаётся со следующим по порядку номером
        /// </param>
        /// <param name="productKit">
        /// Набор банковских продуктов к счёту</param>
        /// <param name="accountKey">
        /// Строка, ключ счёта в тестовых счетах
        /// </param>
        /// <param name="clientKey">
        /// Строка, ключ клиента в тестовых клиентах
        /// </param>
        private void CreateAddTestBankAccount(ref int previousAccNumber, List<BankProduct> productKit, string accountKey, string clientKey)
        {
            previousAccNumber++;

            string accTextNumber = String.Format("{0:D6}", previousAccNumber);

            BankAccount bankAccount = new BankAccount(accTextNumber, TestClient(clientKey), productKit);
            db.AddBankAccount(bankAccount);
            testBankAccounts.Add(accountKey, bankAccount);

        }
        
        /// <summary>
        /// Возвращает тестовый департамент
        /// </summary>
        /// <param name="key">
        /// Строка, ключ в коллекции тестовых объектов
        /// </param>
        /// <returns></returns>
        private Department TestDepartment(string key)
        {
            return db.FindCollectionElementById(db.Departments, testObjects[key]);
        }

        /// <summary>
        /// Возвращает тестового сотрудника
        /// </summary>
        /// <param name="key">
        /// Строка, ключ в коллекции тестовых объектов
        /// </param>
        /// <returns></returns>
        private Employee TestEmployee(string key)
        {
            return db.FindCollectionElementById(db.Employees, testObjects[key]);
        }

        /// <summary>
        /// Возвращает тестовый статус клиента
        /// </summary>
        /// <param name="key">
        /// Строка, ключ в коллекции тестовых объектов
        /// </param>
        /// <returns></returns>
        private ClientStatus TestClientStatus(string key)
        {
            return db.FindCollectionElementById(db.ClientStatuses, testObjects[key]);
        }

        /// <summary>
        /// Возвращает тестовый банковский продукт
        /// </summary>
        /// <param name="key">
        /// Строка, ключ в коллекции тестовых объектов
        /// </param>
        /// <returns></returns>
        private BankProduct TestBankProduct(string key)
        {
            return db.FindCollectionElementById(db.BankProducts, testObjects[key]);
        }

        /// <summary>
        /// Возвращает тестового клиента
        /// </summary>
        /// <param name="key">
        /// Строка, ключ в коллекции тестовых объектов
        /// </param>
        /// <returns></returns>
        private Client TestClient(string key)
        {
            return db.FindCollectionElementById(db.Clients, testObjects[key]);
        }
        
        #endregion Тестовые данные

        #region Служебные методы, классы

        /// <summary>
        /// Вложенный класс - описатель уровня департамента
        /// </summary>
        class depLevel
        {
            private Department department;
            private int level;
            public Department Department { get { return department; } }
            public int Level { get { return level; } }

            public depLevel(Department department, int level)
            {
                this.department = department;
                this.level = level;
            }
        }

        /// <summary>
        /// Генерирует случайным образом мужское или женское сочетание 'Имя Фамилия'
        /// </summary>
        /// <returns>
        /// Строка
        /// </returns>
        private string GenerateHumanFullName()
        {
            // Массивы имён и фамилий. В каждом массиве количество имён должно равняться количеству фамилий (для простоты).
            string[,] mans = new string[,] 
                { 
                    {"Комов", "Петров", "Гусев", "Редько", "Комлев" }, 
                    {"Иван", "Фёдор", "Степан", "Евгений", "Андрей" },
                    {"Евгеньевич", "Константинович", "Александрович", "Валерьевич", "Игнатьевич" }
                    
                };
            string[,] womans = new string[,] 
                { 
                    { "Иванова", "Ульянова", "Колокольцева", "Громова", "Ляпчева"},
                    { "Вера", "Анастасия", "Светлана", "Людмила", "Галина"},
                    { "Леонидовна",  "Петровна", "Фёдровна", "Юрьевна", "Владимировна" }
                     
                };

            int i = random.Next(0, 2);
            
            string[,] names = (i == 0) ? names = mans : names = womans;

            int upperBound = names.GetUpperBound(1) + 1;

            return names[0, random.Next(0, upperBound)] + " "  
                + names[1, random.Next(0, upperBound)] + " "
                + names[2, random.Next(0, upperBound)];

        }

        /// <summary>
        /// Генерирует случайным образом наименование организации
        /// </summary>
        /// <returns></returns>
        private string GenerateLegalEntityName()
        {
            string[,] names = new string[,]
            {
                { 
                    "ООО",
                    "ОАО",
                    "ЗАО",
                    "ПАО",
                    "НКО"
                },

                {
                    "\"Здоровье\"",
                    "\"Кухни без проблем\"",
                    "\"Комьюнити бездельников\"",
                    "\"Лодки и селёдки\"",
                    "\"Галопом по Европам\""
                }
            };

            int upperBound = names.GetUpperBound(1) + 1;

            return names[0, random.Next(0, upperBound)] + " " + names[1, random.Next(0, upperBound)];

        }

        /// <summary>
        /// Генерирует "правильный" ИНН/КПП
        /// </summary>
        /// <param name="clientType">
        /// Тип - какой клиет (Individual / LegalIntity)
        /// </param>
        /// <returns>
        /// Строка. Для Individual - 12-значный ИНН (пример: "012345678912"),
        /// для LegalEntity 10-значный ИНН, слэш, 9-значный КПП (пример: "0123456789/012345678")
        /// </returns>
        private string GenerateINNKPP(Type clientType)
        {

            string INNKPP = "";
            
            if (clientType == typeof(Clients.LegalEntity))
            {

                // Генерируем ИНН по правилам для юр. лиц (https://www.egrul.ru/test_inn.html)
                for (int i = 0; i < 9; i++)
                    INNKPP += random.Next(0, 10).ToString();
                INNKPP += INNControlValue(INNKPP, new int[] { 2, 4, 10, 3, 5, 9, 4, 6, 8}).ToString();

                // Добавляем КПП - первые 4 знака ИНН и "01001"
                INNKPP += "/" + INNKPP.Substring(0, 4) + "01001";

            }
            else
            {
                // Генерируем ИНН по правилам для физ. лиц (https://www.egrul.ru/test_inn.html)
                for (int i = 0; i < 10; i++)
                    INNKPP += random.Next(0, 10).ToString();
                INNKPP += INNControlValue(INNKPP, new int[] { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }).ToString();
                INNKPP += INNControlValue(INNKPP, new int[] { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }).ToString();

                // КПП у физ. лиц нет, ничего не добавляем
            }

            return INNKPP;

        }

        /// <summary>
        /// Вычисляет контрольную цифру ИНН
        /// </summary>
        /// <param name="leftPart">
        /// Строка - левая часть ИНН до контрольной цифры
        /// </param>
        /// <param name="weights">
        /// Массив - весовые коэффициенты для вычисления контрольной цифры 
        /// </param>
        /// <returns></returns>
        private int INNControlValue(string leftPart, int[] weights)
        {
            int control = 0;
            
            for (int i = 0; i < leftPart.Length; i++) 
                control += weights[i] * Convert.ToInt32((leftPart[i].ToString()));

            int controlValue = control % 11;
            if (controlValue > 9)
                controlValue = control % 10;
            
            return controlValue;

        }

        #endregion Служебные методы

    }
}
