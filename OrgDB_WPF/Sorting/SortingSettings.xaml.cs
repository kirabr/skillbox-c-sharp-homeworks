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
using System.ComponentModel;

namespace OrgDB_WPF.Sorting
{
    /// <summary>
    /// Логика взаимодействия для SortingSettings.xaml
    /// </summary>
    public partial class SortingSettings : Window
    {


        #region Поля

        // Возможные поля сортировки.
        string[,] availableSortFields;

        // Постоянные списки - имена и представления полей сортировки
        List<string> availableSortFieldsNames;
        List<string> availableSortFieldsPresentations;

        // Неиспользуемые сейчас поля сортировки - содержимое левого списка
        List<string> unUsedSortFields;
        // Неиспользуемые сейчас поля сортировки-представления
        List<string> unUsedSortFieldsPresentations;

        // Используемые описания сортировки - содержимое правого списка
        List<SortFieldDesription> sortFieldDescriptions;

        #endregion Поля

        #region Конструкторы

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public SortingSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Конструктор по полям сортировки
        /// </summary>
        /// <param name="AvailableSortFields">Массив, размерность [x,2] В первой колонке содержит имена полей сортировки, во второй - представления</param>
        public SortingSettings(string[,] AvailableSortFields) : this(AvailableSortFields, new List<SortDescription>()) { }

        /// <summary>
        /// Конструктор по полям сортировки и описаниям сортировки
        /// </summary>
        /// <param name="AvailableSortFields">Массив, размерность [x,2] В первой колонке содержит имена полей сортировки, во второй - представления</param>
        /// <param name="SortDescriptions">Описания полей сортировки</param>
        public SortingSettings(string[,] AvailableSortFields,
            List<SortDescription> SortDescriptions) : this()
        {
            
            // исходный массив - имена и описания полей сортировки
            availableSortFields = AvailableSortFields;

            // Иницилизируем и заполняем списки неиспользуемых полей сортировки (содержимое левой части),
            // а так же списки всех полей сортировки (для обратного перемещения полей из правой части в левую)
            unUsedSortFields = new List<string>();
            unUsedSortFieldsPresentations = new List<string>();

            availableSortFieldsNames = new List<string>();
            availableSortFieldsPresentations = new List<string>();

            int i;

            for (i = 0; i < AvailableSortFields.Length / 2; i++)
            {

                availableSortFieldsNames.Add(AvailableSortFields[0, i]);
                availableSortFieldsPresentations.Add(AvailableSortFields[1, i]);

                SortDescription sda = new SortDescription(availableSortFieldsNames[i], ListSortDirection.Ascending);
                SortDescription sdd = new SortDescription(availableSortFieldsNames[i], ListSortDirection.Descending);

                if (SortDescriptions.Contains(sda) || SortDescriptions.Contains(sdd)) continue;
                
                unUsedSortFields.Add(AvailableSortFields[0, i]);
                unUsedSortFieldsPresentations.Add(AvailableSortFields[1, i]);
                                
            }

            // Устанавливаем описания полей сортировки
            sortFieldDescriptions = new List<SortFieldDesription>();
            foreach (SortDescription sortDescription in SortDescriptions)
            {

                i = availableSortFieldsNames.FindIndex((string x) => { return x == sortDescription.PropertyName; });

                sortFieldDescriptions.Add(new SortFieldDesription(new SortDescription(sortDescription.PropertyName, sortDescription.Direction),
                                 availableSortFieldsPresentations[i]));

            }

            // Обновляем списки на форме
            lb_unUsedFields.ItemsSource = unUsedSortFieldsPresentations;
            lb_sortDesriptions.ItemsSource = sortFieldDescriptions;
            
        }

        #endregion Конструкторы

        #region Служебные методы

