using System;

namespace DNDCombatTracker
{
    public class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(string text) => Text = text;

        public virtual string Text { get; }
    }

    public delegate void LogEntryEventHandler(object sender, LogEntryEventArgs e);
}
