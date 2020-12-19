using RobotApp.ViewModels;
using RobotApp.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace RobotApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
            Routing.RegisterRoute(nameof(AutoPage), typeof(AutoPage));

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Wyjdź","Czy na pewno chcesz wyjść", "Tak", "Nie");
            if(response)
                System.Environment.Exit(0);
           // System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
