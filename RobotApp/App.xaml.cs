using RobotApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RobotApp.Data;
using System.IO;

namespace RobotApp
{
    public partial class App : Application
    {
        static MapDatabase database;

        public static MapDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new MapDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Maps.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
