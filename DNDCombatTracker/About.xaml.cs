using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DNDCombatTracker
{
    public partial class About : Window, INotifyPropertyChanged
    {
        private string versionString;
        public string VersionString
        {
            get => versionString;
            private set
            {
                versionString = value;
                NotifyPropertyChanged("VersionString");
            }
        }

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public About()
        {
            InitializeComponent();
            DataContext = this;
            VersionString = File.ReadAllText("../../version.txt");
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => Close();
    }
}
