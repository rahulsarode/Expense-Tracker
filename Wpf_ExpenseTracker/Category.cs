using System.ComponentModel;

namespace Wpf_ExpenseTracker
{
    public class Category : INotifyPropertyChanged
    {
        public string name{ get; set; }
        public double moneyAvailable { get; set; }
        public double moneySpent { get; set; }
        public double totalBudget { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}