using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DNDCombatTracker
{
    public partial class AddCharacterDialog : Window
    {
        public string CharacterName { get; set; }
        public int Initiative { get; set; }
        public int ArmorClass { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public bool IsPlayerCharacter { get; set; }

        public AddCharacterDialog(Visibility initiativeBoxVisibility = Visibility.Visible)
        {
            InitializeComponent();
            DataContext = this;
            InitiativeBox.Visibility = initiativeBoxVisibility;
            InitiativeBoxLabel.Visibility = initiativeBoxVisibility;
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
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
