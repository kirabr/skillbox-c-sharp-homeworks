using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OrgDB_WPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // База данных, облсуживаемая этим приложением
        public DataBase DB = new DataBase();
        // Список департаментов-исключений (используется в служебных целях)
        public List<Department> ExceptDepartments = new List<Department>();
        
        /// <summary>
        /// Выполняет базовую подготовку формы департамента и возвращает её
        /// </summary>
        /// <returns></returns>
        public DepartmentForm GetDepartmentForm()
        {
            // Получаем форму департамента
            DepartmentForm departmentForm = new DepartmentForm();
            
            // Добавляем обработчик события начала выбора родительского департамента в форме департамента
            departmentForm.startParentDepartmentChoise += OnStartParentDepartmentChoise;

            // Форма департамента готова, возращаем её.
            return departmentForm;

        }

        /// <summary>
        /// Выполняем базовую подготовку формы выбора департамента и возвращает её
        /// </summary>
        /// <returns></returns>
        private DepartmentChoiseForm GetDepartmentChoiseForm()
        {
            DepartmentChoiseForm departmentChoiseForm = new DepartmentChoiseForm();
            departmentChoiseForm.Departments = DB.Departments;
            departmentChoiseForm.ExceptDepartments = ExceptDepartments;

            return departmentChoiseForm;
        }

        /// <summary>
        /// Обработчик события начала выбора головного департамента (для формы департамента)
        /// </summary>
        /// <param name="window"></param>
        private void OnStartParentDepartmentChoise(DepartmentForm window)
        {
            DepartmentChoiseForm departmentChoiseForm = GetDepartmentChoiseForm();
            departmentChoiseForm.Owner = window;
            departmentChoiseForm.departmentsSelected += window.OnParentDepartmentSelected;

            departmentChoiseForm.Show();
        }
        
        /// <summary>
        /// Выполняет базовую подготовку формы сотрудника и возвращает её
        /// </summary>
        /// <returns></returns>
        public EmployeeForm GetEmployeeForm()
        {
            EmployeeForm employeeForm = new EmployeeForm();

            employeeForm.startDepartmentChoise += OnStartDepartmentChoise;

            return employeeForm;
        }

        /// <summary>
        /// Обработчик события начала выбора департамента (для формы сотрудника)
        /// </summary>
        /// <param name="window"></param>
        private void OnStartDepartmentChoise(EmployeeForm window)
        {
            DepartmentChoiseForm departmentChoiseForm = GetDepartmentChoiseForm();
            departmentChoiseForm.Owner = window;
            departmentChoiseForm.departmentsSelected += window.OnDepartmentSelected;

            departmentChoiseForm.Show();
        }

        /// <summary>
        /// Возвращает форму статуса клиента
        /// </summary>
        /// <returns></returns>
        public Clients.ClientStatusForm GetClientStatusForm()
        {
            Clients.ClientStatusForm clientStatusForm = new Clients.ClientStatusForm();
            return clientStatusForm;
        }

        public Clients.ClientStatusForm GetClientStatusForm(Clients.ClientStatus clientStatus)
        {
            foreach (Window curWindow in Windows)
            {
                if (curWindow.GetType() == typeof(Clients.ClientStatusForm) && ((Clients.ClientStatusForm)curWindow).ClientStatus == clientStatus)
                    return (Clients.ClientStatusForm)curWindow;
            }

            return GetClientStatusForm();

        }


    }
}
