using System;
using System.Collections.Generic;

namespace OrgDB_WPF
{
    internal class Tests
    {

        #region Поля

        DataBase db;

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
                    Random random = new Random();

                    Intern intern = new Intern(
                        $"Intern of {DepLevel.Department.Name}",
                        "",
                        random.Next(18, 30),
                        random.Next(100, 300),
                        DepLevel.Department.id);
                    db.AddEmployee(intern);

                    Specialist specialist = new Specialist(
                        $"Specialist of {DepLevel.Department.Name}",
                        "",
                        random.Next(20, 60),
                        random.Next(intern.Salary + 100, intern.Salary + 300),
                        DepLevel.Department.id);
                    db.AddEmployee(specialist);

                    int managerSalary = db.CalculateManagerSalary(DepLevel.Department);
                    Manager manager = new Manager(
                        $"Manager of {DepLevel.Department.Name}", 
                        "", 
                        random.Next(18, 60), 
                        managerSalary, 
                        DepLevel.Department.id);
                    db.AddEmployee(manager);                    

                }
            }
        
        }

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
        /// Добавляет цепочки статусов (Начинающий [партнёр] - постоянный [партнёр] - опытный [партнёр])
        /// клиентов - юридических и физических лиц
        /// </summary>
        public void AddClientStatuses()
        {
            Clients.ClientStatus individualBeginer = new Clients.IndividualStatus("Начинающий");
            db.AddClientStatus(individualBeginer);

            Clients.ClientStatus individualRegular = new Clients.IndividualStatus("Постоянный");
            individualRegular.PreviousClientStatus = individualBeginer;
            individualBeginer.NextClientStatus = individualRegular;
            db.AddClientStatus(individualRegular);

            Clients.ClientStatus individualMaster = new Clients.IndividualStatus("Опытный");
            individualMaster.PreviousClientStatus = individualRegular;
            individualRegular.NextClientStatus = individualMaster;
            db.AddClientStatus(individualMaster);

            Clients.LegalEntityStatus legalEntityBeginer = new Clients.LegalEntityStatus("Начинающий партнёр");
            db.AddClientStatus(legalEntityBeginer);

            Clients.LegalEntityStatus legalEntityRegular = new Clients.LegalEntityStatus("Постоянный партнёр");
            legalEntityRegular.PreviousClientStatus=legalEntityBeginer;
            legalEntityBeginer.NextClientStatus = legalEntityRegular;
            db.AddClientStatus(legalEntityRegular);

            Clients.LegalEntityStatus legalEntityMaster = new Clients.LegalEntityStatus("Опытный партнёр");
            legalEntityMaster.PreviousClientStatus = legalEntityRegular;
            legalEntityRegular.NextClientStatus = legalEntityMaster;
            db.AddClientStatus(legalEntityMaster);

        }



        #endregion Тестовые данные


    }
}
