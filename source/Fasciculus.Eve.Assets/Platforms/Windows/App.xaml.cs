﻿namespace Fasciculus.Eve.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
