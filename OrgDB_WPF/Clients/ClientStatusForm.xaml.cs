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

namespace OrgDB_WPF.Clients
{
    /// <summary>
    /// Логика взаимодействия для ClientStatusForm.xaml
    /// </summary>
    public partial class ClientStatusForm : Window
    {

        // Редактируемый статус - клон обслуживаемого формой статуса
        private ClientStatus editingClienStatus;

        // Основное приложение
        App currApp = (App)App.Current;

        // Обслуживаемый формой статус
        public ClientStatus ClientStatus;
        
        // Конструктор этой формы
        public ClientStatusForm()
        {
            InitializeComponent();
            DataContext = editingClienStatus;
        }

        #region События

        /// <summary>
        /// Событие завершения редактирования
        /// </summary>
        /// <param name="clientStatus">Статус клиента</param>
        public delegate void FinishEdit(ClientStatus clientStatus);
        public event FinishEdit finishEdit;

        /// <summary>
        /// Событие отмены редактирования
        /// </summary>
        /// <param name="clientStatus"></param>
        public delegate void CancelEdit(ClientStatus clientStatus);
        public event CancelEdit cancelEdit;

        #endregion События

        #region Основные методы

        /// <summary>
        /// Сохраняет статус клиента и закрывает форму
        /// </summary>
        private void Save()
        {

            Close();
            
            ClientStatus.Name = editingClienStatus.Name;
            ClientStatus.PreviousClientStatus = editingClienStatus.PreviousClientStatus;
            ClientStatus.NextClientStatus = editingClienStatus.NextClientStatus;
            ClientStatus.CreditDiscountPercent = editingClienStatus.CreditDiscountPercent;
            ClientStatus.DepositAdditionalPercent = editingClienStatus.DepositAdditionalPercent;

            finishEdit?.Invoke(ClientStatus);

        }

        /// <summary>
        /// Закрывает форму без сохранения статуса клиента
        /// </summary>
        private void Cancel()
        {
            Close();

            cancelEdit?.Invoke(ClientStatus);
        }

        private void OpenClientStatus(ClientStatus clientStatus)
        {
            // подготавливаем и открываем форму
            ClientStatusForm clientStatusForm = currApp.GetClientStatusForm(clientStatus);
            // владельцем новой формы делаем владельца этой формы (основное окно приложения).
            // таким образом избавляемся от каскада владельцев - все окна статусов подчинены основному окну
            clientStatusForm.Owner = Owner;
            clientStatusForm.ClientStatus = clientStatus;
            clientStatusForm.finishEdit += OnClientStatusFinishEditing;
            clientStatusForm.Show();
            clientStatusForm.Activate();
        }

        #endregion Основные методы

        #region Обработчики событий

        /// <summary>
        /// Обработчик события завершения редактирования клиентского статуса (предыдущего или следующего)
        /// </summary>
        /// <param name="clientStatus">Статус клиента</param>
        private void OnClientStatusFinishEditing(ClientStatus clientStatus) => finishEdit?.Invoke(clientStatus);

        #endregion Обработчики событий

        #region Обработчики событий формы

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (ClientStatus.GetType() == typeof(IndividualStatus))
            { 
                Title = "Статус физического лица";
                editingClienStatus = new IndividualStatus();
            }
            else if (ClientStatus.GetType() == typeof(LegalEntityStatus))
            { 
                Title = "Статус юридического лица";
                editingClienStatus = new LegalEntityStatus();
            }

            editingClienStatus.Name = ClientStatus.Name;
            editingClienStatus.PreviousClientStatus = ClientStatus.PreviousClientStatus;
            editingClienStatus.NextClientStatus = ClientStatus.NextClientStatus;
            editingClienStatus.CreditDiscountPercent = ClientStatus.CreditDiscountPercent;
            editingClienStatus.DepositAdditionalPercent = ClientStatus.DepositAdditionalPercent;

            tbName.SetBinding(TextBox.TextProperty, Common.DBElementbinding(editingClienStatus, "Name"));
            tbPreviousClientStatus.SetBinding(TextBox.TextProperty, Common.DBElementbinding(editingClienStatus, "PreviousClientStatus.Name"));
            tbNextClientStatus.SetBinding(TextBox.TextProperty, Common.DBElementbinding(editingClienStatus, "NextClientStatus.Name"));
            tbCreditDiscountPercent.SetBinding(TextBox.TextProperty, Common.DBElementbinding(editingClienStatus, "CreditDiscountPercent"));
            tbDepositAdditionalPercent.SetBinding(TextBox.TextProperty, Common.DBElementbinding(editingClienStatus, "DepositAdditionalPercent"));

        }

        #endregion Обработчики событий формы

        #region Обработчики команд

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel();
        }

        private void OpenPrevious_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenClientStatus(editingClienStatus.PreviousClientStatus);
        }

        private void OpenNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenClientStatus(editingClienStatus.NextClientStatus);
        }

        #endregion Обработчики команд

        #region Доступность команд

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = editingClienStatus!=null && !String.IsNullOrEmpty(editingClienStatus.Name);
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenPrevious_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = editingClienStatus!=null 
                && editingClienStatus.PreviousClientStatus != null;
        }

        private void OpenNext_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = editingClienStatus != null
                && editingClienStatus.NextClientStatus != null;
        }

        #endregion Доступность команд

    }

    #region Класс-описатель собственных команд
    public class ClientStatusWindowsCommands
    {
        public static RoutedCommand Save;
        public static RoutedCommand Cancel;

        public static RoutedCommand OpenPrevious;
        public static RoutedCommand OpenNext;

        static ClientStatusWindowsCommands()
        {
            Type typeThisWindow = typeof(ClientStatusForm);

            Save = new RoutedCommand("Command_Save", typeThisWindow);
            Cancel = new RoutedCommand("Command_Cancel", typeThisWindow);

            OpenPrevious = new RoutedCommand("OpenPrevious", typeThisWindow);
            OpenNext = new RoutedCommand("OpenNext", typeThisWindow);
        }

    }

    #endregion Класс-описатель собственных команд

}