        void MoveSortingField(sbyte direction)
        {
            // Определяем текущее положение поля сортировки в коллекции
            SortFieldDesription currSortFieldDescription = (SortFieldDesription)lb_sortDesriptions.SelectedItem;
            int currIndex = sortFieldDescriptions.IndexOf(currSortFieldDescription);

            // Рассчитываем будущее положение поля сортировки, с прокруткой (с первого на последнее место и наоборот)
            if (direction == -1)
            {
                if (currIndex == 0)
                    currIndex = sortFieldDescriptions.Count - 1;
                else
                    currIndex--;
            }
            else
            {
                if (currIndex == sortFieldDescriptions.Count - 1)
                    currIndex = 0;
                else
                    currIndex++;
            }

            // Двигаем поле сортировки - удаляем его с текущего места и вставляем в новое
            sortFieldDescriptions.Remove(currSortFieldDescription);
            sortFieldDescriptions.Insert(currIndex, currSortFieldDescription);

            // Обновляем список на форме
            lb_sortDesriptions.Items.Refresh();
        }

        #endregion Служебные методы


        #region Команды окна

        #region Обработчики команд

        private void AddSortingDescription_Executed(object sender, ExecutedRoutedEventArgs e) 
        {

            // Индекс текущего поля сортировки
            int currIndex = unUsedSortFieldsPresentations.IndexOf(lb_unUsedFields.SelectedItem.ToString());

            // Добавляем описание сортировки
            sortFieldDescriptions.Add(new SortFieldDesription(
                new SortDescription(
                    unUsedSortFields[currIndex], 
                    ListSortDirection.Ascending), unUsedSortFieldsPresentations[currIndex])
                );

            // Удаляем из неиспользованных полей сортировки
            unUsedSortFields.Remove(unUsedSortFields[currIndex]);
            unUsedSortFieldsPresentations.Remove(unUsedSortFieldsPresentations[currIndex]);

            // Обновляем списки на форме
            lb_unUsedFields.Items.Refresh();
            lb_sortDesriptions.Items.Refresh();
        }

        private void RemoveSortDescription_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            
            // Текущее описание сортировки
            SortFieldDesription currSortFieldDescription = (SortFieldDesription)lb_sortDesriptions.SelectedItem;
            
            // Удаляем из используемых полей сортировки
            sortFieldDescriptions.Remove(currSortFieldDescription);

            // Определяем индекс этого поля в исходных полях сортировки - для последующей вставки в неиспользуемые поля.
            int currIndex = availableSortFieldsNames.FindIndex((string x) => { return x == currSortFieldDescription.SortDescription.PropertyName; });

            // Определяем индекс для вставки - чтобы исходные поля сразу привести к изначальному порядку
            int pastingIndex = Math.Min(currIndex, unUsedSortFields.Count);

            // Возвращаем неиспользуемое поле сортировки на своё место
            unUsedSortFields.Insert(pastingIndex, availableSortFieldsNames[currIndex]);
            unUsedSortFieldsPresentations.Insert(pastingIndex, availableSortFieldsPresentations[currIndex]);

            // Обновляем списки на форме
            lb_unUsedFields.Items.Refresh();
            lb_sortDesriptions.Items.Refresh();

        }

        private void SwitchDirectionSortDescription_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            // Текущее описание поля сортировки
            SortFieldDesription currSortFieldDescription = ((SortFieldDesription)lb_sortDesriptions.SelectedItem).Copy();
            
            // Из поля сортировки вытащим описание сортировки в отдельную переменную,
            // т.к. иначе изменить направление не получится - будет ошибка CS1612 (не удалось изменить возвращаемое значение, т.к. оно не является переменной
            SortDescription sortDescription = currSortFieldDescription.SortDescription;

            // Определяем индекс текущего поля сортировки
            int currIndex = sortFieldDescriptions.IndexOf(currSortFieldDescription);

            // Переключаем направление сортировки
            if (currSortFieldDescription.SortDescription.Direction == ListSortDirection.Ascending)
                sortDescription.Direction = ListSortDirection.Descending;

            else
                sortDescription.Direction = ListSortDirection.Ascending;

            // Устанавливаем новое значение описание сортировки описанию поля сортировки
            currSortFieldDescription.SortDescription = sortDescription;

