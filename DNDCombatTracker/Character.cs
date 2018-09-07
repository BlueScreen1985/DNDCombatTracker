using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DNDCombatTracker
{
    [Serializable]
    public class Character : INotifyPropertyChanged
    {
        public string HitPointCounter => HitPoints + "/" + HitPointMax;
        public string IsPlayerCharacterStr => IsPlayerCharacter ? "X" : "";

        // Bad verbose code because WPF kinda sucks and properties must send an updated event
        private int hitPoints = 100;
        private int hitPointMax = 100;
        public int HitPoints
        {
            get => hitPoints;
            set
            {
                hitPoints = value;
                NotifyPropertyChanged("HitPoints");
                NotifyPropertyChanged("HitPointCounter");
            }
        }
        public int HitPointMax
        {
            get => hitPointMax;
            set
            {
                hitPointMax = value;
                NotifyPropertyChanged("HitPointMax");
                NotifyPropertyChanged("HitPointCounter");
            }
        }

        private int armorClass = 10;
        public int ArmorClass
        {
            get => armorClass;
            set
            {
                armorClass = value;
                NotifyPropertyChanged("ArmorClass");
            }
        }

        private int initiative = 0;
        public int Initiative
        {
            get => initiative;
            set
            {
                initiative = value;
                NotifyPropertyChanged("Initiative");
            }
        }

        public string NameAndIsPlayer => Name + (IsPlayerCharacter ? " [PC]" : "");
        private string name = "";
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("NameAndIsPlayer");
            }
        }

        /*
        private bool isPlayerCharacter = false;
        public bool IsPlayerCharacter
        {
            get => isPlayerCharacter;
            set
            {
                isPlayerCharacter = value;
                NotifyPropertyChanged("IsPlayerCharacter");
                NotifyPropertyChanged("IsPlayerCharacterStr");
            }
        }
        */
        public bool IsPlayerCharacter => this is PlayerCharacter; // Amazing, this looks like pseudocode

        private string notes = "";
        public string Notes
        {
            get => notes;
            set
            {
                notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        public void TakeDamage(int amt, Character attacker = null)
        {
            HitPoints = Math.Min(HitPoints - amt, HitPointMax);
            if (attacker == null)
                AddLogEntry(Name + " took " + amt + " damage from environment.");
            if (HitPoints <= 0)
                Die(attacker);
        }

        public void DealDamage(Character target, int amt)
        {
            AddLogEntry(Name + (amt >= 0 ? " hit " : " healed ") + target.Name + " for " + Math.Abs(amt) + " hit points.");
            target.TakeDamage(amt, this);
        }

        public Character ShowAttackDialog(List<Character> targets)
        {
            AttackDialog attackDialog = new AttackDialog(targets);
            if (attackDialog.ShowDialog() == true)
            {
                Character target = attackDialog.TargetSelection.SelectedItem as Character;
                DealDamage(target, attackDialog.DamageAmt);
                return target;
            }
            return null;
        }

        public void ShowDamageDialog() {
            DamageCharDialog damageCharDialog = new DamageCharDialog();
            if (damageCharDialog.ShowDialog() == true)
                TakeDamage(damageCharDialog.DamageAmt);
        }

        // Property change event handling
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event LogEntryEventHandler LogEntryAdded;
        public void AddLogEntry(string logEntry) => LogEntryAdded?.Invoke(this, new LogEntryEventArgs(logEntry));

        public event CharacterDeathEventHandler OnDeath;
        public void Die(Character killer) => OnDeath?.Invoke(this, new CharacterDeathEventArgs(killer));
    }
}
