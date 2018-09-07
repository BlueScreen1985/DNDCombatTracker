using System.Windows;

namespace DNDCombatTracker
{
    public partial class AddLogEntryDialog : Window
    {
        public string LogEntry { get; set; }

        public AddLogEntryDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
