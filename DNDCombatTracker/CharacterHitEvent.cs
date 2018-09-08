using System;

namespace DNDCombatTracker
{
    public class CharacterHitEventArgs : EventArgs
    {
        public CharacterHitEventArgs(Character attacker, int dmgAmt, bool targetKilledOrDowned)
        {
            Attacker = attacker;
            DamageAmt = dmgAmt;
            TargetKilledOrDowned = targetKilledOrDowned;
        }

        public Character Attacker { get; }
        public int DamageAmt { get; }
        public bool TargetKilledOrDowned { get; }
    }
    
    public delegate void CharacterHitEventHandler(object sender, CharacterHitEventArgs e);
}
