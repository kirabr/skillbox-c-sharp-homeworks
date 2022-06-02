using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace OrgDB_WPF
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Поля

        // Текущее приложение
        App currApp = (App)App.Current;

        // Наборы представлений данных
        CollectionViewSource DepartmentsViewSource;
        CollectionViewSource EmployeesViewSource;
        CollectionViewSource IndividualStatusesViewSource;

        // Наборы элементов интерфейса, изменяемых совместно
        UI.DataSections dataSections = new UI.DataSections();

        #endregion Поля

        #region Свойства
        
        DataBase DB { get; }

        #endregion Свойства

        #region Главный метод
        public MainWindow()
        {

            LogEvent("Запуск приложения");

            DB = currApp.DB;

            InitializeComponent();

            DataContext = DB;
            sp_OrgSettings.DataContext = DB.Organization;

            tcDataTabs.Visibility = Visibility.Collapsed;
            
            TestsDB();

            DepartmentsViewSource = new CollectionViewSource();
            DepartmentsViewSource.Source = DB.Departments;
                        
            EmployeesViewSource = new CollectionViewSource();
            EmployeesViewSource.Source = DB.Employees;

            IndividualStatusesViewSource = new CollectionViewSource();
            IndividualStatusesViewSource.Source = DB.ClientStatuses;

            DepListView.ItemsSource = DepartmentsViewSource.View;
            EmpListView.ItemsSource = EmployeesViewSource.View;
            IndividualStatusesViewSource.Filter += IndividualStatusesViewSource_Filter;
            IndividualStatusesListView.ItemsSource = IndividualStatusesViewSource.View;
            
            InitializeDataSections();
            SetDataSectionsVisibility();

        }

        #endregion Главный метод

        #region Обработчики событий формы

        private void Window_Closed(object sender, EventArgs e)
        {
            LogEvent("Закрытие приложения");
        }

        #endregion Обработчики событий формы

        #region Обработчики событий элементов формы

        #region Элементы меню "Вкладки"

        private void TabItemDepartmentsVisibility_Checked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemDepartmentsVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemEmployeesVisibility_Checked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemEmployeesVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemClientStatusesVisibility_Checked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemClientStatusesVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemdbSettingsVisibility_Checked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        private void TabItemdbSettingsVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            SetDataSectionVisibility(dataSections[(MenuItem)sender]);
        }

        #endregion Элементы меню "Вкладки"

        #region Закладка Департаменты

        private void DepListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ChangeDepartmentCanExecute()) ChangeDepartment((Department)DepListView.SelectedItem);
            if (e.Key == Key.Delete && DeleteDepartmentCanExecute()) DeleteDepartment((Department)DepListView.SelectedItem);
        }

        private void DepListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ChangeDepartmentCanExecute()) ChangeDepartment((Department)DepListView.SelectedItem);
        }

        #endregion Закладка Департаменты

        #region Закладка Сотрудники

        private void EmpListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ChangeEmployeeCanExecute()) 
                ChangeEmployee((Employee)EmpListView.SelectedItem);
            if (e.Key == Key.Delete && DeleteEmployeeCanExecute()) 
                DeleteEmployee((Employee)EmpListView.SelectedItem);
        }

        private void EmpListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ChangeEmployeeCanExecute()) ChangeEmployee((Employee)EmpListView.SelectedItem);
        }

        #region Закладка Статусы клиентов

        private void IndividualStatusesListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OneOfIndividualClientStatusSelected()) 
                ChangeClientStatus((Clients.IndividualStatus)IndividualStatusesListView.SelectedItem);
            if (e.Key == Key.Delete && DeleteIndividualClientStatusCanExecute())
                DeleteClientStatus((Clients.IndividualStatus)IndividualStatusesListView.SelectedItem);
        }

        private void IndividualStatusesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OneOfIndividualClientStatusSelected())
                ChangeClientStatus((Clients.IndividualStatus)IndividualStatusesListView.SelectedItem);
        }
        
        #endregion Закладка Статусы клиентов

        #endregion Закладка Сотрудники

        #region Закладка Настройки базы

        /// <summary>
        /// Обработчик нажатия кнопки выбора имени файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            SetDBFilePathInDialog();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Записать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteFile_Click(object sender, RoutedEventArgs e)
        {
            SaveDB();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Прочитать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            LoadDB();
        }

        #endregion Закладка Настройки базы

        #endregion Обработчики событий элементов формы

        #region Основные методы

        /// <summary>
        /// Сохраняет базу
        /// </summary>
        private void SaveDB()
        {
            if (!CheckDBFilePath()) return;
            if (DB.SaveDB())
            {
                MessageBox.Show("База записана");
                LogEvent("База записана в " + DB.dbSettings.DBFilePath);
            }

            else
            {
                MessageBox.Show("База не записана");
                LogEvent("Сбой записи базы в " + DB.dbSettings.DBFilePath);
            }

        }

        /// <summary>
        /// Загружает базу
        /// </summary>
        private void LoadDB()
        {
            if (!CheckDBFilePath(true)) return;
            if (DB.LoadDB())
            {
                MessageBox.Show("База загружена");
                LogEvent("База загружена из " + DB.dbSettings.DBFilePath);
            }
            else
            {
                MessageBox.Show("База не загружена");
                LogEvent("Сбой загрузки базы из " + DB.dbSettings.DBFilePath);
            }

            DepartmentsViewSource.Source = DB.Departments;
            DepListView.ItemsSource = DepartmentsViewSource.View;
            DepListView.Items.Refresh();
            
            EmployeesViewSource.Source = DB.Employees;
            EmpListView.ItemsSource = EmployeesViewSource.View;            
            EmpListView.Items.Refresh();

            IndividualStatusesViewSource.Source = DB.ClientStatuses;
            IndividualStatusesListView.ItemsSource = IndividualStatusesViewSource.View;
            IndividualStatusesViewSource.Filter -= IndividualStatusesViewSource_Filter;
            IndividualStatusesViewSource.Filter += IndividualStatusesViewSource_Filter;
            IndividualStatusesListView.Items.Refresh();

        }

        /// <summary>
        /// Подготавливает и открывает форму нового департамента для добавления
        /// </summary>
        private void AddDepartment()
        {
            DepartmentForm departmentForm = currApp.GetDepartmentForm();
            departmentForm.Owner = this;
            // Добавляем обработчик события завершения редактирования. Он нужен только при добавлении департамента.
            departmentForm.finishEdit += OnDepartmentAdding;
            departmentForm.Show();
        }

        /// <summary>
        /// Подготваливает и открывает форму существующего департамента для изменения
        /// </summary>
        /// <param name="Dep">Департамент для изменения</param>
        private void ChangeDepartment(Department Dep)
        {
            DepartmentForm departmentForm = currApp.GetDepartmentForm();
            departmentForm.Owner = this;
            // Устанавливаем департамент для редактирования.
            departmentForm.Dep = Dep;
            departmentForm.finishEdit += OnDepartmentFinishEditing;

            currApp.ExceptDepartments.Clear();
            currApp.ExceptDepartments.Add(Dep);

            departmentForm.Show();
        }

        /// <summary>
        /// Удаляет департамент из базы
        /// </summary>
        /// <param name="Dep"></param>
        private void DeleteDepartment(Department Dep)
        {

            MessageBoxResult UserAnswer = MessageBox.Show($"Удалить {Dep.Name}?", 
                "Подтверждение удаления департамента", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (UserAnswer != MessageBoxResult.Yes) return;
            
            try
            {
                DB.RemoveDepartment(Dep);
                LogEvent($"Удалён депантамент {Dep.Name}");
                DepListView.Items.Refresh();
            }
            catch(Exception excp)
            {
                string text = $"Не удалось удалить департамент {Dep.Name}, причина: \n{excp.Message}";
                LogEvent(text);
                MessageBox.Show(text);
            }
        }

        /// <summary>
        /// Подготавливает и открывает форму нового сотрудника для добавления
        /// </summary>
        private void AddEmployee()
        {
            EmployeeForm employeeForm = currApp.GetEmployeeForm();
            employeeForm.Owner = this;
            employeeForm.finishEdit += OnEmployeeAdding;
            employeeForm.Show();
        }

        /// <summary>
        /// Подготавливает и открывает форму существующего сотрудника для изменения
        /// </summary>
        /// <param name="Emp"></param>
        private void ChangeEmployee(Employee Emp)
        {
            EmployeeForm employeeForm = currApp.GetEmployeeForm();
            employeeForm.Owner = this;
            employeeForm.Emp = Emp;
            employeeForm.finishEdit += OnEmployeeFinishEditing;
            employeeForm.Show();
        }

        /// <summary>
        /// Удаляет сотрудника из базы
        /// </summary>
        /// <param name="Emp"></param>
        private void DeleteEmployee(Employee Emp)
        {
            MessageBoxResult UserAnswer = MessageBox.Show($"Удалить {Emp.Name}?",
                "Подтверждение удаления сотрудника",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (UserAnswer != MessageBoxResult.Yes) return;

            try
            {
                DB.RemoveEmployee(Emp);
                LogEvent($"Удалён сотрудник {Emp.Name}");
                EmployeesViewSource.Source = DB.Employees;
                EmpListView.Items.Refresh();
            }
            catch (Exception excp)
            {
                string text = $"Не удалось удалить сотрудника {Emp.Name}, причина: \n{excp.Message}";
                LogEvent(text);
                MessageBox.Show(text);
            }
        }

        /// <summary>
        /// Добавляет статус клиента в базу
        /// </summary>
        /// <param name="type">Тип (статуса клиента)</param>
        private void AddClientStatus(Type type)
        {

            // новый статус
            Clients.ClientStatus newStatus;

            // определяем тип нового статуса (физ. или юр. лица) и создаём новый статус            
            if (type == typeof(Clients.IndividualStatus))
                newStatus = new Clients.IndividualStatus();
            else newStatus = new Clients.LegalEntityStatus();

            // ищем последний статус нужного типа, новый добавляем после него
            Clients.ClientStatus lastClientStatus = null;
            foreach (Clients.ClientStatus clientStatus in DB.ClientStatuses)
            {
                if (clientStatus.GetType() == type && clientStatus.NextClientStatus == null)
                {
                    lastClientStatus = clientStatus;
                    break;
                }
            }
            newStatus.PreviousClientStatus = lastClientStatus;

            // подготавливаем и открываем форму
            Clients.ClientStatusForm clientStatusForm = currApp.GetClientStatusForm();
            clientStatusForm.Owner = this;
            clientStatusForm.ClientStatus = newStatus;
            clientStatusForm.finishEdit += OnClientStatusAdding;
            clientStatusForm.Show();

        }

        /// <summary>
        /// Вставляет статус клиента перед указанным статусом
        /// </summary>
        /// <param name="statusAfter"></param>
        private void InsertClientStatus(Clients.ClientStatus statusAfter)
        {
            // Новый статус
            Clients.ClientStatus newStatus;

            // определяем тип нового статуса (физ. или юр. лица) и создаём новый статус
            if (statusAfter.GetType()==typeof(Clients.IndividualStatus))
                newStatus = new Clients.IndividualStatus();
            else newStatus = new Clients.LegalEntityStatus();

            newStatus.PreviousClientStatus = statusAfter.PreviousClientStatus;
            newStatus.NextClientStatus = statusAfter;
            statusAfter.PreviousClientStatus = newStatus;

            // подготавливаем и открываем форму
            Clients.ClientStatusForm clientStatusForm = currApp.GetClientStatusForm();
            clientStatusForm.Owner = this;
            clientStatusForm.ClientStatus = newStatus;
            clientStatusForm.finishEdit += OnClientStatusAdding;
            clientStatusForm.cancelEdit += OnCancelStatusEdit;
            clientStatusForm.Show();

        }

        /// <summary>
        /// Изменяет статус клиента
        /// </summary>
        /// <param name="clientStatus">Статус клиента</param>
        private void ChangeClientStatus(Clients.ClientStatus clientStatus)
        {
            Clients.ClientStatusForm clientStatusForm = currApp.GetClientStatusForm();
            clientStatusForm.Owner = this;
            clientStatusForm.ClientStatus = clientStatus;
            clientStatusForm.finishEdit += OnClientStatusFinishEditing;
            clientStatusForm.Show();
        }

        /// <summary>
        /// Удаляет статус клиента
        /// </summary>
        /// <param name="clientStatus">Удаляемый статус</param>
        private void DeleteClientStatus(Clients.ClientStatus clientStatus)
        {
            MessageBoxResult UserAnswer = MessageBox.Show($"Удалить {clientStatus.Name}?",
                "Подтверждение удаления статуса клиента",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (UserAnswer != MessageBoxResult.Yes) return;

            try
            {
                DB.RemoveClientStatus(clientStatus);
                LogEvent($"Удалён статус клиента {clientStatus.Name}");
                ListView listView = ClientStatusListView(clientStatus);
                IndividualStatusesViewSource.Filter -= IndividualStatusesViewSource_Filter;
                IndividualStatusesViewSource.Filter += IndividualStatusesViewSource_Filter;
                listView.Items.Refresh();
            }
            catch (Exception excp)
            {
                string text = $"Не удалось удалить статус клиента {clientStatus.Name}, причина: \n{excp.Message}";
            }
        }

        /// <summary>
        /// Записывает событие в лог
        /// </summary>
        /// <param name="Event">
        /// Строка, представление события
        /// </param>
        private void LogEvent(string Event)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true))
            {
                writer.WriteLine(DateTime.Now.ToShortDateString() + " " +
                DateTime.Now.ToLongTimeString() + " : " + Event);
                writer.Flush();
            }
        }

        #endregion Основные методы

        #region Вспомогательные методы

        /// <summary>
        /// Выбирает файл базы в диалоге с пользователем
        /// </summary>
        private string SelectDBFilePathInDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML files (*.xml)|*.xml|JSON files (*.json)|*.json";
            fileDialog.CheckFileExists = false;
            fileDialog.Multiselect = false;

            if ((bool)fileDialog.ShowDialog() == true)
                return fileDialog.FileName;
            else
                return "";

        }

        /// <summary>
        /// Устанавливает имя файла базы в диалоге
        /// </summary>
        private void SetDBFilePathInDialog()
        {
            string filePath = SelectDBFilePathInDialog();
            if (!String.IsNullOrEmpty(filePath))
            { 
                DB.DBFilePath = filePath;
                DB.dbSettings.SetDBFilePath(filePath);
            }
        }
        
        /// <summary>
        /// Проверяет правильность указания пути к файлу базы
        /// </summary>
        /// <param name="CheckFileExists">
        /// Булево, проверять существование файла
        /// </param>
        /// <returns>
        /// Булево, признак правильности указания пути к файлу базы
        /// </returns>
        private bool CheckDBFilePath(bool CheckFileExists = false)
        {
            if (String.IsNullOrEmpty(DB.DBFilePath))
            {
                MessageBox.Show("Не указано имя файла базы!", "Проверка имени файла",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CheckFileExists && !File.Exists(DB.DBFilePath))
            {
                MessageBox.Show("Указанный файл базы не существует или недоступен!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Определяет, что клиентских статусов указанного типа в базе более одного
        /// </summary>
        /// <param name="clientStatusType">Тип клиентского статуса</param>
        /// <returns></returns>
        private bool ClienStatusesTypeOfMoreThenOne(Type clientStatusType)
        {
            int count = 0;
            foreach (Clients.ClientStatus clientStatus in DB.ClientStatuses)
            {
                if (clientStatus.GetType() == clientStatusType) count++;
                if (count > 1) break;
            }

            return count > 1;
        }

        /// <summary>
        /// Определяет, существует ли в базе хотя бы один клиентский статус указанного типа
        /// </summary>
        /// <param name="clientStatusType"></param>
        /// <returns></returns>
        private bool ClienStatusesTypeOfExists(Type clientStatusType)
        {
            foreach (Clients.ClientStatus clientStatus in DB.ClientStatuses)
            {
                if (clientStatus.GetType() == clientStatusType) return true;
            }

            return false;
        }

        /// <summary>
        /// Определяет ListView, отображающий статусы того же типа, что и нужный статус
        /// </summary>
        /// <param name="clientStatus">Статус клиента</param>
        /// <returns></returns>
        private ListView ClientStatusListView(Clients.ClientStatus clientStatus)
        {
            if (clientStatus.GetType() == typeof(Clients.IndividualStatus))
                return IndividualStatusesListView;
            return IndividualStatusesListView;//LegalEntityStatusesListView

        }

        /// <summary>
        /// Фильтр статусов клиентов-физ. лиц
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IndividualStatusesViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (e.Item.GetType() == typeof(Clients.IndividualStatus));
        }

        /// <summary>
        /// Проверяет, что в текущий момент выделен один из статусов физ. лиц.
        /// </summary>
        /// <returns></returns>
        private bool OneOfIndividualClientStatusSelected()
        {
            return IndividualStatusesListView != null
                && IndividualStatusesListView.SelectedItems.Count == 1;
        }

        #endregion Вспомогательные методы

        #region Настройка внешнего вида

        /// <summary>
        /// Заполняет указатели на коллекции (секции) отображаемых данных - вкладки "Департаменты", "Сотрудники" и т.д.
        /// и связанные с ними элементы меню и прочие элементы.
        /// </summary>
        void InitializeDataSections()
        {

            // Общая секция со всеми вкладками. Не привязана к пункту меню
            dataSections["DataTabs"] = new UI.DataSection()
            {
                name = "DataTabs",
                elements = new List<FrameworkElement> { tcDataTabs }
            };
            
            // Секция "Департаменты"
            dataSections["Departments"] = new UI.DataSection()
            {
                name = "Departments",
                menuItem = TabItemDepartmentsVisibility,
                elements = new List<FrameworkElement> { TabItemDepartments, GridDepartments }
            };

            // Секция "Сотрудники"
            dataSections["Employees"] = new UI.DataSection()
            {
                name = "Employees",
                menuItem = TabItemEmployeesVisibility,
                elements = new List<FrameworkElement> { TabItemEmployees, GridEmployees }
            };

            // Секция "Статусы клиентов
            dataSections["ClientStauses"] = new UI.DataSection()
            {
                name = "ClientStauses",
                menuItem = TabItemClientStatusesVisibility,
                elements = new List<FrameworkElement> 
                { 
                    TabItemClientStatuses, tiIndividualStatuses, tiLegalEntityStatuses,
                    gridStatusKind
                }
            };

            // Секция "Настройки"
            dataSections["Settings"] = new UI.DataSection()
            {
                name = "Settings",
                menuItem = TabItemdbSettingsVisibility,
                elements = new List<FrameworkElement> { TabItemdbSettings, GridSettings }
            };
            
        }

        /// <summary>
        /// Настраивает видимость всех секций данных
        /// </summary>
        void SetDataSectionsVisibility()
        {
            SetDataSectionVisibility(dataSections["Departments"]);
            SetDataSectionVisibility(dataSections["Employees"]);
            SetDataSectionVisibility(dataSections["ClientStauses"]);
            SetDataSectionVisibility(dataSections["Settings"]);
        }

        /// <summary>
        /// Настраивает видимость секции данных
        /// </summary>
        /// <param name="dataSection">Секция данных</param>
        void SetDataSectionVisibility(UI.DataSection dataSection)
        {
            
            // Видимость секции по умолчанию - свёрнуто
            Visibility visibility = Visibility.Collapsed;

            // Определяем пункт меню, связанный с секцией.
            MenuItem menuItem = dataSection.menuItem;

            // По наличию своего пункта меню определяем, с какой секцией мы работаем - с частной или общей.
            // У общей нет своего пункта меню.
            if (menuItem != null)
            {
                // Определяем видимость текущей секции
                visibility = menuItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                // Устанавливаем видимость общей секции всех вкладок
                SetDataSectionVisibility(dataSections["DataTabs"]);
            }                
            else
            {
                // Определяем видимость общей секции - если хотя бы одна частная секция
                // должна быть видимой, то и общая должна быть видимой.
                foreach (UI.DataSection curDataSection in dataSections)
                {
                    // У общей секции нет своего пункта меню, по этому признаку пропускаем её проверку
                    if (curDataSection.menuItem != null && curDataSection.menuItem.IsChecked)
                    {
                        visibility = Visibility.Visible;
                        break;
                    }
                }
            }
            
            // Устанавливаем видимость всех элементов секции
            foreach (System.Windows.FrameworkElement frameworkElement in dataSection.elements)
                frameworkElement.Visibility = visibility;

        }

        #endregion Настройка внешнего вида

        #region Тесты

        private void TestsDB()
        {

            Tests tests = new Tests(DB);
            
            tests.AddTestDepartments();
            tests.AddTestEmployees();
            tests.AddClientStatuses();
            tests.AddBankProducts();
            tests.AddClients();

            //// проверка иерархии департаментов

            //Department d_1 = new Department("d_1", "Podolsk", Guid.Empty); DB.AddDepartment(d_1);
            //Department d_2 = new Department("d_2", "Podolsk", Guid.Empty); DB.AddDepartment(d_2);

            //Department d_1_1 = new Department("d_1_1", "Orehovo", d_1.id); DB.AddDepartment(d_1_1);
            //Department d_1_2 = new Department("d_1_2", "Borisovo", d_1.id); DB.AddDepartment(d_1_2);

            //Department d_1_1_1 = new Department("d_1_1_1", "Kukuevo", d_1_1.id); DB.AddDepartment(d_1_1_1);
            //Department d_1_1_2 = new Department("d_1_1_2", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_2);
            //Department d_1_1_3 = new Department("d_1_1_3", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_3);

            //Department d_1_1_1_1 = new Department("d_1_1_1_1", "Zvezduevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_1);
            //Department d_1_1_1_2 = new Department("d_1_1_1_2", "Yasenevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_2);

            //Intern i_1 = new Intern("Name_i_1", "Surname_i_1", 20, 200, d_1.id); DB.AddEmployee(i_1);
            //Specialist s_1 = new Specialist("Name_s_1", "Surname_s_1", 23, 300, d_1.id); DB.AddEmployee(s_1);
            //Manager m_1 = new Manager("Manager_m_1", "Surname_m_1", 30, 500, d_1.id); DB.AddEmployee(m_1);

            //List<Department> sub_d_1 = DB.GetSubDepartments(d_1);

            //string pdn = DB.ParentDepartmentName(d_1);
            //string pdn1 = DB.ParentDepartmentName(d_1_1);

            //Products.Deposit deposit = new Products.Deposit("Мастер годового дохода", 5, 10);
            //Products.BankAccountService bankAccountService = new Products.BankAccountService("Обслуживание счета", 0, 15);
            
            //Clients.Individual individual1 = new Clients.Individual("Иван Петрович");
            
            //individual1.FirstName = "Иван";
            //individual1.SurName = "Голиков";
            //individual1.Patronymic = "Петрович";

            //individual1.ClientManager = i_1;
            
            //BankAccounts.BankAccount bankAccount1 = new BankAccounts.BankAccount("000001", individual1, new List<Products.BankProduct>() { deposit, bankAccountService });
            //BankAccounts.BankAccountBalance bankAccountBalance1 = new BankAccounts.BankAccountBalance(bankAccount1);

            //BankOperation bankOperation1 = new BankOperations.Refill(bankAccountBalance1, 100);
            //Thread.Sleep(1);
            //BankOperation bankOperation2 = new BankOperations.Withdrawing(bankAccountBalance1, 45);

            //try 
            //{
            //    bankOperation1.Apply();
            //    //bankAccountBalance1.AddBankOperation(bankOperation1, 100);
            //}
            //catch {}
            //try {
            //    bankOperation2.Apply(); 
            //    //bankAccountBalance1.AddBankOperation(bankOperation2, 55);
            //}
            //catch { }

            //Clients.ClientStatus clientStatus = new Clients.IndividualStatus("Начинающий");
            //DB.AddClientStatus(clientStatus);

            //Clients.ClientStatus clientStatus1 = new Clients.IndividualStatus("Средний");
            //clientStatus1.PreviousClientStatus = clientStatus;
            //clientStatus.NextClientStatus = clientStatus1;
            //DB.AddClientStatus(clientStatus1);

            //Clients.ClientStatus clientStatus2 = new Clients.IndividualStatus("Опытный");
            //clientStatus2.PreviousClientStatus = clientStatus1;
            //clientStatus1.NextClientStatus = clientStatus2;
            //DB.AddClientStatus(clientStatus2);

            ////DB.RemoveClientStatus(clientStatus);

            //Clients.Individual individual2 = new Clients.Individual("Петр Сергеевич");
            ////individual2.ClientStatus = clientStatus;
            //DB.SetClientStatus(individual2, clientStatus);
            //individual2.FirstName = "Петр";
            //individual2.SurName = "Шнурков";
            //individual2.Patronymic = "Сергеевич";

            //BankAccounts.BankAccount bankAccount2 = new BankAccounts.BankAccount("000002", individual2, new List<Products.BankProduct>() { deposit });
            //BankAccounts.BankAccountBalance bankAccountBalance2 = new BankAccounts.BankAccountBalance(bankAccount2);

            //Thread.Sleep(1);

            //BankOperation bankOperationTransfer =
            //    new BankOperations.TransferBetweenAccounts(new List<BankAccounts.BankAccountBalance>() { bankAccountBalance1, bankAccountBalance2 }, 12);

            //try
            //{
            //    bankOperationTransfer.Apply();

            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Отказ в выполненении операции: " + e.Message);
            //}

            //Thread.Sleep(1);

            //BankOperation bankOperationTransferStorno = new BankOperations.TransferBetweenAccounts(bankOperationTransfer);
            //bankOperationTransferStorno.Apply();

            //for (int i = 1; i<5; i++)
            //{
            //    Thread.Sleep(1);

            //    BankOperations.ChargeForInterest chargeForInterest1 = new BankOperations.ChargeForInterest(bankAccountBalance1);
            //    chargeForInterest1.Apply();
            //}

            //Thread.Sleep(1);
            //BankOperations.Withdrawing withdrawing1 = new BankOperations.Withdrawing(bankAccountBalance1, 10);
            //try
            //{
            //    withdrawing1.Apply();
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}

            //BankOperations.ChargeForInterest LastChargeForInterest = null;
            //BankOperations.ChargeForInterest MiddleChargeForInterest = null;

            //for (int i = 1; i < 5; i++)
            //{
            //    Thread.Sleep(1);

            //    BankOperations.ChargeForInterest chargeForInterest1 = new BankOperations.ChargeForInterest(bankAccountBalance1);
            //    chargeForInterest1.Apply();

            //    Thread.Sleep(1);

            //    if (i == 3) MiddleChargeForInterest = chargeForInterest1;

            //    LastChargeForInterest = chargeForInterest1;
            //}

            //BankOperations.ChargeForInterest StornoLast = new BankOperations.ChargeForInterest(LastChargeForInterest);
            //StornoLast.Apply();

            //// Сработает исключение добавления сторно-операции к непоследней операции.
            ////Thread.Sleep(1);

            ////BankOperations.ChargeForInterest StornoMiddle = new BankOperations.ChargeForInterest(MiddleChargeForInterest);
            ////StornoMiddle.Apply();

            //Clients.LegalEntityStatus legalEntityStatus1 = new Clients.LegalEntityStatus("Начинающий партнёр");
            //DB.AddClientStatus(legalEntityStatus1);

            //Clients.LegalEntity legalEntity1 = new Clients.LegalEntity("ООО 'Копа и рогыта'");
            //legalEntity1.FullName = "Общество с неограниченной безответственностью 'Копа и рогыта'";
            //legalEntity1.INN = "000111";
            //legalEntity1.KPP = "12345";
            //legalEntity1.IsCorporate = true;
            ////legalEntity1.ClientStatus = legalEntityStatus1;
            //DB.SetClientStatus(legalEntity1, legalEntityStatus1);
            //legalEntity1.ClientManager = s_1;

            ////DB.BankProducts.Add(deposit);
            ////DB.BankProducts.Add(bankAccountService);
            //DB.AddBankProduct(deposit);
            //DB.AddBankProduct(bankAccountService);
            ////DB.Clients.Add(individual1);
            ////DB.Clients.Add(individual2);
            ////DB.Clients.Add(legalEntity1);
            //DB.AddClient(individual1);
            //DB.AddClient(individual2);
            //DB.AddClient(legalEntity1);
            
            //DB.AddAccountBalance(bankAccountBalance1);
            //DB.AddAccountBalance(bankAccountBalance2);

        }

        #endregion Тесты
        
        #region Обработчики команд

        #region Обработчики команд приложения

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            bool ConfirmOpen = true;

            if (!DB.IsEmpty)
            {
                ConfirmOpen = MessageBox.Show(this, "База не пустая, при загрузке из файла данные будут потеряны. Продолжить?",
                    "Подтверждение загрузки базы", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }

            if (ConfirmOpen)
            {
                SetDBFilePathInDialog();
                if (!String.IsNullOrEmpty(DB.DBFilePath)) LoadDB();
            }

        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDB();
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetDBFilePathInDialog();
            SaveDB();
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LogEvent("Выход из приложения");
            this.Close();
        }

        #endregion Обработчики команд приложения

        #region Обработчики команд данных базы

        private void AddDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddDepartment();
        }

        private void ChangeDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeDepartment((Department)DepListView.SelectedItem);
        }

        private void DeleteDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteDepartment((Department)DepListView.SelectedItem);
        }

        private void AddEmployee_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddEmployee();
        }

        private void ChangeEmployee_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeEmployee((Employee)EmpListView.SelectedItem);
        }

        private void DeleteEmployee_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteEmployee((Employee)EmpListView.SelectedItem);
        }

        private void AddIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddClientStatus(typeof(Clients.IndividualStatus));
        }

        private void InsertIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertClientStatus((Clients.ClientStatus)IndividualStatusesListView.SelectedItem);     
        }

        private void ChangeIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeClientStatus((Clients.IndividualStatus)IndividualStatusesListView.SelectedItem);
        }

        private void MoveUpIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveDownIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void DeleteIndividualClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteClientStatus((Clients.ClientStatus)IndividualStatusesListView.SelectedItem);
        }

        private void AddLegalEntitylClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void InsertLegalEntitylClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ChangeLegalEntitylClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveUpLegalEntitylClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MoveDownLegalEntitylClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void DeleteLegalEntityllClientStatus_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #endregion Обработчики команд данных базы

        #region Обработчики команд представления данных базы

        private void SortDepartments_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Sorting.SortingSettings sortingSettings = new Sorting.SortingSettings(
                new string[,] {
                    { "Name",           "Location" },
                    { "Наименование",   "Расположение"} },
                new List<SortDescription>(DepartmentsViewSource.SortDescriptions));
            sortingSettings.finishEdit += OnFinishEditDepartmentsSortingSettings;
            sortingSettings.ShowDialog();
        }

        private void SortEmployees_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Sorting.SortingSettings sortingSettings = new Sorting.SortingSettings(
                new string[,] { 
                    { "Name",   "Surname",  "Age",      "Salary",   "Post_int" },
                    { "Имя",    "Фамилия",  "Возраст",  "Оклад",    "Должность"} }, 
                new List<SortDescription>(EmployeesViewSource.SortDescriptions));
            sortingSettings.finishEdit += OnFinishEditEmployeesSortingSettings;
            sortingSettings.ShowDialog();
        }

        #endregion Обработчики команд представления данных базы

        #endregion Обработчики команд

        #region Доступность команд

        private void AddDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ChangeDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ChangeDepartmentCanExecute();
        }

        private bool ChangeDepartmentCanExecute()
        {
            return DepListView != null 
                && DepListView.SelectedItems.Count == 1;
        }

        private void DeleteDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DeleteDepartmentCanExecute();
        }

        private bool DeleteDepartmentCanExecute()
        {
            return DepListView != null 
                && DepListView.SelectedItems.Count == 1 
                && !DB.LinksExists((Department)DepListView.SelectedItem);
        }

        private void SortDepartments_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DB.Departments.Count > 1;
        }

        private void AddEmployee_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AddEmployeeCanExecute();
        }

        private bool AddEmployeeCanExecute()
        {
            return true;
        }

        private void ChangeEmployee_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ChangeEmployeeCanExecute();
        }

        private bool ChangeEmployeeCanExecute()
        {
            return EmpListView != null
                && EmpListView.SelectedItems.Count == 1;
        }

        private void DeleteEmployee_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DeleteEmployeeCanExecute();    
        }

        private bool DeleteEmployeeCanExecute()
        {
            return (EmpListView != null
                && EmpListView.SelectedItems.Count == 1
                && !DB.LinksExists((Employee)EmpListView.SelectedItem));
        }

        private void SortEmployees_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DB.Employees.Count > 1;
        }

        private void AddIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void InsertIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OneOfIndividualClientStatusSelected();
        }

        private void ChangeIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OneOfIndividualClientStatusSelected();
        }

        
        private void MoveUpIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClienStatusesTypeOfMoreThenOne(typeof(Clients.IndividualStatus));
        }
                
        private void MoveDownIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClienStatusesTypeOfMoreThenOne(typeof(Clients.IndividualStatus));
        }

        private void DeleteIndividualClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DeleteIndividualClientStatusCanExecute();
        }

        private bool DeleteIndividualClientStatusCanExecute()
        {
            return IndividualStatusesListView != null
                && IndividualStatusesListView.SelectedItems.Count == 1
                && !DB.LinksExists((Clients.ClientStatus)IndividualStatusesListView.SelectedItem);
        }

        private void AddLegalEntitylClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void InsertLegalEntitylClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ChangeLegalEntitylClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = true;
        }

        private void MoveUpLegalEntitylClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClienStatusesTypeOfMoreThenOne(typeof(Clients.LegalEntityStatus));
        }

        private void MoveDownLegalEntitylClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClienStatusesTypeOfMoreThenOne(typeof(Clients.LegalEntityStatus));
        }

        private void DeleteLegalEntityllClientStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ClienStatusesTypeOfExists(typeof(Clients.LegalEntityStatus));
        }

        #endregion Доступность команд

        #region Обработчики внешних событий
        private void OnDepartmentAdding(Department Dep)
        {
            DB.AddDepartment(Dep);
            LogEvent($"Добавлен департамент {Dep.Name}");
            DepListView.Items.Refresh();
        }

        private void OnDepartmentFinishEditing(Department Dep)
        {
            LogEvent($"Изменён департамент {Dep.Name}");
        }

        private void OnEmployeeAdding(Employee Emp, bool PostChanged)
        {
            DB.AddEmployee(Emp);
            LogEvent($"Добавлен сотрудник {Emp.Name}");
            EmployeesViewSource.Source = DB.Employees;
            EmpListView.Items.Refresh();
        }

        private void OnEmployeeFinishEditing(Employee Emp, bool PostChanged)
        {
            
            if (!PostChanged)
            {
                LogEvent($"Изменён сотрудник {Emp.Name}, должность не изменена.");
                return;
            }

            DB.RemoveEmployee((Employee)EmpListView.SelectedItem);
            DB.AddEmployee(Emp);

            LogEvent($"Изменён сотрудник {Emp.Name}, в т.ч. изменена должность.");

            EmpListView.Items.Refresh();

        }

        private void OnClientStatusAdding(Clients.ClientStatus clientStatus)
        {
            DB.AddClientStatus(clientStatus);
            clientStatus.PreviousClientStatus.NextClientStatus = clientStatus;
            
            LogEvent($"Добавлен статус клиента {clientStatus.Name}");
            
            ListView listView = ClientStatusListView(clientStatus);
            listView.ItemsSource = DB.ClientStatuses;
            listView.Items.Refresh();
        }

        private void OnClientStatusFinishEditing(Clients.ClientStatus clientStatus)
        {

            ListView listView = ClientStatusListView(clientStatus);
            
            LogEvent($"Изменён статус {clientStatus.Name}");

            listView.Items.Refresh();
        }
         
        private void OnCancelStatusEdit(Clients.ClientStatus clientStatus)
        {
            // восстанавливаем цепочку статусов
            clientStatus.NextClientStatus.PreviousClientStatus = clientStatus.PreviousClientStatus;
        }

        public void OnFinishEditDepartmentsSortingSettings(List<SortDescription> sortDescriptions)
        {
            DepartmentsViewSource.SortDescriptions.Clear();
            foreach (SortDescription sortDescription in sortDescriptions)
                DepartmentsViewSource.SortDescriptions.Add(sortDescription);
        }

        public void OnFinishEditEmployeesSortingSettings(List<SortDescription> sortDescriptions)
        {
            EmployeesViewSource.SortDescriptions.Clear();
            foreach (SortDescription sortDescription in sortDescriptions)
                EmployeesViewSource.SortDescriptions.Add(sortDescription);
        }


        #endregion Обработчики внешних событий
                
    }

    #region Класс - описатель собственных команд

    /// <summary>
    /// Класс собственных команд окна
    /// </summary>
    public class WindowCommands
    {

        #region Поля (объявление команд)

        public static RoutedCommand AddDepartment { get; set; }
        public static RoutedCommand ChangeDepartment { get; set; }
        public static RoutedCommand DeleteDepartment { get; set; }
        public static RoutedCommand AddEmployee { get; set; }
        public static RoutedCommand ChangeEmployee { get; set; }
        public static RoutedCommand DeleteEmployee { get; set; }
        public static RoutedCommand SortDepartments { get; set; }
        public static RoutedCommand SortEmployees { get; set; }
        public static RoutedCommand AddIndividualClientStatus { get; set; }
        public static RoutedCommand InsertIndividualClientStatus { get; set; }
        public static RoutedCommand ChangeIndividualClientStatus { get; set; }
        public static RoutedCommand MoveUpIndividualClientStatus { get; set; }
        public static RoutedCommand MoveDownIndividualClientStatus { get; set; }
        public static RoutedCommand DeleteIndividualClientStatus { get; set; }
        public static RoutedCommand AddLegalEntitylClientStatus { get; set; }
        public static RoutedCommand InsertLegalEntitylClientStatus { get; set; }
        public static RoutedCommand ChangeLegalEntitylClientStatus { get; set; }
        public static RoutedCommand MoveUpLegalEntitylClientStatus { get; set; }
        public static RoutedCommand MoveDownLegalEntitylClientStatus { get; set; }
        public static RoutedCommand DeleteLegalEntityllClientStatus { get; set; }
        public static RoutedCommand Exit { get; set; }

        #endregion Поля (объявление команд)

        #region Конструкторы (создание команд)

        static WindowCommands()
        {

            Type typeThisWindow = typeof(MainWindow);

            Exit = new RoutedCommand("Exit", typeThisWindow);

            AddDepartment = new RoutedCommand("AddDepartment", typeThisWindow);
            ChangeDepartment = new RoutedCommand("ChangeDepartment", typeThisWindow);
            DeleteDepartment = new RoutedCommand("DeleteDepartment", typeThisWindow);
            SortDepartments = new RoutedCommand("SortDepartments", typeThisWindow);

            AddEmployee = new RoutedCommand("AddEmployee", typeThisWindow);
            ChangeEmployee = new RoutedCommand("ChangeEmployee", typeThisWindow);
            DeleteEmployee = new RoutedCommand("DeleteEmployee", typeThisWindow);
            SortEmployees = new RoutedCommand("SortEmployees", typeThisWindow);

            AddIndividualClientStatus = new RoutedCommand("AddIndividualClientStatus", typeThisWindow);
            InsertIndividualClientStatus = new RoutedCommand("InsertIndividualClientStatus", typeThisWindow);
            ChangeIndividualClientStatus = new RoutedCommand("ChangeIndividualClientStatus", typeThisWindow);
            MoveUpIndividualClientStatus = new RoutedCommand("MoveUpIndividualClientStatus", typeThisWindow);
            MoveDownIndividualClientStatus = new RoutedCommand("MoveDownIndividualClientStatus", typeThisWindow);
            DeleteIndividualClientStatus = new RoutedCommand("DeleteIndividualClientStatus", typeThisWindow);

            AddLegalEntitylClientStatus = new RoutedCommand("AddLegalEntitylClientStatus", typeThisWindow);
            InsertLegalEntitylClientStatus = new RoutedCommand("InsertLegalEntitylClientStatus", typeThisWindow);
            ChangeLegalEntitylClientStatus = new RoutedCommand("ChangeLegalEntitylClientStatus", typeThisWindow);
            MoveUpLegalEntitylClientStatus = new RoutedCommand("MoveUpLegalEntitylClientStatus", typeThisWindow);
            MoveDownLegalEntitylClientStatus = new RoutedCommand("MoveDownLegalEntitylClientStatus", typeThisWindow);
            DeleteLegalEntityllClientStatus = new RoutedCommand("DeleteLegalEntityllClientStatus", typeThisWindow);

        }

        #endregion Конструкторы (создание команд)

    }

    #endregion Класс - описатель собственных команд

}
