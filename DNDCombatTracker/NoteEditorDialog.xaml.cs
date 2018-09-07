using System.Windows;

namespace DNDCombatTracker
{
    public partial class NoteEditorDialog : Window
    {
        public Character Character { get; }

        public NoteEditorDialog(Character character)
        {
            InitializeComponent();
            Character = character;
            DataContext = this;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
