using System;

namespace DNDCombatTracker
{
    // Implements Death Save functionality and eventually other player-only features
    [Serializable]
    public class PlayerCharacter : Character
    {
        private int deathSaveFails = 0;
        private int deathSaveSuccess = 0;

        public override bool IsDead => HitPoints <= -HitPointMax;

        public bool ShouldMakeDeathSave => HitPoints < 0 && HitPoints > -HitPointMax;
        public int DeathSaveFails
        {
            get => deathSaveFails;
            set
            {
                deathSaveFails = value;
                MadeDeathSave(false);
                if (deathSaveFails >= 3)
                    HitPoints = -HitPointMax; // You are dead. Not big surprise.

                NotifyPropertyChanged("DeathSaveFails");
            }
        }
        public int DeathSaveSuccess
        {
            get => deathSaveSuccess;
            set
            {
                deathSaveSuccess = value;
                MadeDeathSave(true);
                if (deathSaveSuccess >= 3)
                    HitPoints = 0; // Stabilize on 3 succesful saves

                NotifyPropertyChanged("DeathSaveSuccess");
            }
        }

        public override void TakeDamage(int amt, Character attacker = null)
        {
            bool wasDying = ShouldMakeDeathSave;
            base.TakeDamage(amt, attacker);
            if (wasDying && HitPoints >= 0)
                Stabilized();
        }

        public void PromptDeathSave()
        {
            DeathSaveDialog deathSaveDialog = new DeathSaveDialog(Name);
            bool? result = deathSaveDialog.ShowDialog();
            if (result == true)
                DeathSaveSuccess++;
            else if (result == false)
                DeathSaveFails++;
        }

        public event PlayerStabilizedEventHandler OnStabilized;
        private void Stabilized() => OnStabilized?.Invoke(this, EventArgs.Empty);

        public event DeathSaveEventHandler OnDeathSave;
        private void MadeDeathSave(bool success)
        {
            OnDeathSave?.Invoke(this, new DeathSaveEventArgs(success, DeathSaveSuccess, DeathSaveFails));
        }
    }
}
