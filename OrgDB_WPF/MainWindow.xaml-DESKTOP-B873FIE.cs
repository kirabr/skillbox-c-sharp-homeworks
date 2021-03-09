using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        DataBase DB;

        #endregion Поля

        #region Главный метод
        public MainWindow()
        {

            LogEvent("Запуск приложения");

            DB = currApp.DB;

            InitializeComponent();

            this.DataContext = DB;

            TestsDB();

            DepListView.ItemsSource = DB.Departments;
            EmpListView.ItemsSource = DB.Employees;
        }

        #endregion Главный метод

        #region Обработчики событий формы

        private void Window_Closed(object sender, EventArgs e)
        {
            LogEvent("Закрытие приложения");
        }

        #endregion Обработчики событий формы

        #region Обработчики событий элементов формы

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
            if (e.Key == Key.Enter && ChangeEmployeeCanExecute()) ChangeEmployee((Employee)EmpListView.SelectedItem);
            if (e.Key == Key.Delete && DeleteEmployeeCanExecute()) DeleteEmployee((Employee)EmpListView.SelectedItem);
        }

        private void EmpListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ChangeEmployeeCanExecute()) ChangeEmployee((Employee)EmpListView.SelectedItem);
        }        

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
            
            DepListView.ItemsSource = DB.Departments;
            DepListView.Items.Refresh();

            EmpListView.ItemsSource = DB.Employees;
            EmpListView.Items.Refresh();

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
                DB.DBFilePath = filePath;
        }

        #endregion Вспомогательные методы

        #region Тесты

        private void TestsDB()
        {
            // проверка иерархии департаментов

            Department d_1 = new Department("d_1", "Podolsk", Guid.Empty); DB.AddDepartment(d_1);
            Department d_2 = new Department("d_2", "Podolsk", Guid.Empty); DB.AddDepartment(d_2);

            Department d_1_1 = new Department("d_1_1", "Orehovo", d_1.id); DB.AddDepartment(d_1_1);
            Department d_1_2 = new Department("d_1_2", "Borisovo", d_1.id); DB.AddDepartment(d_1_2);

            Department d_1_1_1 = new Department("d_1_1_1", "Kukuevo", d_1_1.id); DB.AddDepartment(d_1_1_1);
            Department d_1_1_2 = new Department("d_1_1_2", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_2);
            Department d_1_1_3 = new Department("d_1_1_3", "Shuruevo", d_1_1.id); DB.AddDepartment(d_1_1_3);

            Department d_1_1_1_1 = new Department("d_1_1_1_1", "Zvezduevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_1);
            Department d_1_1_1_2 = new Department("d_1_1_1_2", "Yasenevo", d_1_1_1.id); DB.AddDepartment(d_1_1_1_2);

            Intern i_1 = new Intern("Name_i_1", "Surname_i_1", 20, 200, d_1.id); DB.AddEmployee(i_1);
            Specialist s_1 = new Specialist("Name_s_1", "Surname_s_1", 23, 300, d_1.id); DB.AddEmployee(s_1);
            Manager m_1 = new Manager("Manager_m_1", "Surname_m_1", 30, 500, d_1.id); DB.AddEmployee(m_1);

            List<Department> sub_d_1 = DB.GetSubDepartments(d_1);

            string pdn = DB.ParentDepartmentName(d_1);
            string pdn1 = DB.ParentDepartmentName(d_1_1);

            DB.dbSettings.ManagerSalaryPercent = 20;

            //Department d_r_1 = new Department(;

        }

        #endregion Тесты

        #region Вспомогательные методы

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

        #endregion Вспомогательные методы

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

        #endregion Обработчики команд данных базы

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

            AddEmployee = new RoutedCommand("AddEmployee", typeThisWindow);
            ChangeEmployee = new RoutedCommand("ChangeEmployee", typeThisWindow);
            DeleteEmployee = new RoutedCommand("DeleteEmployee", typeThisWindow);

        }

        #endregion Конструкторы (создание команд)

    }

    #endregion Класс - описатель собственных команд

}
