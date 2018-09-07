using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DNDCombatTracker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Character selectedCharacter;
        public Character SelectedCharacter
        {
            get => selectedCharacter;
            set
            {
                selectedCharacter = value;
                NotifyPropertyChanged("SelectedCharacter");
                NotifyPropertyChanged("SelectedCharacterExists");
            }
        }
        public bool SelectedCharacterExists => SelectedCharacter != null;

        public ObservableCollection<Character> PartyMembers = new ObservableCollection<Character>();

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CharacterList.ItemsSource = PartyMembers;
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

        private void AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            AddCharacterDialog addCharacterDialog = new AddCharacterDialog(Visibility.Hidden);
            if (addCharacterDialog.ShowDialog() == true)
            {
                Character newCharacter = new Character
                {
                    Name = addCharacterDialog.CharacterName,
                    ArmorClass = addCharacterDialog.ArmorClass,
                    HitPoints = addCharacterDialog.CurrentHP,
                    HitPointMax = addCharacterDialog.MaxHP,
                    IsPlayerCharacter = addCharacterDialog.IsPlayerCharacter
                };

                PartyMembers.Add(newCharacter);
            }
        }

        private void RemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCharacterExists)
                PartyMembers.Remove(SelectedCharacter);
        }

        private void NewEncounter_Click(object sender, RoutedEventArgs e)
        {
            CombatTracker combatWindow = new CombatTracker();

            foreach (Character character in PartyMembers)
            {
                InitiativeDialog initiativeDialog = new InitiativeDialog(character.Name);
                if (initiativeDialog.ShowDialog() == true)
                    character.Initiative = initiativeDialog.Initiative;
                else
                    character.Initiative = 0;

                combatWindow.AddCharacter(character);
            }

            combatWindow.Show();
        }

        private void LoadCharacterData(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".pty",
                Filter = "Party data file (.pty)|*.pty",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                ObservableCollection<Character> newCharacters = formatter.Deserialize(stream) as ObservableCollection<Character>;
                stream.Close();

                PartyMembers.Clear();
                foreach (Character character in newCharacters)
                    PartyMembers.Add(character);
            }
        }

        private void SaveCharacterData(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".pty",
                Filter = "Party data file (.pty)|*.pty",
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, PartyMembers);
                stream.Close();
            }
        }

        private void Quit(object sender, RoutedEventArgs e) => this.Close();
    }
}
