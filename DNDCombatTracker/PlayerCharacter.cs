using System;

namespace DNDCombatTracker
{
    // Implements Death Save functionality and eventually other player-only features
    [Serializable]
    public class PlayerCharacter : Character
    {
        public override bool IsDead => HitPoints <= -HitPointMax;

        public bool ShouldMakeDeathSave => HitPoints < 0 && HitPoints > -HitPointMax;
        public int DeathSaveFails { get; private set; } = 0;
        public void FailDeathSave()
        {
            DeathSaveFails++;
            MadeDeathSave(false);
            if (DeathSaveFails >= 3)
            {
                HitPoints = -HitPointMax; // You are dead. Not big surprise.
                DeathSaveFails = 0;
                DeathSaveSuccess = 0;
            }

            NotifyPropertyChanged("DeathSaveFails");
        }
        public int DeathSaveSuccess { get; private set; } = 0;
        public void SucceedDeathSave()
        {
            DeathSaveSuccess++;
            MadeDeathSave(true);
            if (DeathSaveSuccess >= 3)
            {
                HitPoints = 0; // Stabilize on 3 succesful saves
                DeathSaveFails = 0;
                DeathSaveSuccess = 0;
            }

            NotifyPropertyChanged("DeathSaveSuccess");
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
                SucceedDeathSave();
            else if (result == false)
                FailDeathSave();
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
