using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml;

namespace OrgDB_WPF
{
    /// <summary>
    /// Логика взаимодействия для EmployeeForm.xaml
    /// </summary>
    public partial class EmployeeForm : Window
    {

        // Редактируемый сотрудник (общая структура)
        private EditingEmployee EditingEmployee = new EditingEmployee();
        
        // Сотрдуник (результат редактирования)
        public Employee Emp;

        // Основное приложение
        App currApp = (App)App.Current;

        #region Главный метод
        public EmployeeForm()
        {
            InitializeComponent();
            //EditingEmployee.Flush();
            this.DataContext = EditingEmployee;
        }

        #endregion Главный метод

        #region Обработчики событий формы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (Emp != null)
            {
                EditingEmployee.Name = Emp.Name;
                EditingEmployee.Surname = Emp.Surname;
                EditingEmployee.Age = Emp.Age;
                EditingEmployee.Salary = Emp.Salary;
                EditingEmployee.DepartmentID = Emp.DepartmentID;
                EditingEmployee.DepartmentName = Emp.DepartmentName;
                EditingEmployee.Post_Enum = Emp.post_Enum;
                EditingEmployee.Post = Emp.Post;
                
            }
            
            cbPost.ItemsSource = Enum.GetValues(typeof(Employee.post_enum));
            tbName.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingEmployee, "Name"));
            tbSurname.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingEmployee, "Surname"));
            tbAge.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingEmployee, "Age"));
            tbSalary.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingEmployee, "Salary"));
            tbDepartmentName.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingEmployee, "DepartmentName"));

            cbPost.SelectedIndex = (int)EditingEmployee.Post_Enum;

        }

        #endregion Обработчики событий формы

        #region Обработчики событий элементов формы
        private void cbPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditingEmployee.Post_Enum = (Employee.post_enum)((object[])e.AddedItems)[0];
            EditingEmployee.Post = EmployeePostDescriptionConverter.GetEnumDescription(EditingEmployee.Post_Enum);
        }

        #endregion Обработчики событий элементов формы

        #region События формы

        /// <summary>
        /// Событие завершения редактирования
        /// </summary>
        /// <param name="employee"></param>
        public delegate void FinishEdit(Employee employee, bool PostChanged = false);
        public event FinishEdit finishEdit;

        /// <summary>
        /// Событие начала выбора департамента
        /// </summary>
        /// <param name="department"></param>
        public delegate void StartDepartmentChoise(EmployeeForm window);
        public event StartDepartmentChoise startDepartmentChoise;

        #endregion События формы

        #region Обработчики событий
        public void OnDepartmentSelected(Department[] departments)
        {
            if (departments.Length == 1)
            {
                EditingEmployee.DepartmentID = departments[0].id;
                EditingEmployee.DepartmentName = departments[0].Name;
                tbDepartmentName.Text = $"{departments[0].Name} ({departments[0].Location})";
            }
        }

        public void OnDepartmentFinishEditing(Department department)
        {
            EditingEmployee.DepartmentID = department.id;
            EditingEmployee.DepartmentName = department.Name;
        }

        #endregion Обработчики событий

        #region Основные методы
        private void Save() 
        {

            bool PostChanged = false;
            
            if (Emp == null)
            {
                CreateEmployee();
            }
            else
            {
                if (Emp.post_Enum == EditingEmployee.Post_Enum)
                {
                    Emp.Name = EditingEmployee.Name;
                    Emp.Surname = EditingEmployee.Surname;
                    Emp.Age = EditingEmployee.Age;
                    Emp.Salary = EditingEmployee.Salary;
                    Emp.DepartmentID = EditingEmployee.DepartmentID;
                    Emp.DepartmentName = EditingEmployee.DepartmentName;
                }
                else
                {
                    PostChanged = true;
                    CreateEmployee();
                }
            }

            if (finishEdit != null)
                finishEdit(Emp, PostChanged);

            Close();

        }

        private void CreateEmployee()
        {
            switch (EditingEmployee.Post_Enum)
            {
                case Employee.post_enum.manager:
                    Emp = new Manager(EditingEmployee.Name, EditingEmployee.Surname, EditingEmployee.Age, EditingEmployee.Salary, EditingEmployee.DepartmentID);
                    break;
                case Employee.post_enum.specialist:
                    Emp = new Specialist(EditingEmployee.Name, EditingEmployee.Surname, EditingEmployee.Age, EditingEmployee.Salary, EditingEmployee.DepartmentID);
                    break;
                case Employee.post_enum.intern:
                    Emp = new Intern(EditingEmployee.Name, EditingEmployee.Surname, EditingEmployee.Age, EditingEmployee.Salary, EditingEmployee.DepartmentID);
                    break;
            }

            Emp.DepartmentName = currApp.DB.DepartmentName(EditingEmployee.DepartmentID);
        }

        private void Cancel()
        {
            Close();
        }

        private void CalculateSalary() 
        {
            EditingEmployee.Salary = currApp.DB.CalculateManagerSalary(currApp.DB.GetDepartment(EditingEmployee.DepartmentID));
        }

        private void DepartmentChoise()
        {
            if (startDepartmentChoise != null) startDepartmentChoise(this);
        }

        private void OpenDepartment() 
        {
            DepartmentForm DepartmentForm = currApp.GetDepartmentForm();
            DepartmentForm.Owner = this;
            DepartmentForm.Dep = currApp.DB.GetDepartment(EditingEmployee.DepartmentID);
            DepartmentForm.finishEdit += OnDepartmentFinishEditing;
            DepartmentForm.Show();
        }

        #endregion Основные методы
        
        #region Обработчики команд

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel();
        }

        private void CalculateSalary_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalculateSalary();
        }

        private void StartDepartmentChoise_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DepartmentChoise();
        }

        private void OpenDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenDepartment();
        }

        #endregion Обработчики команд

        #region Доступность команд

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = 
                !String.IsNullOrEmpty(EditingEmployee.Name)
                && !String.IsNullOrEmpty(EditingEmployee.Surname)
                && EditingEmployee.Age > 17
                && EditingEmployee.Salary > 0
                && EditingEmployee.DepartmentID != Guid.Empty
                && !String.IsNullOrEmpty(EditingEmployee.Post);
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CalculateSalary_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditingEmployee.Post_Enum == Employee.post_enum.manager && EditingEmployee.DepartmentID != Guid.Empty;
        }

        private void StartDepartmentChoise_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditingEmployee.DepartmentID != Guid.Empty;
        }


        #endregion Доступность команд
                
    }

    #region Класс - описатель собственных команд

    public class EmployeeWindowCommands
    {
        public static RoutedCommand Save;
        public static RoutedCommand Cancel;

        public static RoutedCommand CalculateSalary;

        public static RoutedCommand StartDepartmentChoise;
        public static RoutedCommand OpenDepartment;

        static EmployeeWindowCommands()
        {

            Type typeThisWindow = typeof(DepartmentForm);

            Save = new RoutedCommand("Save", typeThisWindow);
            Cancel = new RoutedCommand("Cancel", typeThisWindow);

            CalculateSalary = new RoutedCommand("CalculateSalary", typeThisWindow);

            StartDepartmentChoise = new RoutedCommand("StartDepartmentChoise", typeThisWindow);
            OpenDepartment = new RoutedCommand("OpenDepartment", typeThisWindow);

        }
    }

    #endregion Класс - описатель собственных команд
}


