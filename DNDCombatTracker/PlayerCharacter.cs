using System;

namespace DNDCombatTracker
{
    // Implements Death Save functionality and eventually other player-only features
    [Serializable]
    public class PlayerCharacter : Character
    {
        private int deathSaveFails = 0;
        private int deathSaveSuccess = 0;

        public bool ShouldMakeDeathSave => HitPoints < 0;
        public int DeathSaveFails
        {
            get => deathSaveFails;
            set
            {
                deathSaveFails = value;
                if (deathSaveFails >= 3)
                    Hit(); // Self explanatory

                NotifyPropertyChanged("DeathSaveFails");
            }
        }
        public int DeathSaveSuccess
        {
            get => deathSaveSuccess;
            set
            {
                deathSaveSuccess = value;
                if (deathSaveSuccess >= 3)
                    HitPoints = 0; // Stabilize on 3 succesful saves

                NotifyPropertyChanged("DeathSaveSuccess");
            }
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

        public override void TakeDamage(int amt, Character attacker = null)
        {
            HitPoints = Math.Min(HitPoints - amt, HitPointMax);
            if (attacker == null)
                AddLogEntry(Name + " took " + amt + " damage from environment.");
            if (HitPoints <= 0)
                AddLogEntry(Name + " is unconscious.");
        }

        event PlayerStabilizedEventHandler OnStabilized;
        private void Stabilized() => OnStabilized?.Invoke(this, EventArgs.Empty);
    }
}
