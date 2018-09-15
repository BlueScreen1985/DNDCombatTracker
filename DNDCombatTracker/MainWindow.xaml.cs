using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
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
                Character newCharacter;
                if (addCharacterDialog.IsPlayerCharacter)
                {
                    newCharacter = new PlayerCharacter
                    {
                        Name = addCharacterDialog.CharacterName,
                        ArmorClass = addCharacterDialog.ArmorClass,
                        HitPoints = addCharacterDialog.CurrentHP,
                        HitPointMax = addCharacterDialog.MaxHP,
                        //IsPlayerCharacter = addCharacterDialog.IsPlayerCharacter
                    };
                }
                else
                {
                    newCharacter = new Character
                    {
                        Name = addCharacterDialog.CharacterName,
                        ArmorClass = addCharacterDialog.ArmorClass,
                        HitPoints = addCharacterDialog.CurrentHP,
                        HitPointMax = addCharacterDialog.MaxHP,
                        //IsPlayerCharacter = addCharacterDialog.IsPlayerCharacter
                    };
                }

                PartyMembers.Add(newCharacter);
            }
        }

        private void RemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCharacterExists)
                foreach (Character selected in SelectedCharacters)
                    PartyMembers.Remove(selected);
        }

        private void NewEncounter_Click(object sender, RoutedEventArgs e)
        {
            CombatTracker combatWindow = new CombatTracker();

            foreach (Character character in PartyMembers)
            {
                InitiativeDialog initiativeDialog = new InitiativeDialog(character.Name);
                if (initiativeDialog.ShowDialog() == true)
                    character.Initiative = initiativeDialog.Initiative.Value;
                else
                    character.Initiative = 0;

                combatWindow.AddCharacter(character);
            }

            combatWindow.Show();
        }

        private List<Character> SelectedCharacters
        {
            get
            {
                IList selectedItems = CharacterList.SelectedItems;
                List<Character> selectedCharacters = new List<Character>();

                // Convert to character list
                foreach (Character character in selectedItems)
                    selectedCharacters.Add(character as Character);

                return selectedCharacters;
            }
        }

        private void LoadCharacterData(object sender, RoutedEventArgs e) => LoadCharacterDataImpl(true);
        private void LoadPartyData(object sender, RoutedEventArgs e) => LoadCharacterDataImpl();

        private void LoadCharacterDataImpl(bool append = false)
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
                List<Character> newCharacters = formatter.Deserialize(stream) as List<Character>;
                stream.Close();

                if (!append)
                    PartyMembers.Clear();
                foreach (Character character in newCharacters)
                    PartyMembers.Add(character);
            }
        }

        private void SaveCharacterData(object sender, RoutedEventArgs e) => SaveCharacterDataImpl(SelectedCharacters);
        private void SavePartyData(object sender, RoutedEventArgs e) => SaveCharacterDataImpl(PartyMembers);

        private void SaveCharacterDataImpl(IEnumerable<Character> characters)
        {
            // Convert to List so saving is consistent
            List<Character> charactersList = new List<Character>();
            foreach (Character character in characters)
                charactersList.Add(character);

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
                formatter.Serialize(stream, charactersList);
                stream.Close();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e) => new About().Show();
        private void Changelog_Click(object sender, RoutedEventArgs e) => new Changelog().Show();

        private void Quit(object sender, RoutedEventArgs e) => Close();
    }
}
