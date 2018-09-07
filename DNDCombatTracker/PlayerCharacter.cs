using System;

namespace DNDCombatTracker
{
    // Implements Death Save functionality and eventually other player-only features
    [Serializable]
    public class PlayerCharacter : Character
    {
        private int deathSaveFails = 0;
        private int deathSaveSuccess = 0;

        public int DeathSaveFails
        {
            get => deathSaveFails;
            set
            {
                deathSaveFails = value;
                NotifyPropertyChanged("DeathSaveFails");
            }
        }
        public int DeathSaveSuccess
        {
            get => deathSaveSuccess;
            set
            {
                deathSaveSuccess = value;
                NotifyPropertyChanged("DeathSaveSuccess");
            }
        }

        public void PromptDeathSave()
        {
            
        }
    }
}
