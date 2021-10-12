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
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO;


namespace OrgDB_WPF
{
    /// <summary>
    /// Логика взаимодействия для DepartmentForm.xaml
    /// </summary>
    public partial class DepartmentForm : Window
    {

        private Department EditingDepartment = new Department(true);
        public Department Dep;

        App currApp = (App)App.Current;

        #region Главный метод

        public DepartmentForm()
        {
            InitializeComponent();

            this.DataContext = EditingDepartment;

        }

        #endregion Главный метод

        #region Обработчики событий формы

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (Dep != null) EditingDepartment = Dep.ShallowCopy();

            tbName.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingDepartment, "Name"));
            tbLocation.SetBinding(TextBox.TextProperty, Common.DBElementbinding(EditingDepartment, "Location"));
            tblParentDepartment.SetBinding(TextBlock.TextProperty, Common.DBElementbinding(EditingDepartment, "ParentName"));
            
        }

        #endregion Обработчики событий формы

        #region Основные методы

        private void Save()
        {
            Close();

            if (Dep == null) Dep = new Department(EditingDepartment.Name, EditingDepartment.Location, EditingDepartment.ParentId);
            else
            {
                Dep.Name = EditingDepartment.Name;
                Dep.Location = EditingDepartment.Location;
                Dep.ParentId = EditingDepartment.ParentId;

            }

            Dep.ParentName = EditingDepartment.ParentName;

            if (finishEdit != null) finishEdit(Dep);
        }

        private void Cancel()
        {
            Close();
        }

        private void ParentDepartmentChoise()
        {
            if (startParentDepartmentChoise != null) startParentDepartmentChoise(this);
        }

        private void OpenParentDepartment()
        {
            DepartmentForm parentDepartmentForm = currApp.GetDepartmentForm();
            parentDepartmentForm.Owner = this;
            parentDepartmentForm.Dep = currApp.DB.GetDepartment(EditingDepartment.ParentId);
            parentDepartmentForm.finishEdit += OnParentDepartmentFinishEditing;
            parentDepartmentForm.Show();
        }

        private void ClearParentDepartment()
        {
            EditingDepartment.ParentId = Guid.Empty;
            EditingDepartment.ParentName = "";
        }

        #endregion Основные методы

        #region События формы

        /// <summary>
        /// Событие завершения редактирования
        /// </summary>
        /// <param name="department"></param>
        public delegate void FinishEdit(Department department);
        public event FinishEdit finishEdit;

        /// <summary>
        /// Событие начала выбора головного департамента
        /// </summary>
        /// <param name="department"></param>
        public delegate void StartParentDepartmentChoise(DepartmentForm window);
        public event StartParentDepartmentChoise startParentDepartmentChoise;

        #endregion События формы      
        
        #region Обработчики событий

        public void OnParentDepartmentSelected(Department[] departments)
        {
            if (departments.Length == 1)
            {
                EditingDepartment.ParentId = departments[0].id;
                EditingDepartment.ParentName = departments[0].Name;
            }
        }

        private void OnParentDepartmentFinishEditing(Department parentDep)
        {
            EditingDepartment.ParentId = parentDep.id;
            EditingDepartment.ParentName = parentDep.Name;
        }

        #endregion Обработчики событий

        #region Обработчики команд

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel();
        }

        private void StartParentDepartmentChoise_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ParentDepartmentChoise();
        }

        private void OpenParentDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenParentDepartment();
        }

        private void ClearParentDepartment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearParentDepartment();
        }

        private void Test_JsonSerialize_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.CheckFileExists = false;
            fileDialog.Multiselect = false;

            if ((bool)fileDialog.ShowDialog() == true)
            {
                string js = JsonConvert.SerializeObject(Dep, Formatting.Indented, new DepartmentJsonConverter());
                File.WriteAllText(fileDialog.FileName, js);
            }            

        }

        private void Test_JsonDeserialize_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "JSON files (*.json)|*.json";
            fileDialog.CheckFileExists = false;
            fileDialog.Multiselect = false;

            if ((bool)fileDialog.ShowDialog() == true)
            {
                string js = File.ReadAllText(fileDialog.FileName);
                Department testDep = JsonConvert.DeserializeObject<Department>(js, new DepartmentJsonConverter());
            }

        }

        #endregion Обработчики команд

        #region Доступность команд

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(EditingDepartment.Name) && !String.IsNullOrEmpty(EditingDepartment.Location);
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StartParentDepartmentChoise_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenParentDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditingDepartment.ParentId != Guid.Empty;
        }

        private void ClearParentDepartment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Test_JsonSerialize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Test_JsonDeserialize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion Доступность команд

    }

    #region Класс - описатель собственных команд

    public class DepartmentWindowCommands
    {
        public static RoutedCommand Save;
        public static RoutedCommand Cancel;

        public static RoutedCommand StartParentDepartmentChoise;
        public static RoutedCommand OpenParentDepartment;
        public static RoutedCommand ClearParentDepartment;

        public static RoutedCommand Test_JsonSerialize;
        public static RoutedCommand Test_JsonDeserialize;

        static DepartmentWindowCommands()
        {

            Type typeThisWindow = typeof(DepartmentForm);

            Save = new RoutedCommand("Command_Save", typeThisWindow);
            Cancel = new RoutedCommand("Command_Cancel", typeThisWindow);

            StartParentDepartmentChoise = new RoutedCommand("Command_StartParentDepartmentChoise", typeThisWindow);
            OpenParentDepartment = new RoutedCommand("Command_OpenParentDepartment", typeThisWindow);
            ClearParentDepartment = new RoutedCommand("Command_ClearParentDepartment", typeThisWindow);

            Test_JsonSerialize = new RoutedCommand("Command_Test_JsonSerialize", typeThisWindow);
            Test_JsonDeserialize = new RoutedCommand("Command_Test_JsonDeserialize", typeThisWindow);

        }
    } 

    #endregion Класс - описатель собственных команд

}
