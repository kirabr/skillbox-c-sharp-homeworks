using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OrgDB_WPF
{
    /// <summary>
    /// Логика взаимодействия для DepartmentChoiseForm.xaml
    /// </summary>
    public partial class DepartmentChoiseForm : Window
    {

        #region Поля

        // Департаменты, отображаемые в списке
        public ReadOnlyCollection<Department> Departments;

        // Департаменты, действия над которыми запрещены
        public List<Department> ExceptDepartments;

        // Закрывать форму после выбора
        public bool CloseAfterChoise = true;

        // Разрешить множественный выбор
        public bool EnableMultiSelect = false;

        #endregion Поля

        #region Конструкторы

        public DepartmentChoiseForm()
        {
            InitializeComponent();
            this.DataContext = Departments;
        }

        #endregion Конструкторы

        #region События

        // Событие выбора департаментов
        public delegate void DepartmentsSelected(Department[] departments);
        public event DepartmentsSelected departmentsSelected;

        #endregion События

        #region Обработчики событий формы

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepListView.ItemsSource = Departments;
        }

        #endregion Обработчики событий формы

        #region Обработчики событий элементов формы

        private void DepListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectDepartmentsCanExecute()) SelectDepartments();
        }

        private void DepListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SelectDepartmentsCanExecute()) SelectDepartments();
        }

        #endregion Обработчики событий элементов формы

        #region Основные методы

        private void SelectDepartments()
        {

            if (departmentsSelected != null)
            {
                Department[] selectedDepartments = new Department[DepListView.SelectedItems.Count];
                for (int i = 0; i < DepListView.SelectedItems.Count; i++)
                {
                    selectedDepartments[i] = (Department)DepListView.SelectedItems[i];
                }
                departmentsSelected(selectedDepartments);
            }
            if (CloseAfterChoise) Close();

        }

        private void Cancel()
        {
            Close();
        }

        #endregion Основные методы

        #region Обработчики команд

        private void SelectDepartments_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            SelectDepartments();

        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel();
        }

        #endregion Обработчики команд

        #region Доступность команд

        private void SelectDepartments_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectDepartmentsCanExecute();
        }

        private bool SelectDepartmentsCanExecute() 
        {
            if (DepListView == null || DepListView.SelectedItems.Count == 0) return false;

            if (DepListView != null && !EnableMultiSelect && DepListView.SelectedItems.Count > 1) return false;

            foreach (Department Dep in DepListView.SelectedItems)
            {
                if (ExceptDepartments.Contains(Dep)) return false;
            }

            return true;
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CancelCanExecute(); 
        }

        private bool CancelCanExecute()
        {
            return true;
        }

        #endregion Доступность команд
               
    }

    #region Класс - описатель собственных команд

    public class DepartmentChoiseWindowCommands
    {

        public static RoutedCommand SelectDepartments;
        public static RoutedCommand Cancel;

        static DepartmentChoiseWindowCommands()
        {
            Type typeThisWindow = typeof(DepartmentChoiseForm);

            SelectDepartments = new RoutedCommand("SelectDepartments", typeThisWindow);
            Cancel = new RoutedCommand("Cancel", typeThisWindow);
        }

    } 

    #endregion Класс - описатель собственных команд

}
