using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Wpf_ExpenseTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Expense> expenses;
        ObservableCollection<Category> categories;
        Category totalBudget = new Category { name = "Total Budget", moneyAvailable = 0, moneySpent = 0, totalBudget = 0 };
        string filter = "";
        string filterByCategory = "";
        private readonly string[] categoryNames = new string[] { "Grocery", "Health", "Housing", "Personal Care", "Restaurant", "Shopping", "Others" };
        private readonly string[] categoryNamesForFilter = new string[] { "All", "Grocery", "Health", "Housing", "Personal Care", "Restaurant", "Shopping", "Others" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            expenses = DataStorage.ReadXML<ObservableCollection<Expense>>("ExpenseData.xml");
            if (expenses == null)
            {
                expenses = new ObservableCollection<Expense> { new Expense { description = "Edit Description", category = "Others", amount = 0.0, expenseDate = DateTime.Today } };
                DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
            }
            categories = GenerateCategory();
            Lbx_expenses.ItemsSource = expenses;
            Lbx_categories.ItemsSource = categories;
            Cbx_category.ItemsSource = categoryNames;
            Cbx_filterByCategory.ItemsSource = categoryNamesForFilter;
            Cbx_filterByCategory.SelectedItem = "All";
            ChangeTotalBudgetValues(totalBudget);
            UpdateTotalBudget(totalBudget);
            Tbx_filter.Text = "Enter text to Filter Expenses";
        }

        private void ChangeTotalBudgetValues(Category totalBudget)
        {
            totalBudget.moneyAvailable = GetTotalMoneyAvaiable(categories);
            totalBudget.moneySpent = GetTotalMoneySpent(categories);
            totalBudget.totalBudget = GetOverallTotalBudget(categories);
            UpdateTotalBudget(totalBudget);
        }

        private ObservableCollection<Category> GenerateCategory()
        {
            var categoryList = new ObservableCollection<Category>();
            ObservableCollection<Category> existingCategories = GetExistingCategories();
            if (existingCategories == null)
            {
                existingCategories = new ObservableCollection<Category> { new Category { name = "Grocery", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Health", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Housing", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Personal Care", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Restaurant", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Shopping", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Others", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 } };
                DataStorage.WriteXML<ObservableCollection<Category>>(existingCategories, "CategoryData.xml");
            }
            for (int i = 0; i < categoryNames.Length; i++)
            {
                string categoryName = categoryNames[i];
                Category category = (from cat in existingCategories where cat.name.Equals(categoryName) select cat).First<Category>();
                if (category != null)
                {
                    double totalBudget = category.totalBudget;
                    double moneySpent = GetTotalMoneySpentOfCategory(categoryName);
                    categoryList.Add(new Category { name = categoryName, moneyAvailable = totalBudget - moneySpent, moneySpent = moneySpent, totalBudget = totalBudget });
                }
            }
            //Category overallCategory = GetOverallCategory(existingCategories);
            //UpdateOverallCategoryDetails(overallCategory,categoryList);
            //categoryList.Add(overallCategory);
            return categoryList;
        }

        private double GetTotalMoneySpentOfCategory(string categoryName)
        {
            double moneySpent = 0;
            foreach (Expense exp in from expense in expenses where expense.category.Equals(categoryName) select expense)
            {
                moneySpent += exp.amount;
            }
            return moneySpent;
        }

        private ObservableCollection<Category> GetExistingCategories()
        {
            ObservableCollection<Category> existingCategories = DataStorage.ReadXML<ObservableCollection<Category>>("CategoryData.xml");
            return existingCategories;
        }

        private Category GetOverallCategory(ObservableCollection<Category> existingCategories)
        {
            Category overallCategory = (from cat in existingCategories where cat.name.Equals("Overall") select cat).First<Category>();
            return overallCategory;
        }

        private void UpdateOverallCategoryDetails(Category overallCategory, ObservableCollection<Category> categoryList)
        {
            double totalMoneyAvailable = GetTotalMoneyAvaiable(categoryList);
            double totalMoneySpent = GetTotalMoneySpent(categoryList);
            double overallTotalBudget = GetOverallTotalBudget(categoryList);
            overallCategory.moneyAvailable = totalMoneyAvailable;
            overallCategory.moneySpent = totalMoneySpent;
            overallCategory.totalBudget = overallTotalBudget;
            DataStorage.WriteXML<ObservableCollection<Category>>(categories, "CategoryData.xml");
            //UpdateTotalBudget(overallCategory);
            ChangeTotalBudgetValues(totalBudget);
        }

        private void UpdateTotalBudget(Category overallCategory)
        {
            Tbx_totalMoneyAvailable.Text = overallCategory.moneyAvailable.ToString();
            Tbx_totalMoneySpent.Text = overallCategory.moneySpent.ToString();
            Tbx_totalBudget.Text = overallCategory.totalBudget.ToString();
        }

        private double GetOverallTotalBudget(ObservableCollection<Category> categoryList)
        {
            double totalBudget = 0;
            foreach (var category in categoryList)
            {
                if (!category.name.Equals("Overall"))
                    totalBudget += category.totalBudget;
            }
            return totalBudget;
        }

        private double GetTotalMoneySpent(ObservableCollection<Category> categoryList)
        {
            double totalMoneySpent = 0;
            foreach (var category in categoryList)
            {
                if (!category.name.Equals("Overall"))
                    totalMoneySpent += category.moneySpent;
            }
            return totalMoneySpent;
        }

        private double GetTotalMoneyAvaiable(ObservableCollection<Category> categoryList)
        {
            double totalMoneyAvailable = 0;
            foreach (var category in categoryList)
            {
                if (!category.name.Equals("Overall"))
                    totalMoneyAvailable += category.moneyAvailable;
            }
            return totalMoneyAvailable;
        }

        private void Tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tbx_filter.Text == "Enter text to Filter Expenses")
            {
                Lbx_expenses.ItemsSource = expenses;
            }
            else
            {
                filter = Tbx_filter.Text.ToLower();
                if (filter == "")
                {
                    Lbx_expenses.ItemsSource = expenses;
                }
                else
                {
                    var results = from exp in expenses where exp.description.ToLower().Contains(filter) select exp;
                    if (results != null)
                    {
                        Lbx_expenses.ItemsSource = results;
                    }
                }
            }
        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            var exp = new Expense { description = "Please Add Description of Expense !!!", amount = 0.0, category = "Others", expenseDate = DateTime.Today.Date };
            expenses.Add(exp);
            Lbx_expenses.Items.Refresh();
            Lbx_expenses.SelectedItem = exp;
            Lbx_expenses.ScrollIntoView(exp);
            DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
        }

        private void Cbx_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Expense expense = (Expense)Lbx_expenses.SelectedItem;
            if (expense != null)
            {
                string selectedCategory = (String)Cbx_category.SelectedItem;
                expense.category = selectedCategory;
            }
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (Lbx_expenses.SelectedItem == null)
            {
                MessageBox.Show("Please select the Expense to be deleted!", "Error");
                return;
            }
            expenses.Remove(Lbx_expenses.SelectedItem as Expense);
            Tbx_filter.Text = "";
            DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
        }

        private void Dpr_expenseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Expense expense = (Expense)Lbx_expenses.SelectedItem;
            if (expense != null)
            {
                DateTime selectedDate = (DateTime)Dpr_expenseDate.SelectedDate;
                if (selectedDate != null)
                    if (selectedDate > DateTime.Today)
                    {
                        MessageBox.Show("Future Date is not allowed! Please select current date or earlier", "Invalid Date");
                        Dpr_expenseDate.SelectedDate = DateTime.Today;
                        return;
                    }
                    else
                    {
                        expense.expenseDate = selectedDate;
                    }
            }
        }

        private void Tbx_budget_TextChanged(object sender, TextChangedEventArgs e)
        {
            Category category = (Category)Lbx_categories.SelectedItem;
            if (Tbx_budget.Text != "" && Tbx_budget.Text != null)
            {
                Regex regEx = new Regex(@"^[0-9]\d{0,9}(\.\d{1,3})?%?$");
                string input = Tbx_budget.Text;
                if (regEx.IsMatch(input))
                {
                    category.totalBudget = Convert.ToDouble(input);
                    category.moneyAvailable = category.totalBudget - category.moneySpent;
                    Tbx_moneyAvailable.Text = category.moneyAvailable.ToString();
                    if (!category.name.Equals("Overall"))
                    {
                        //Category overallCategory = (from cat in categories where cat.name.Equals("Overall") select cat).First<Category>();
                        //UpdateOverallCategoryDetails(overallCategory, categories);
                        //UpdateTotalBudget(overallCategory);
                        ChangeTotalBudgetValues(totalBudget);
                    }
                    Lbx_categories.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Please enter valid Amount!", "Invalid Amount");
                }
            }
        }

        private void Lbx_expenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lbx_expenses.Items.Refresh();
            Lbx_categories.Items.Refresh();
        }

        private void Lbx_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lbx_categories.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
            DataStorage.WriteXML<ObservableCollection<Category>>(categories, "CategoryData.xml");
        }

        private void Tbx_amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            Expense expense = (Expense)Lbx_expenses.SelectedItem;
            if (expense != null)
            {
                string categoryName = expense.category;
                if (!Tbx_amount.Text.Equals(""))
                {
                    Regex regEx = new Regex(@"^[0-9]\d{0,9}(\.\d{1,3})?%?$");
                    string input = Tbx_amount.Text;
                    if (regEx.IsMatch(input))
                    {
                        expense.amount = Convert.ToDouble(input);
                        double totalMoneySpentOfCategory = GetTotalMoneySpentOfCategory(categoryName);
                        Category category = (from cat in categories where cat.name.Equals(categoryName) select cat).First<Category>();
                        if (category != null)
                        {
                            if (totalMoneySpentOfCategory > category.totalBudget)
                            {
                                category.totalBudget += (totalMoneySpentOfCategory - category.totalBudget);
                            }
                            category.moneySpent = totalMoneySpentOfCategory;
                            category.moneyAvailable = category.totalBudget - category.moneySpent;
                        }
                        Category selectedCategory = (Category)Lbx_categories.SelectedItem;
                        if (selectedCategory == category)
                        {
                            Tbx_moneySpent.Text = category.moneySpent.ToString();
                            Tbx_moneyAvailable.Text = category.moneyAvailable.ToString();
                            Tbx_budget.Text = category.totalBudget.ToString();
                        }
                        Lbx_categories.Items.Refresh();
                        Lbx_categories.ItemsSource = categories;
                        ChangeTotalBudgetValues(totalBudget);
                        UpdateTotalBudget(totalBudget);
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid Amount!", "Invalid Amount");
                    }
                }
            }

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lbx_expenses.Items.Refresh();
            Lbx_categories.Items.Refresh();
            UpdateTotalBudget(totalBudget);
        }

        private void Cbx_filterByCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbx_filterByCategory.SelectedItem == null)
            {
                Cbx_filterByCategory.SelectedItem = "All";
                return;
            }
            filterByCategory = Cbx_filterByCategory.SelectedItem.ToString();
            ObservableCollection<Expense> allExpenses = DataStorage.ReadXML<ObservableCollection<Expense>>("ExpenseData.xml");
            if (filterByCategory != "All")
            {
                ObservableCollection<Expense> filteredExpenses = new ObservableCollection<Expense>(from expense in allExpenses where expense.category.Equals(filterByCategory) select expense);
                Lbx_expenses.ItemsSource = filteredExpenses;
                Category category = (from cat in categories where cat.name.Equals(filterByCategory) select cat).First<Category>();
                Lbx_categories.SelectedItem = category;
            }
            else
            {
                Lbx_expenses.ItemsSource = allExpenses;
            }
        }
    }
}
