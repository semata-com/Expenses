using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Expenses
{
    public class DateToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime d)
            {
                value = d.ToString("d");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

    }
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Expenses.ExpensesDataStoreView mainView;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            //            mainView = new ExpensesDataStoreView(x => Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => x())); 
            mainView = new Expenses.ExpensesDataStoreView(null);

            var expensesDataStoreFile = "Expenses.ds";
            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var expensesDataStorePath = localFolder.Path + @"\" + expensesDataStoreFile;
            bool dataStoreExists = false;
            try
            {
                await localFolder.GetFileAsync(expensesDataStoreFile);
                dataStoreExists = true;
            }
            catch
            {
            }

            if (!dataStoreExists)
            {
                (new Expenses.ExpensesDataStore(null)).Create(expensesDataStorePath);
            }

            mainView.Open(expensesDataStorePath);
            this.DataContext = mainView;

        }

        bool ActivityDataGridInEdit = false;
        bool ExpenseDataGridInEdit = false;

        private void EnableDisable()
        {
            var editing = ActivityDataGridInEdit || ExpenseDataGridInEdit;
            AddActivityButton.IsEnabled = !editing;
            DeleteActivityButton.IsEnabled = !editing && ActivityDataGrid.SelectedItem != null;
            AddExpenseButton.IsEnabled = !editing && ActivityDataGrid.SelectedItem != null;
            DeleteExpenseButton.IsEnabled = !editing && ExpenseDataGrid.SelectedItem != null;
            ActivityDataGrid.IsEnabled = !ExpenseDataGridInEdit;
            ExpenseDataGrid.IsEnabled = !ActivityDataGridInEdit;
        }

        private void ActivityDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            EnableDisable();
        }

        private void ActivityDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableDisable();
        }

        private void ActivityDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ActivityDataGridInEdit = true;
            EnableDisable();
        }

        private void ActivityDataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel
                && ((ActivityView)e.Row.DataContext).IsNew)
            {
                mainView.EditableActivityItems.Remove((ActivityView)e.Row.DataContext);
            }
            ActivityDataGridInEdit = false;
            EnableDisable();
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            var activityView = mainView.EditableActivityItems.AddNew();
            ActivityDataGrid.SelectedItem = activityView;
            ActivityDataGrid.BeginEdit();
        }
        private void DeleteActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActivityDataGrid.SelectedItem != null)
            {
                mainView.EditableActivityItems.Remove((ActivityView)ActivityDataGrid.SelectedItem);
            }
            EnableDisable();
        }

        private void ExpenseDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            EnableDisable();
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableDisable();
        }

        private void ExpenseDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ExpenseDataGridInEdit = true;
            EnableDisable();
        }

        private void ExpenseDataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel
                && ((ExpenseView)e.Row.DataContext).IsNew)
            {
                var activityView = (ActivityView)ActivityDataGrid.SelectedItem;
                activityView.ExpensesIncurred.Remove((ExpenseView)ExpenseDataGrid.SelectedItem);
            }
            ExpenseDataGridInEdit = false;
            EnableDisable();
        }


        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            var activityView = (ActivityView)ActivityDataGrid.SelectedItem;
            var expenseView = activityView.ExpensesIncurred.AddNew();
            ExpenseDataGrid.SelectedItem = expenseView;
            ExpenseDataGrid.BeginEdit();
        }

        private void DeleteExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExpenseDataGrid.SelectedItem != null)
            {
                var activityView = (ActivityView)ActivityDataGrid.SelectedItem;
                activityView.ExpensesIncurred.Remove((ExpenseView)ExpenseDataGrid.SelectedItem);
            }
            EnableDisable();
        }
    }
}
