using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace DNDCombatTracker
{
    public partial class DamageCharDialog : Window
    {
        public int DamageAmt { get; set; }

        public DamageCharDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void DamageAmtInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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
