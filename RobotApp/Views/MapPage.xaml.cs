using OxyPlot.Series;
using RobotApp.Models;
using RobotApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RobotApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        OxyPlotViewModel oxyPlotViewModel;
        MapItem map;
        public MapPage(MapItem map)
        {
            InitializeComponent();
            this.map = map;
            oxyPlotViewModel = new OxyPlotViewModel();
            ScatterSeries pointsSeries = new ScatterSeries();
            BindingContext = oxyPlotViewModel;
            Title = map.Name;
            if(map.Points != null)
            {
                pointsSeries.Points.AddRange(map.Points);
                oxyPlotViewModel.Model.Series.Add(pointsSeries);
                oxyPlotViewModel.Model.InvalidatePlot(true);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Usuń", "Czy na pewno chcesz usunąć tą mapę?", "Tak", "Nie");
            if(result)
            {
                await App.Database.DeleteMapAsync(map);
                await Navigation.PopAsync();
            }
        }
    }
}