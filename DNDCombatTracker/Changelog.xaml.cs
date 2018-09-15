using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DNDCombatTracker
{
    public partial class Changelog : Window, INotifyPropertyChanged
    {
        private string changelogContent;
        public string ChangelogContent
        {
            get => changelogContent;
            private set
            {
                changelogContent = value;
                NotifyPropertyChanged("VersionString");
            }
        }

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Changelog()
        {
            InitializeComponent();
            DataContext = this;
            ChangelogContent = File.ReadAllText("../../changelog.txt");
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => Close();
    }
}
