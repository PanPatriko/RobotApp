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
            
            MessagingCenter.Subscribe<Application, string>(this, "Alert", (sender, arg) => CreateAlert(arg));

            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
            Routing.RegisterRoute(nameof(AutoPage), typeof(AutoPage));

        }

        private async void CreateAlert(string arg)
        {
            await DisplayAlert("", arg, "OK");
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
