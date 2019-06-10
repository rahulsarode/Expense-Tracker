using System;
using System.ComponentModel;

namespace Wpf_ExpenseTracker
{
    public class Expense : INotifyPropertyChanged
    {
        public string description { get; set; }
        public string category { get; set; }
        public DateTime expenseDate { get; set; }
        public double amount { get; set; }
        /*public string expenseDateInString
        {
            get => expenseDate.ToString("dd MMMM");
            set
            {
                expenseDateInString = value;
            }
        }*/

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}