using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DNDCombatTracker
{
    public partial class AttackDialog : Window
    {
        // TODO: This should be an observable collection
        public ObservableCollection<Character> PossibleTargets { get; set; }
        public int DamageAmt { get; set; }

        public AttackDialog(List<Character> targets)
        {
            InitializeComponent();
            DataContext = this;
            PossibleTargets = new ObservableCollection<Character>(targets);
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void DamageAmtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9-]+"); // Regex matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
