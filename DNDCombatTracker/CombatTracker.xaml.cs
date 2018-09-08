using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DNDCombatTracker
{
    public partial class CombatTracker : Window, INotifyPropertyChanged
    {
        public Character SelectedCharacter { get; set; }

        public string ToggleCombatButtonString => isCombatEnabled ? "End Combat" : "Start Combat";
        private bool isCombatEnabled = false;
        public bool IsCombatEnabled
        {
            get => isCombatEnabled;
            set
            {
                isCombatEnabled = value;
                NotifyPropertyChanged("IsCombatEnabled");
                NotifyPropertyChanged("ToggleCombatButtonString");
                NotifyPropertyChanged("ActiveCharacter");
            }
        }

        private bool removeNPCsOnDeath = true;
        public bool RemoveNPCsOnDeath
        {
            get => removeNPCsOnDeath;
            set
            {
                removeNPCsOnDeath = value;
                NotifyPropertyChanged("RemoveNPCsOnDeath");
            }
        }

        public Character ActiveCharacter => IsCombatEnabled ? Characters[ActiveCharacterIndex] : null;
        private int activeCharacterIndex;
        public int ActiveCharacterIndex
        {
            get => activeCharacterIndex;
            private set
            {
                activeCharacterIndex = value;
                NotifyPropertyChanged("ActiveCharacterIndex");
                NotifyPropertyChanged("ActiveCharacter");
            }
        }

        public ObservableCollection<Character> Characters = new ObservableCollection<Character>();

        private string combatLogContent;
        public string CombatLogContent
        {
            get => combatLogContent;
            private set
            {
                combatLogContent = value;
                NotifyPropertyChanged("CombatLogContent");
            }
        }

        // Property change event handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CombatTracker()
        {
            InitializeComponent();
            DataContext = this;
            CharactersList.ItemsSource = Characters;
        }

        private void AddLogEntry(string text) => CombatLogContent += text + '\n';
        private void LogEntryAdded(object sender, LogEntryEventArgs e) => AddLogEntry(e.Text);

        private void NewLogEntry_Click(object sender, RoutedEventArgs e)
        {
            AddLogEntryDialog logEntryDialog = new AddLogEntryDialog();
            if (logEntryDialog.ShowDialog() == true)
                AddLogEntry(logEntryDialog.LogEntry);
        }

        private void PromptAttack(Character actor)
        {
            Character target = actor.ShowAttackDialog(new List<Character>(Characters));
        }

        private void DamageCharacter(Character actor) => actor.ShowDamageDialog();

        private void CharacterActionsAttack_Click(object sender, RoutedEventArgs e) => PromptAttack(ActiveCharacter);
        private void CharacterActionsDamage_Click(object sender, RoutedEventArgs e) => DamageCharacter(ActiveCharacter);

        private void MenuCharacterAttack_Click(object sender, RoutedEventArgs e) => PromptAttack(SelectedCharacter);
        private void MenuCharacterDamage_Click(object sender, RoutedEventArgs e) => DamageCharacter(SelectedCharacter);

        private void CharacterActionsRemove_Click(object sender, RoutedEventArgs e)
        {
            AddLogEntry(ActiveCharacter.NameAndIsPlayer + " was removed from combat.");
            RemoveCharacter(ActiveCharacter); // Remove character from global charlist
        }

        private void MenuCharacterRemove_Click(object sender, RoutedEventArgs e)
        {
            AddLogEntry(SelectedCharacter.NameAndIsPlayer + " was removed from combat.");
            RemoveCharacter(SelectedCharacter); // Remove character from global charlist
        }
        
        private void CharacterActionsEditNotes_Click(object sender, RoutedEventArgs e)
        {
            NoteEditorDialog noteEditor = new NoteEditorDialog(SelectedCharacter);
            noteEditor.ShowDialog();
        }

        private void CharacterActions_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = (sender as Button).ContextMenu;
            contextMenu.IsEnabled = true;
            contextMenu.PlacementTarget = (sender as Button);
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }

        private void AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            AddCharacterDialog addCharacterDialog = new AddCharacterDialog();
            if (addCharacterDialog.ShowDialog() == true)
            {
                Character newCharacter;
                if (addCharacterDialog.IsPlayerCharacter)
                {
                    newCharacter = new PlayerCharacter
                    {
                        Name = addCharacterDialog.CharacterName,
                        Initiative = addCharacterDialog.Initiative,
                        HitPointMax = addCharacterDialog.MaxHP,
                        HitPoints = addCharacterDialog.CurrentHP,
                        //IsPlayerCharacter = characterDialog.IsPlayerCharacter
                    };
                }
                else {
                    newCharacter = new Character
                    {
                        Name = addCharacterDialog.CharacterName,
                        Initiative = addCharacterDialog.Initiative,
                        HitPointMax = addCharacterDialog.MaxHP,
                        HitPoints = addCharacterDialog.CurrentHP,
                        //IsPlayerCharacter = characterDialog.IsPlayerCharacter
                    };
                }

                AddCharacter(newCharacter);
                AddLogEntry(newCharacter.NameAndIsPlayer + " has entered combat with initiative " + newCharacter.Initiative + ".");
            }
        }

        public void AddCharacter(Character character)
        {
            if (IsCombatEnabled)
                InsertCharacter(character); // If in combat insert character in correct initiative order
            else 
                Characters.Add(character); // Otherwise just append it to the list
            
            character.OnHit += OnCharacterHit;
            if (character is PlayerCharacter)
            {
                ((PlayerCharacter)character).OnStabilized += OnPlayerStabilized;
                ((PlayerCharacter)character).OnDeathSave += OnPlayerMadeDeathSave;
            }
        }

        // Inserts a character in the list in the right initiative order
        private void InsertCharacter(Character character)
        {
            int i = 0;
            while (i < Characters.Count && character.Initiative <= Characters[i].Initiative)
                i++;

            Characters.Insert(i, character);
            
            character.OnHit += OnCharacterHit;
            if (character is PlayerCharacter)
            {
                ((PlayerCharacter)character).OnStabilized += OnPlayerStabilized;
                ((PlayerCharacter)character).OnDeathSave += OnPlayerMadeDeathSave;
            }
        }

        // TODO: Find a better way to do this
        // Probably implement sorting directly into ObservableCollection
        private void SortCharacterList()
        {
            // Put the ObservableCollection in a List because the former can't be sorted
            List<Character> charactersAsList = new List<Character>(Characters);

            charactersAsList.Sort((p, q) => q.Initiative.CompareTo(p.Initiative)); // Sort the list in descending order

            // Empty the ObsCol and add elements individually, otherwise WPF won't pick it up
            Characters.Clear();
            foreach (Character character in charactersAsList)
                Characters.Add(character);
        }

        private void ToggleCombat_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCombatEnabled) // Start combat
            {
                if (Characters.Count == 0)
                {
                    MessageBox.Show("Cannot start combat with no characters!", "Oi.");
                    return;
                }

                SortCharacterList();
                ActiveCharacterIndex = 0; // Start from first initiative order
                CombatLogContent = ""; // Clear combat log

                IsCombatEnabled = true;
                AddLogEntry("A combat encounter has started! " + ActiveCharacter.Name + " is first with initiative " + ActiveCharacter.Initiative);
            }
            else // End combat
            {
                Characters.Clear();
                IsCombatEnabled = false;
                AddLogEntry("The combat encounter has ended.");
                NotifyPropertyChanged("ActiveCharacter");
            }
        }

        private void NextTurn()
        {
            if (!IsCombatEnabled)
                return;

            if (Characters.Count == 0)
            {
                IsCombatEnabled = false;
                AddLogEntry("The combat encounter has ended.");
                return;
            }

            if (ActiveCharacterIndex >= Characters.Count - 1)
                ActiveCharacterIndex = 0;
            else
                ActiveCharacterIndex++;

            PlayerCharacter playerCharacter = ActiveCharacter as PlayerCharacter;
            if (playerCharacter != null && playerCharacter.ShouldMakeDeathSave)
                playerCharacter.PromptDeathSave();
        }

        private void RemoveCharacter(Character character)
        {
            Characters.Remove(character);
            if (Characters.Count == 0)
            {
                IsCombatEnabled = false;
                AddLogEntry("The combat encounter has ended.");
                return;
            }

            if (ActiveCharacterIndex >= Characters.Count)
                ActiveCharacterIndex = 0; // Prevents OOI errors

            NotifyPropertyChanged("ActiveCharacter");
        }

        private void OnCharacterHit(object sender, CharacterHitEventArgs e)
        {
            Character hit = sender as Character;

            // Print damage log
            if (e.Attacker != null)
                AddLogEntry(e.Attacker + (e.DamageAmt < 0 ? " healed " : " hit ") + hit + " for " + e.DamageAmt + " hit points.");
            else
                AddLogEntry(hit + " took " + e.DamageAmt + " damage from environment.");

            if (e.TargetKilledOrDowned) // On kill print kill log and remove if enabled
            {
                AddLogEntry(hit.Name + " was " + 
                    ((hit.IsPlayerCharacter && hit.HitPoints > -(hit.HitPointMax)) ? "downed" : "killed") + 
                    (e.Attacker != null ? " by " + e.Attacker.Name : "") + ".");
                if (!hit.IsPlayerCharacter && RemoveNPCsOnDeath)
                    RemoveCharacter(hit); // Never remove dead players
            }
        }

        private void OnPlayerStabilized(object sender, EventArgs e)
        {
            PlayerCharacter player = sender as PlayerCharacter;
            AddLogEntry(player.Name + " is now stabilized.");
        }

        private void OnPlayerMadeDeathSave(object sender, DeathSaveEventArgs e)
        {
            PlayerCharacter player = sender as PlayerCharacter;
            AddLogEntry(player.Name + (e.Success ? " succeeded" : " failed") + " their death save. Successes: "
                + e.Successes + ", Failures: " + e.Failures);

            if (e.Successes >= 3)
                AddLogEntry(player.Name + " succeeded their death saves and is now stabilized.");
            if (e.Failures >= 3)
                AddLogEntry(player.Name + " failed their death saves and died.");
        }

        private void EndTurn_Click(object sender, RoutedEventArgs e) => NextTurn();

        private void SaveCombatLog_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt",
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllLines(saveFileDialog.FileName, combatLogContent.Split('\n'));
        }
    }
}
