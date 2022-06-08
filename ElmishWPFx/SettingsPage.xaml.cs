using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Input;

namespace ElmishWPFx
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings() => InitializeComponent();

        private void SelectAllText(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.SelectAll();
                e.Handled = true;
                textBox.Focus();
            }
        }
    }
}
