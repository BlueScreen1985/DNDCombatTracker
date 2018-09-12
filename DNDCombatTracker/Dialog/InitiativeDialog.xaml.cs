using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DNDCombatTracker
{
    public partial class InitiativeDialog : Window, INotifyPropertyChanged
    {
        private string characterName;
        public string CharacterName
        {
            get => characterName;
            private set
            {
                characterName = value;
                NotifyPropertyChanged("CharacterName");
            }
        }
        public int? Initiative { get; set; } = null;

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public InitiativeDialog(string characterName)
        {
            InitializeComponent();
            DataContext = this;
            CharacterName = characterName;
        }

        private void FilterNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9-]+"); // Regex matches disallowed text
            return !regex.IsMatch(text);
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
