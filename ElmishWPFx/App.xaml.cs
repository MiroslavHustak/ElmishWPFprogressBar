using System;
using System.Windows;

namespace ElmishWPFx
{
    public partial class App : Application
    {
        public App()
        {
            this.Activated += StartElmish;
        }

        private void StartElmish(object sender, EventArgs e)
        {
            this.Activated -= StartElmish;
            Elmish.Program.main(MainWindow);
        }
    }
}
