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
        string filter="";
        private string[] categoryNames = new string[] { "Grocery", "Health", "Housing", "Personal Care", "Restaurant", "Shopping", "Others" };

        public MainWindow()
        {
            InitializeComponent();
            //var lst = new ObservableCollection<Expense> { new Expense { description = "Edit Description", category = "Others", amount=0.0, expenseDate=DateTime.Today }, new Expense { description = "Edit Description 2", category = "Others", amount = 0.0, expenseDate = DateTime.Today } };
            //DataStorage.WriteXML<ObservableCollection<Expense>>(lst,"SampleExpense.xml");
            //var lst = new ObservableCollection<Category> { new Category { name = "Edit Description", moneyAvailable = 0.0, moneySpent=0.0, totalBudget=0.0}};
            //DataStorage.WriteXML<ObservableCollection<Category>>(lst,"CategoryData.xml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //expenses = GenerateExpenses(10);
            expenses = DataStorage.ReadXML<ObservableCollection<Expense>>("ExpenseData.xml");
            if (expenses == null)
            {
                expenses = new ObservableCollection<Expense> { new Expense { description = "Edit Description", category = "Others", amount=0.0, expenseDate=DateTime.Today } };
                DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
            }
            categories = GenerateCategory(categoryNames);
            Lbx_expenses.ItemsSource = expenses;
            Lbx_categories.ItemsSource = categories;
            Cbx_category.ItemsSource = categoryNames;
        }

        private ObservableCollection<Category> GenerateCategory(string[] categoryNames)
        {
            var categoryList = new ObservableCollection<Category>();
            ObservableCollection<Category> existingCategories = GetExistingCategories();
            if (existingCategories == null)
            {
                existingCategories = new ObservableCollection<Category> { new Category { name = "Grocery", moneyAvailable = 0.0, moneySpent=0.0, totalBudget=0.0}, new Category { name = "Health", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Housing", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Personal Care", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Restaurant", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Shopping", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Others", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }, new Category { name = "Overall", moneyAvailable = 0.0, moneySpent = 0.0, totalBudget = 0.0 }};
                DataStorage.WriteXML<ObservableCollection<Category>>(existingCategories, "CategoryData.xml");
            }
            for (int i = 0; i < categoryNames.Length; i++)
            {
                //var expensesOfCategory = new ObservableCollection<Expense>();
                string categoryName = categoryNames[i];
                Category category = (from cat in existingCategories where cat.name.Equals(categoryName) select cat).First<Category>();
                if (category !=null)
                {
                    double totalBudget = category.totalBudget;
                    double moneySpent = 0;
                    foreach (Expense exp in from expense in expenses where expense.category.Equals(categoryName) select expense)
                    {
                        moneySpent = moneySpent + exp.amount;
                    }
                    categoryList.Add(new Category { name = categoryName, moneyAvailable = totalBudget - moneySpent, moneySpent = moneySpent, totalBudget = totalBudget });
                }
            }
            Category overallCategory = GetOverallCategory(existingCategories);
            UpdateOverallCategoryDetails(overallCategory,categoryList);
            categoryList.Add(overallCategory);


            return categoryList;
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
            //Write code for writeXML here
            DataStorage.WriteXML<ObservableCollection<Category>>(categories, "CategoryData.xml");
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

        /*private ObservableCollection<Category> GenerateCategory(string[] categoryNames)
{
   var categoryList = new ObservableCollection<Category>();
   Random random = new Random();
   for (int i = 0; i < categoryNames.Length; i++)
   {

       double moneyAvailable = Math.Round(random.Next(100) * random.NextDouble(), 2);
       double moneySpent = random.Next(100);
       var categoryName = categoryNames[i];
       categoryList.Add(new Category { name = categoryName, moneyAvailable = moneyAvailable, moneySpent=moneySpent,totalBudget=moneyAvailable+moneySpent});
   }
   double totatlMoneyAvailable = getTotalMoneyAvaiable(categoryList);
   var overall = new Category { name = "Overall", moneyAvailable = totatlMoneyAvailable };
   categoryList.Add(overall);
   return categoryList;
}*/

        /*private ObservableCollection<Expense> GenerateExpenses(int count)
        {
            var expenseList = new ObservableCollection<Expense>();
            for (int i = 0; i < count; i++)
            {
                expenseList.Add(new Expense { description = "Expense : " + i, category = "Grocery", expenseDate = new DateTime(2019, 06, 03), amount = count + i });
            };
            return expenseList;
        }*/

        private double GetTotalMoneyAvaiable(ObservableCollection<Category> categoryList)
        {
            double totalMoneyAvailable=0;
            foreach(var category in categoryList)
            {
                if (!category.name.Equals("Overall"))
                    totalMoneyAvailable += category.moneyAvailable;
            }
            return totalMoneyAvailable;
        }

        private void Tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            filter = Tbx_filter.Text.ToLower();
            if (filter == "")
            {
                Lbx_expenses.ItemsSource = expenses;
            }
            else
            {
                var results = from exp in expenses where exp.description.ToLower().Contains(filter) select exp;
                Lbx_expenses.ItemsSource = results;
            }

        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            var exp = new Expense { description = "Please Add Description of Expense !!!", amount = 0.0, category="Others", expenseDate=DateTime.Today};
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
                string selectedCategory = (String) Cbx_category.SelectedItem;
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
            var temp = Tbx_budget.Text;
            if (Tbx_budget.Text != "" && Tbx_budget.Text != null)
            {
            category.totalBudget = Convert.ToDouble(Tbx_budget.Text);
            category.moneyAvailable = category.totalBudget - category.moneySpent;
            Tbx_moneyAvailable.Text = category.moneyAvailable.ToString();
            if (!category.name.Equals("Overall"))
            {
                Category overallCategory = (from cat in categories where cat.name.Equals("Overall") select cat).First<Category>();
                UpdateOverallCategoryDetails(overallCategory, categories);
            }
            Lbx_categories.Items.Refresh();
            DataStorage.WriteXML<ObservableCollection<Category>>(categories, "CategoryData.xml");
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

        private void Tbx_budget_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Category category = (Category)Lbx_categories.SelectedItem;
            category.moneyAvailable = category.totalBudget - category.moneySpent;
            Tbx_moneyAvailable.Text = category.moneyAvailable.ToString();
            if (!category.name.Equals("Overall"))
            {
                Category overallCategory = (from cat in categories where cat.name.Equals("Overall") select cat).First<Category>();
                UpdateOverallCategoryDetails(overallCategory, categories);
            }
            Lbx_categories.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataStorage.WriteXML<ObservableCollection<Expense>>(expenses, "ExpenseData.xml");
            DataStorage.WriteXML<ObservableCollection<Category>>(categories, "CategoryData.xml");
        }
    }
}
