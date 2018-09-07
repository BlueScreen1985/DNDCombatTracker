using System;

namespace DNDCombatTracker
{
    public class CharacterDeathEventArgs : EventArgs
    {
        public CharacterDeathEventArgs(Character killer) => Killer = killer;

        public Character Killer { get; }
    }
    
    public delegate void CharacterDeathEventHandler(object sender, CharacterDeathEventArgs e);
}
