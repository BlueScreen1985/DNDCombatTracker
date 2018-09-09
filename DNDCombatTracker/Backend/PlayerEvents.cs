using System;

namespace DNDCombatTracker
{
    public class DeathSaveEventArgs : EventArgs
    {
        public DeathSaveEventArgs(bool success, int successes, int fails)
        {
            Success = success;
            Successes = successes;
            Failures = fails;
        }

        public bool Success { get; }
        public int Successes { get; }
        public int Failures { get; }
    }

    public delegate void DeathSaveEventHandler(object sender, DeathSaveEventArgs e);

    public delegate void PlayerStabilizedEventHandler(object sender, EventArgs e);
}
