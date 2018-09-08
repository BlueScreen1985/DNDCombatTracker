using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DNDCombatTracker
{
    [Serializable]
    public class Character : INotifyPropertyChanged
    {
        public string HitPointCounter => !IsDead ? (HitPoints + "/" + HitPointMax) : "DEAD";
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
        public virtual bool IsDead => HitPoints <= 0;

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

        public virtual void TakeDamage(int amt, Character attacker = null)
        {
            HitPoints = Math.Max(Math.Min(HitPoints - amt, HitPointMax), -HitPointMax);
            Hit(amt, attacker);
        }

        public void DealDamage(Character target, int amt)
        {
            if (target.HitPoints < 0 && amt < 0) // Heal from 0 hit points (negative HP technically doesn't exist in 5e)
                target.HitPoints = 0;
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

        public void ShowDamageDialog()
        {
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

        public event CharacterHitEventHandler OnHit;
        public void Hit(int dmgAmt, Character attacker = null)
        {
            OnHit?.Invoke(this, new CharacterHitEventArgs(attacker, dmgAmt, HitPoints <= 0));
        }
    }
}
