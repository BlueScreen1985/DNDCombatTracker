using System.ComponentModel;
using System.Windows;

namespace DNDCombatTracker
{
    public partial class DeathSaveDialog : Window, INotifyPropertyChanged
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

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DeathSaveDialog(string characterName)
        {
            InitializeComponent();
            DataContext = this;
            CharacterName = characterName;
        }

        private void ButtonSuccess_Click(object sender, RoutedEventArgs e) => DialogResult = true;
        private void ButtonFail_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
