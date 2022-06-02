using System;
using System.Collections.Generic;

namespace OrgDB_WPF
{
    internal class Tests
    {

        #region Поля

        DataBase db;
        Random random = new Random();

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

            Department d_1 = new Department("d_1", "Podolsk", Guid.Empty); DB.AddDepartment(d_1);
            Department d_2 = new Department("d_2", "Podolsk", Guid.Empty); DB.AddDepartment(d_2);

            Department d_1_1 = new Department("d_1_1", "Orehovo", d_1.id); DB.AddDepartment(d_1_1);
            Department d_1_2 = new Department("d_1_2", "Borisovo", d_1.id); DB.AddDepartment(d_1_2);

            Department d_1_1_1 = new Department("d_1_1_1", "Kukuevo", d_1_1.id); DB.AddDepartment(d_1_1_1);
            Department d_1_1_2 = new Department("d_1_1_2", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_2);
            Department d_1_1_3 = new Department("d_1_1_3", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_3);

            Department d_1_1_1_1 = new Department("d_1_1_1_1", "Zvezduevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_1);
            Department d_1_1_1_2 = new Department("d_1_1_1_2", "Yasenevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_2);

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

                    NameSurname = GenerateHumanFullName().Split(' ');

                    Specialist specialist = new Specialist(
                        NameSurname[1],
                        NameSurname[0],
                        random.Next(20, 60),
                        random.Next(intern.Salary + 100, intern.Salary + 300),
                        DepLevel.Department.id);
                    db.AddEmployee(specialist);

                    NameSurname = GenerateHumanFullName().Split(' ');

                    int managerSalary = db.CalculateManagerSalary(DepLevel.Department);
                    Manager manager = new Manager(
                        NameSurname[1],
                        NameSurname[0],
                        random.Next(18, 60), 
                        managerSalary, 
                        DepLevel.Department.id);
                    db.AddEmployee(manager);                    

                }
            }
        
        }
                
        /// <summary>
        /// Добавляет цепочки статусов (Начинающий [партнёр] - постоянный [партнёр] - опытный [партнёр])
        /// клиентов - юридических и физических лиц
        /// </summary>
        public void AddClientStatuses()
        {
            Clients.ClientStatus individualBeginer = new Clients.IndividualStatus("Начинающий");
            individualBeginer.CreditDiscountPercent = 0;
            individualBeginer.DepositAdditionalPercent = 0;
            db.AddClientStatus(individualBeginer);

            Clients.ClientStatus individualRegular = new Clients.IndividualStatus("Постоянный");
            individualRegular.PreviousClientStatus = individualBeginer;
            individualBeginer.NextClientStatus = individualRegular;
            individualRegular.CreditDiscountPercent = 3;
            individualRegular.DepositAdditionalPercent = 3;
            db.AddClientStatus(individualRegular);

            Clients.ClientStatus individualMaster = new Clients.IndividualStatus("Опытный");
            individualMaster.PreviousClientStatus = individualRegular;
            individualRegular.NextClientStatus = individualMaster;
            individualMaster.CreditDiscountPercent = 5;
            individualMaster.DepositAdditionalPercent = 5;
            db.AddClientStatus(individualMaster);

            Clients.LegalEntityStatus legalEntityBeginer = new Clients.LegalEntityStatus("Начинающий партнёр");
            legalEntityBeginer.CreditDiscountPercent = 2;
            legalEntityBeginer.DepositAdditionalPercent = 2;
            db.AddClientStatus(legalEntityBeginer);

            Clients.LegalEntityStatus legalEntityRegular = new Clients.LegalEntityStatus("Постоянный партнёр");
            legalEntityRegular.PreviousClientStatus=legalEntityBeginer;
            legalEntityBeginer.NextClientStatus = legalEntityRegular;
            legalEntityRegular.CreditDiscountPercent = 4;
            legalEntityRegular.DepositAdditionalPercent = 4;
            db.AddClientStatus(legalEntityRegular);

            Clients.LegalEntityStatus legalEntityMaster = new Clients.LegalEntityStatus("Опытный партнёр");
            legalEntityMaster.PreviousClientStatus = legalEntityRegular;
            legalEntityRegular.NextClientStatus = legalEntityMaster;
            legalEntityMaster.CreditDiscountPercent = 6;
            legalEntityMaster.DepositAdditionalPercent = 6;
            db.AddClientStatus(legalEntityMaster);            
        
        }

        /// <summary>
        /// Добавляет банковские продукты - депозиты (без и с капитализацией), кредит, обслуживание счёта
        /// </summary>
        public void AddBankProducts()
        {
            Products.Deposit deposit = new Products.Deposit("Мастер годового дохода", 5, 10);
            Products.Deposit capDeposit = new Products.Deposit("Повышенный доход", 5, 10, true);
            Products.Credit credit = new Products.Credit("Не отказывай себе", 10, 15);
            Products.BankAccountService bankAccountService = new Products.BankAccountService("Обслуживание счета", 0, 15);

            db.AddBankProduct(deposit);
            db.AddBankProduct(capDeposit);
            db.AddBankProduct(credit);
            db.AddBankProduct(bankAccountService);
        }

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
                    }

                    
                }
            }    
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

        private string GenerateINNKPP(Type clientType)
        {

            string INNKPP = "";
            
            if (clientType == typeof(Clients.LegalEntity))
            {

                // Генерируем ИНН по правилам для юр. лиц (https://www.egrul.ru/test_inn.html)
                for (int i = 0; i < 9; i++)
                    INNKPP += random.Next(0, 10).ToString();
                INNKPP += ControlValue(INNKPP, new int[] { 2, 4, 10, 3, 5, 9, 4, 6, 8}).ToString();

                // Добавляем КПП - первые 4 знака ИНН и "01001"
                INNKPP += "/" + INNKPP.Substring(0, 4) + "01001";

            }
            else
            {
                // Генерируем ИНН по правилам для физ. лиц (https://www.egrul.ru/test_inn.html)
                for (int i = 0; i < 10; i++)
                    INNKPP += random.Next(0, 10).ToString();
                INNKPP += ControlValue(INNKPP, new int[] { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }).ToString();
                INNKPP += ControlValue(INNKPP, new int[] { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 }).ToString();

                // КПП у физ. лиц нет, ничего не добавляем
            }

            return INNKPP;

        }

        private int ControlValue(string leftPart, int[] weights)
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
