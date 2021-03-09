using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace OrgDB_WPF
{
    public class DataBase : INotifyPropertyChanged
    {

        #region Поля

        // Департаметны, сотрудники
        protected List<Department> departments;
        protected List<Employee> employees;

        // Организация
        private Organization organization;

        // Общие настройки базы
        public DBSettings dbSettings = new DBSettings();
        
        #endregion
        
        #region Свойства

        // Организация
        public Organization Organization { get { return organization; } set { organization = value; } }

        // Свойства "Департаменты" и "Сотрудники" с доступом public нужны Json-сериализатору, иначе они игнорируются

        // Департаметны, сотрудники
        public List<Department> Departments { get { return departments; } }
        public List<Employee> Employees { get { return employees; } }

        /// <summary>
        /// Путь к файлу базы
        /// </summary>
        public string DBFilePath
        {
            get { return dbSettings.DBFilePath; }
            set
            {
                dbSettings.DBFilePath = value;

                // при установке свойства вызываем событие
                // для обслуживания подписанными на него
                OnPropertyChanged("DBFilePath");
            }
        }

        /// <summary>
        /// Признак, что база пустая (нет департаменов и сотрудников)
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return departments.Count == 0 && employees.Count == 0;
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public DataBase()
        {

            dbSettings = new DBSettings();
            dbSettings.Initialize();

            organization = new Organization();

            departments = new List<Department>();
            employees = new List<Employee>();
        }

        public DataBase(DataBase dataBase) { }

        #endregion Конструкторы

        #region API

        #region Департаменты

        /// <summary>
        /// Добавляет департамент в базу.
        /// </summary>
        /// <param name="Dep">Экземляр департамента</param>
        public void AddDepartment(Department Dep)
        {

            if (Dep.IsEditing)
                throw new Exception("Служебный экземпляр департамента не предназначен для добавления в базу!");
            
            if (departments == null) departments = new List<Department>();
            
            // Проверяем уникальность id.
            if (departments.Exists(x => x.id == Dep.id))
                throw new Exception("Попытка добавления элемента с дублирующимся ключом!");

            // Установим уникальное имя департамента
            Dep.Name = Common.NextAutoName(Dep.Name, Department.DepartmentNames);

            // Установим имя головного департамента
            Dep.ParentName = ParentDepartmentName(Dep);

            // Всё ок, добавляем департрамент
            departments.Add(Dep);
        }

        /// <summary>
        /// Удаляет департамент из базы.
        /// Опционально, перед удалением проверяет наличие ссылок. По умолчанию опция включена.
        /// </summary>
        /// <param name="Dep">Экземпляр департамента</param>
        /// <param name="CheckLinks">Проверять наличие ссылок</param>
        public void RemoveDepartment(Department Dep, bool CheckLinks = true)
        {
            
            if (CheckLinks && LinksExists(Dep))
                throw new Exception("На данный элемент существуют ссылки, удаление невозможно!");

            departments.Remove(Dep);
        }

        /// <summary>
        /// Возвращает список всех (напрямую и опосредованно) подчинённых департаментов
        /// (департаменты в иерархии текущего)
        /// </summary>
        /// <param name="Dep">Экзепляр департамента, для которого нужно список подчинённых</param>
        /// <returns>List<Department></returns>
        public List<Department> GetSubDepartments(Department Dep)
        {
            // Список всех (напрямую и опосредованно) подчинённых департаментов
            List<Department> subDepartments = new List<Department>();

            // Список дочерних департаментов (только напрямую подчинённых)
            List<Department> directSubDepartments =
                departments.FindAll((Department dsD) => { return dsD.ParentId == Dep.id; });
            
            // В цикле для каждого напрямую подчинённого департамента:
            // 1. Добавляем напрямую подчинённый в список всех подчинённых
            // 2. Рекурсивно определяем подчинённых для напрямую подчинённых и их добавляем
            //      в список всех подчинённых
            foreach (Department directSubDeparment in directSubDepartments)
            {
                subDepartments.Add(directSubDeparment);
                List<Department> secondarySubDepartments = GetSubDepartments(directSubDeparment);
                foreach (Department secondarySubDepartment in secondarySubDepartments)
                    subDepartments.Add(secondarySubDepartment);
            }

            return subDepartments;
        }

        /// <summary>
        /// Возвращает имя головного департамента
        /// </summary>
        /// <param name="Dep">
        /// Департамент, для которого надо получить имя головного
        /// </param>
        /// <returns></returns>
        public string ParentDepartmentName(Department Dep)
        {
            Department parentDep = ParentDepartment(Dep);
            if (parentDep == null) return "";
            else return parentDep.Name;
        }

        /// <summary>
        /// Возвращает головной департамент
        /// </summary>
        /// <param name="Dep">
        /// Департамент, для которого надо найти головной
        /// </param>
        /// <returns></returns>
        public Department ParentDepartment(Department Dep)
        {
            return departments.Find((Department dep) => { return dep.id == Dep.ParentId; });
        }

        /// <summary>
        /// Получает департамент по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Department GetDepartment(Guid id)
        {
            return departments.Find((Department dep)=> { return dep.id == id; });
        }

        /// <summary>
        /// Возвращает наименовнаие департаментам по его ID
        /// </summary>
        /// <param name="id">ID департамента</param>
        /// <returns></returns>
        public string DepartmentName(Guid id)
        {
            Department dep = GetDepartment(id);
            if (dep == null) return "";
            else return dep.Name;
        }

        #endregion Департаменты

        #region Сотрудники

        /// <summary>
        /// Добавляет сотрудника в базу.
        /// </summary>
        /// <param name="Emp">Экземпляр сотрудника</param>
        public void AddEmployee(Employee Emp)
        {
            // Если экземпляр списка сотрудников ещё не определён, задаём его.
            if (employees == null) employees = new List<Employee>();

            // Проверяем уникальность id
            if (employees.Exists(x => x.id == Emp.id))
                throw new Exception("Попытка добавления элемента с дублирующимся ключом!");

            // Установим уникальное имя сотрудника
            Emp.Name = Emp.NextAutoName();

            // Устанавливаем имя департамента
            Emp.DepartmentName = DepartmentName(Emp.DepartmentID); 

            // Всё ок, добавляем сотрудника
            employees.Add(Emp);
        }

        /// <summary>
        /// Удаляет сотрудника из базы. 
        /// Опционально, перед удалением проверяет наличие ссылок. По умолчанию опция включена
        /// </summary>
        /// <param name="Emp">Экземпляр сотрудника</param>
        /// <param name="CheckLinks">Проверять наличие ссылок</param>
        public void RemoveEmployee(Employee Emp, bool CheckLinks = true)
        {
            if (CheckLinks && LinksExists(Emp))
                throw new Exception("На данный элемент существуют ссылки, удаление невозможно!");

            employees.Remove(Emp);
        }

        /// <summary>
        /// Возвращает список всех сотрдуников департамента, кроме менеджера, а также список сотрудников напрямую и опосредованно подчинённых департаментов
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public List<Employee> GetSubEmployees(Department department)
        {
            // Cписок всех (напрямую и опосредованно) подчинённых сотрудников
            List<Employee> subEmployees = new List<Employee>();

            // Добавляем всех сотрудников этого департамента, кроме менеджера
            foreach (Employee employee in employees.FindAll(x => x.DepartmentID == department.id && x.post_Enum != Employee.post_enum.manager)) 
                subEmployees.Add(employee);

            // Список всех (напрямую и опосредованно) подчинённых департаментов для данного
            List<Department> subDepartments = GetSubDepartments(departments.Find(x => x.id == department.id));

            // В цикле для каждого подчинённого департамента:
            // 1. Получаем список всех сотрудников этого подчинённого департамента
            // 2. Во вложенном цикле добавляем их в список всех подчинённых сотрудников
            foreach (Department subDepartment in subDepartments)
            {
                List<Employee> subDepartmentEmployees = employees.FindAll(x => x.DepartmentID == subDepartment.id);
                foreach (Employee subDepEmp in subDepartmentEmployees) subEmployees.Add(subDepEmp);
            }

            return subEmployees;
        }

        /// <summary>
        /// Возвращает список всех (напрямую и опосредованно) подчинённых сотрудников
        /// </summary>
        /// <param name="Man">Руководитель, для которого нужно вывести подчинённых</param>
        /// <returns></returns>
        public List<Employee> GetSubEmployees(Manager Man)
        {
            return GetSubEmployees(GetDepartment(Man.DepartmentID));
        }

        /// <summary>
        /// Возвращает рассчитанную зарплату управляющего департаментом (вариант для департамента)
        /// Зарплата рассчитывается как зарплата всех подчинённых сотрудников, умноженная на процент з/п управляющего,
        /// но не менее 1300.
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public int CalculateManagerSalary(Department department)
        {

            int subEmployeeSalaryTotal = 0;
            foreach (Employee employee in GetSubEmployees(department)) subEmployeeSalaryTotal += employee.Salary;

            return Math.Max(organization.MinManagerSalary, (int)(subEmployeeSalaryTotal * organization.ManagerSalaryPercent / 100));
        }

        /// <summary>
        /// Возвращает рассчитанную зарплату управляющего департаментом (вариант для сотрудника)
        /// Зарплата рассчитывается как зарплата всех подчинённых сотрудников, умноженная на процент з/п управляющего,
        /// но не менее 1300. 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public int CalculateManagerSalary(Manager manager)
        {
            return CalculateManagerSalary(GetDepartment(manager.DepartmentID));
        }

        public void SortEmployeesByFields(List<EmployeeSortField> employeesSortingFields)
        {

        }

        #endregion Сотрудники

        #region Проверка ссылок

        /// <summary>
        /// Проверяет, существуют ли в других элементах базы ссылки на этот
        /// Перегрузка для департамента
        /// </summary>
        /// <param name="Dep">Экземпляр департамента</param>
        /// <returns>bool</returns>
        public bool LinksExists(Department Dep)
        {
            // ссылки могут быть в самих департаментах - на головные подразделения
            // и в сотрдуниках
            return 
                (departments.Exists(x=>x.ParentId==Dep.id))
                ||
                (employees!=null && employees.Exists(x => x.DepartmentID == Dep.id));
        }

        /// <summary>
        /// Проверяет, существуют ли в других элементах базы ссылки на этот
        /// Перегрузка для сотрудника
        /// </summary>
        /// <param name="Emp">Экземпляр сотрудника</param>
        /// <returns></returns>
        public bool LinksExists(Employee Emp)
        {
            return false;
        }

        #endregion Проверка ссылок

        #region Запись / чтение базы

        /// <summary>
        /// Записывает базу данных в файл.
        /// Тип файла (XML, JSON) определяется по расширению, указанному в настройке базы DBFilePath
        /// </summary>
        /// <returns>
        /// Булево - признак, что база успешно записана в файл
        /// </returns>
        public bool SaveDB()
        {
            return Serialize();                        
        }

        /// <summary>
        /// Читает базу из файла.
        /// Тип файла (XML, JSON) определяется по расширению, указанному в настройке базы DBFilePath
        /// </summary>
        /// <returns>
        /// Булево - признак, что база успешно прочитана из файла
        /// </returns>
        public bool LoadDB()
        {
            Flush();
            return Deserialize();
        }

        /// <summary>
        /// Сериализация. Фактическая переопределяется и выполняется в соответствующем производном классе
        /// </summary>
        /// <returns></returns>
        public bool Serialize() 
        {
            return new DBSerializer(this).Serialize();
        }

        /// <summary>
        /// Десериализация. Фактическая переопределяется и выполняется в соответствующем производном классе
        /// </summary>
        /// <returns></returns>
        public bool Deserialize() 
        {
            return new DBSerializer(this).Deserialize();
        }
        
        #region Классы записи / чтения в различные форматы

        public class DBSerializer
        {
            public DataBase db;

            // Список ошибок
            protected List<string> errorList = new List<string>();

            public DBSerializer(DataBase dataBase) { db = dataBase; }
            public DBSerializer(DBSerializer dBSerializer) { }
            public DBSerializer() { }

            virtual public bool Serialize()
            {
                return dBSerializer().Serialize();
            }

            virtual public bool Deserialize()
            {
                return dBSerializer().Deserialize();
            }

            DBSerializer dBSerializer()
            {
                string Extension = Path.GetExtension(db.DBFilePath).ToUpper();

                switch (Extension)
                {

                    case ".XML":
                        return new XMLDataBaseSerializer(this);
                    case ".JSON":
                        return new JSONDataBaseSerializer(this);
                }

                return new DBSerializer(db);
            }

        }
        public class XMLDataBaseSerializer : DBSerializer, IXmlSerializable
        {

            #region Поля

            // Чтение XML. Получается из десериализуемой базы для дальнейшей управляемой десериализации
            public XmlReader xmlReader;
            
            #endregion Поля

            #region Конструкторы
            
            /// <summary>
            /// Основной конструктор
            /// </summary>
            /// <param name="dataBase"></param>
            public XMLDataBaseSerializer(DBSerializer dBSerializer):base() { db = dBSerializer.db; }

            /// <summary>
            /// Формальный конструктор по умолчанию для реализации интерфейса IXmlSerializable
            /// </summary>
            public XMLDataBaseSerializer() { }

            #endregion Конструкторы

            #region Переопределённые методы базового класаа серилизации / десериализации

            /// <summary>
            /// Записывает базу в XML-файл
            /// </summary>
            /// <returns>
            /// Булево - признак успешной записи
            /// </returns>
            override public bool Serialize()

            {

                Stream fStream = new FileStream(db.DBFilePath, FileMode.Create, FileAccess.Write);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLDataBaseSerializer));
                xmlSerializer.Serialize(fStream, this);
                fStream.Close();

                return true;
            }

            /// <summary>
            /// Загружает базу из файла
            /// </summary>
            /// <returns>
            /// Булево - признак успешной загрузки из файла
            /// </returns>
            override public bool Deserialize()
            {

                // Создаём служебный экземпляр базы, получаем из него читалку XML,
                // с помощью этой читалки заполняем эту базу.

                errorList.Clear();

                XmlSerializer xmlSerializerDB = new XmlSerializer(typeof(XMLDataBaseSerializer));

                Stream fileStream = new FileStream(db.DBFilePath, FileMode.Open, FileAccess.Read);

                XMLDataBaseSerializer dbCopy = xmlSerializerDB.Deserialize(fileStream) as XMLDataBaseSerializer;

                xmlReader = dbCopy.xmlReader;

                while (xmlReader.Read())
                    ReadDBPart(xmlReader);

                fileStream.Close();

                if (db.organization.ManagerSalaryPercent == 0) errorList.Add("В указанном файле нет данных \"Процент менеджера\"");
                if (String.IsNullOrEmpty(db.dbSettings.DBFilePath)) errorList.Add("В указанном файле нет данных \"Путь к файлу базы\"");

                if (errorList.Count > 0) return false;

                return true;
            }

            #endregion Переопределённые методы базового класаа серилизации / десериализации

            #region Служебные методы

            /// <summary>
            /// Читает часть XML
            /// </summary>
            /// <param name="reader">
            /// Читалка XML
            /// </param>
            void ReadDBPart(XmlReader reader)
            {

                if (reader.Name == "dbSettings" && reader.NodeType == XmlNodeType.Element)
                {
                    XmlReader dbSettingsReader = reader.ReadSubtree();
                    db.dbSettings.ReadXml(dbSettingsReader);

                    // Оповестим об изменениях свойств
                    db.NotifyDBSettingsPropertiesChanged();

                }

                if (reader.Name == "organization" && reader.NodeType == XmlNodeType.Element)
                {
                    XmlReader organizationReader = reader.ReadSubtree();
                    db.organization.ReadXml(organizationReader);
                }

                if (reader.Name == "departments" && reader.NodeType == XmlNodeType.Element)
                {
                    // В отдельном XML-чтении считаем департаменты
                    XmlReader DepReader = reader.ReadSubtree();
                    ReadDepartments(DepReader);

                }

                if (reader.Name == "employees" && reader.NodeType == XmlNodeType.Element)
                {

                    // Создаём объект-список сотрудников
                    db.employees = new List<Employee>();

                    // В отдельном XML-чтении считаем сотрудников
                    XmlReader EmpReader = reader.ReadSubtree();
                    ReadEmployees(EmpReader);

                }

            }

            /// <summary>
            /// Читает департаменты
            /// </summary>
            /// <param name="reader">
            /// XML-читалка департаментов (состоит из узла departments) 
            /// </param>
            private void ReadDepartments(XmlReader reader)
            {

                // Создаём объект-список департаментов
                db.departments = new List<Department>();

                // Перемещаемся к дочернему узлу департамента
                reader.ReadToDescendant("department");

                // В цикле для каждого узла департамента
                // создаём отдельную читалку, с помощью соответствующего
                // конструктора создаём департамент и добавляем его в базу
                while (!(reader.Name == "departments" && reader.NodeType == XmlNodeType.EndElement))
                {

                    XmlReader departmentReader = reader.ReadSubtree();
                    Department dep = new Department(departmentReader);
                    db.departments.Add(dep);
                    reader.Skip();

                }

                // Заполним имена головных департаментов
                db.FillParentDepartmentNames();

            }

            /// <summary>
            /// Читает сотрудников
            /// </summary>
            /// <param name="reader">
            /// XML-читалка сотрудников (состоит из узла employees)
            /// </param>
            private void ReadEmployees(XmlReader reader)
            {
                db.employees = new List<Employee>();

                // Перемещаемся к узлу "employees"
                reader.Read();
                // Перемещаемся к первому узлу сотрудника
                reader.Read();

                // В цикле создаём XML-читалку сотрудника, анализируем должность,
                // создаём соответствующего сотрудника и добавляем его в базу
                while (!(reader.Name == "employees" && reader.NodeType == XmlNodeType.EndElement))
                {

                    XmlReader employeeReader = reader.ReadSubtree();

                    Employee.post_enum post_Enum = (Employee.post_enum)Enum.Parse(typeof(Employee.post_enum), reader.Name, false);

                    Employee emp;

                    switch (post_Enum)
                    {
                        case Employee.post_enum.intern:
                            emp = new Intern(employeeReader);
                            db.employees.Add(emp);
                            break;
                        case Employee.post_enum.specialist:
                            emp = new Specialist(employeeReader);
                            db.employees.Add(emp);
                            break;
                        case Employee.post_enum.manager:
                            emp = new Manager(employeeReader);
                            db.employees.Add(emp);
                            break;
                    }
                    reader.Skip();
                }

                db.FillEmployeesDepartmentNames();
            }

            #endregion Служебные методы

            #region Реализация IXmlSerializable

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                // Задаём значение служебному полю XML-читалки
                xmlReader = reader;
            }

            public void WriteXml(XmlWriter writer)
            {
                // Узел dbSettings
                writer.WriteStartElement("dbSettings");
                db.dbSettings.WriteXml(writer);
                writer.WriteEndElement();

                // Узел organization
                writer.WriteStartElement("organization");
                db.Organization.WriteXml(writer);
                writer.WriteEndElement();

                // Узел Departments
                if (db.departments != null && db.departments.Count > 0)
                {
                    writer.WriteStartElement("departments");
                    foreach (Department Dep in db.departments) Dep.WriteXml(writer);
                    writer.WriteEndElement();
                }

                // Узел Employees
                if (db.employees != null && db.employees.Count > 0)
                {
                    writer.WriteStartElement("employees");
                    foreach (Employee Emp in db.employees) Emp.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }

            #endregion Реализация IXmlSerializable

        }
        class JSONDataBaseSerializer : DBSerializer
        {
            
            #region Конструкторы

            /// <summary>
            /// Основной констурктор
            /// </summary>
            /// <param name="dataBase"></param>
            public JSONDataBaseSerializer(DBSerializer dBSerializer) { db = dBSerializer.db; }

            #endregion Конструкторы

            #region Переопределённые методы базового класаа серилизации / десериализации

            /// <summary>
            /// Сериализует базу в JSON-файл
            /// </summary>
            /// <returns></returns>
            override public bool Serialize()
            {

                // Пользуемся своими JSON-конвертерами для каждого члена базы,
                // чтобы управлять записью (записывать только нужные элементы)

                string js = JsonConvert.SerializeObject(db, Newtonsoft.Json.Formatting.Indented,
                    new DBSettingsJsonConverter(),
                    new OrganizationJsonConverter(),
                    new DepartmentJsonConverter(),
                    new EmployeeJsonConverter());
                File.WriteAllText(db.DBFilePath, js);

                return true;
            }

            /// <summary>
            /// Десериализует базу из JSON-файла
            /// </summary>
            /// <returns></returns>
            override public bool Deserialize()
            {

                errorList.Clear();

                if (db.departments == null) db.departments = new List<Department>();
                else db.departments.Clear();

                if (db.employees == null) db.employees = new List<Employee>();
                else db.employees.Clear();

                string js = File.ReadAllText(db.DBFilePath);

                // DTO - Data Transfer Object

                // JSON - объект (DTO) базы в файле
                JObject dbJson = JObject.Parse(js);

                JToken jToken;

                // Параметры настройки базы получаем по соответствующим ключам (токенам)
                jToken = dbJson.SelectToken("dbSettings.dbFilePath");
                if (jToken != null)
                    db.dbSettings.DBFilePath = (string)jToken;
                else
                    errorList.Add("В указанном файле нет данных \"Путь к файлу базы\"");

                jToken = dbJson.SelectToken("Organization.ManagerSalaryPercent");
                if (jToken != null)
                    db.organization.ManagerSalaryPercent = (double)jToken;
                else
                    errorList.Add("В указанном файле нет данных \"Процент управляющего\"");

                jToken = dbJson.SelectToken("Organization.MinManagerSalary");
                if (jToken != null)
                    db.organization.MinManagerSalary = (int)jToken;
                else
                    errorList.Add("В указанном файле нет данных \"Минимальный уровень з/п управляющего\"");

                jToken = dbJson.SelectToken("Organization.MinSpecSalary");
                if (jToken != null)
                    db.organization.MinSpecSalary = (int)jToken;
                else
                    errorList.Add("В указанном файле нет данных \"Минимальный уровень з/п специалиста\"");

                jToken = dbJson.SelectToken("Organization.MinInternSalary");
                if (jToken != null)
                    db.organization.MinInternSalary = (int)jToken;
                else
                    errorList.Add("В указанном файле нет данных \"Минимальный уровень з/п интерна\"");

                if (errorList.Count > 0) return false;

                // Оповестим об изменениях свойств
                db.NotifyDBSettingsPropertiesChanged();

                // JSON - массив департаментов
                jToken = dbJson["Departments"];
                if (jToken.HasValues)
                {
                    JArray jDepartments = (JArray)jToken;
                    for (int i = 0; i < jDepartments.Count; i++)
                    {
                        // получаем JSON - департамент (DTO), с помощью соответствующего конструктора
                        // создаём департамент и добавляем его в список
                        JObject jDepartment = (JObject)jDepartments[i];
                        Department curDep = new Department(jDepartment);
                        db.departments.Add(curDep);
                    }

                    // Заполним имена головных департаментов
                    db.FillParentDepartmentNames();
                }

                // JSON - массив сотрудников
                jToken = dbJson["Employees"];
                if (jToken.HasValues)
                {
                    JArray jEmployees = (JArray)jToken;
                    for (int i = 0; i < jEmployees.Count; i++)
                    {
                        // получаем JSON - сотрудника (DTO), 
                        // по соответствующему токену анализируем его должность,
                        // в зависимости от должности создаём соответствующего сотрудника,
                        // добавляем его в список
                        JObject jEmployee = (JObject)jEmployees[i];
                        Employee.post_enum post_Enum = (Employee.post_enum)(int)jEmployee.SelectToken("Post");

                        Employee emp;
                        switch (post_Enum)
                        {
                            case Employee.post_enum.intern:
                                emp = new Intern(jEmployee);
                                db.employees.Add(emp);
                                break;
                            case Employee.post_enum.specialist:
                                emp = new Specialist(jEmployee);
                                db.employees.Add(emp);
                                break;
                            case Employee.post_enum.manager:
                                emp = new Manager(jEmployee);
                                db.employees.Add(emp);
                                break;
                        }
                    }

                    db.FillEmployeesDepartmentNames();
                }

                return true;
            }

            #endregion Переопределённые методы базового класаа серилизации / десериализации

        }

        #endregion Классы записи / чтения в различные форматы

        #endregion Запись / чтение базы

        #region Очистка базы

        /// <summary>
        /// Удаляет все данные из базы, кроме пути к файлу базы.
        /// </summary>
        public void Flush()
        {
            employees = new List<Employee>();
            departments = new List<Department>();

            string dbFilePath = DBFilePath;
            dbSettings = new DBSettings();
            dbSettings.Initialize();
            DBFilePath = dbFilePath;

            organization.Flush();
        }

        #endregion Очистка базы

        #endregion API
        
        #region Вспомогательные методы

        /// <summary>
        /// Заполняет имена головных департаментов
        /// </summary>
        private void FillParentDepartmentNames()
        {
            foreach (Department dep in Departments)
            {
                dep.ParentName = ParentDepartmentName(dep);
            }
        }

        /// <summary>
        /// Заполняет имена департаментов у сотрдуников
        /// </summary>
        private void FillEmployeesDepartmentNames()
        {
            foreach (Employee emp in Employees)
            {
                emp.DepartmentName = DepartmentName(emp.DepartmentID);
            }
        }

        private void NotifyDBSettingsPropertiesChanged()
        {
            string propertyNames = "DBFilePath";
            foreach (string propertyName in propertyNames.Split(',')) OnPropertyChanged(propertyName);
        }
        
        #endregion Вспомогательные методы

        #region Реализация INotifyPropertyChanged

        /// <summary>
        /// Обработчик изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion Реализация INotifyPropertyChanged
       
    }
}
