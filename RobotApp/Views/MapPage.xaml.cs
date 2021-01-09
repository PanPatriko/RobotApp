using OxyPlot;
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
        ScatterSeries pointsSeries = new ScatterSeries();
        LineSeries lineSeries = new LineSeries();

        public MapPage(MapItem map)
        {
            InitializeComponent();
            this.map = map;
            oxyPlotViewModel = new OxyPlotViewModel();
            
            BindingContext = oxyPlotViewModel;
            Title = map.Name;
            if(map.Points != null)
            {
                pointsSeries.Points.AddRange(map.Points);
                foreach (ScatterPoint scatter in pointsSeries.Points)
                {
                    scatter.Size = 2.5;
                    lineSeries.Points.Add(new DataPoint(scatter.X, scatter.Y));
                }
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

        private async void Button_Clicked2(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Zmień nazwe", "Wpisz nową nazwę", "OK", "Anuluj");
            if (result != null)
            {
                map.Name = result;
                await App.Database.SaveMapAsync(map);
                Title = result;
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if(MapSwitch.IsToggled)
            {
                oxyPlotViewModel.Model.Series.Clear();
                oxyPlotViewModel.Model.Series.Add(lineSeries);
                oxyPlotViewModel.Model.InvalidatePlot(true);
            }
            else
            {
                oxyPlotViewModel.Model.Series.Clear();
                oxyPlotViewModel.Model.Series.Add(pointsSeries);
                oxyPlotViewModel.Model.InvalidatePlot(true);
            }
        }
    }
}