﻿using LingvoCards.App.Views;

namespace LingvoCards.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell() { HeightRequest = 1000, WidthRequest = 800 };
        }

    }
}