            // Заменяем описание поля сортировки в коллекции            
            sortFieldDescriptions[currIndex] = currSortFieldDescription;

            // Обновляем список на форме и восстанавливаем выделенное поле
            lb_sortDesriptions.Items.Refresh();
            lb_sortDesriptions.SelectedItem = currSortFieldDescription;

        }

        private void MoveSortDescriptionUp_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            MoveSortingField(-1);                        
        }

        private void MoveSortDescriptionDown_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            MoveSortingField(1);
        }

        private void OK_Executed(object sender, ExecutedRoutedEventArgs e) 
        {

            Close();

            List<SortDescription> SelectedSortDescriptions = new List<SortDescription>();
            foreach (SortFieldDesription sortFieldDesription in sortFieldDescriptions) SelectedSortDescriptions.Add(sortFieldDesription.SortDescription);
            if (finishEdit !=null) finishEdit(SelectedSortDescriptions);
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            Close();
        }

        #region События формы

        public delegate void FinishEdit(List<SortDescription> sortDescriptions);
        public event FinishEdit finishEdit;

        #endregion События формы

        #endregion Обработчики команд

        #region Доступность команд

        private void AddSortingDescription_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = lb_unUsedFields != null && lb_unUsedFields.SelectedItem != null;
        }
        private void RemoveSortDescription_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = sortFieldDescriptions != null && lb_sortDesriptions.SelectedItem != null;
        }

        private void SwitchDirectionSortDescription_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = lb_sortDesriptions !=null && lb_sortDesriptions.SelectedItem != null;
        }

        private void MoveSortDescriptionUp_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = lb_sortDesriptions != null 
                && lb_sortDesriptions.SelectedItem != null && sortFieldDescriptions.Count > 1;
        }

        private void MoveSortDescriptionDown_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = lb_sortDesriptions != null
                && lb_sortDesriptions.SelectedItem != null && sortFieldDescriptions.Count > 1;
        }

        private void OK_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = true;
        }

        private void Cancel_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
        {
            e.CanExecute = true;
        }

        #endregion Доступность команд

        #endregion Команды окна

        public struct SortFieldDesription
        {
            public SortDescription SortDescription { get; set; }
            string PropertyNamePresentation { get; set; }

            public SortFieldDesription(SortDescription sortDescription, string propertyNamePresentation) 
            { 
                SortDescription = sortDescription;
                PropertyNamePresentation = propertyNamePresentation;
            }

            public override string ToString()
            {
                string suff = (SortDescription.Direction == ListSortDirection.Ascending) ? "Возр" : "Убыв";
                return PropertyNamePresentation + " (" + suff + ")";
            }

            public SortFieldDesription Copy()
            {
                return new SortFieldDesription(this.SortDescription, this.PropertyNamePresentation);
            }

        }
                
    }

    #region Класс - описатель собственных команд

    public class SortWindowCommands
    {
        public static RoutedCommand AddSortDescription;
        public static RoutedCommand RemoveSortDescription;
        public static RoutedCommand SwitchDirectionSortDescription;
        public static RoutedCommand MoveSortDescriptionUp;
        public static RoutedCommand MoveSortDescriptionDown;
        public static RoutedCommand OK;
        public static RoutedCommand Cancel;

        static SortWindowCommands()
        {
            Type typeThisWindow = typeof(SortingSettings);

            AddSortDescription = new RoutedCommand("AddSortingDescription", typeThisWindow);
            RemoveSortDescription = new RoutedCommand("RemoveSortDescription", typeThisWindow);
            SwitchDirectionSortDescription = new RoutedCommand("SwitchDirectionSortDescription", typeThisWindow);
            MoveSortDescriptionUp = new RoutedCommand("MoveSortDescriptionUp", typeThisWindow);
            MoveSortDescriptionDown = new RoutedCommand("MoveSortDescriptionDown", typeThisWindow);
            OK = new RoutedCommand("OK", typeThisWindow);
            Cancel = new RoutedCommand("Cancel", typeThisWindow);
        }

    }

    #endregion Класс - описатель собственных команд
}
