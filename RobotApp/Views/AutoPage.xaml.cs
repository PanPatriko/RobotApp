using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using RobotApp.Models;
using RobotApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RobotApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoPage : ContentPage
    {
        OxyPlotViewModel oxyPlotViewModel;
        Random random = new Random();
        ScatterSeries pointsSeries = new ScatterSeries();
        LineSeries lineSeries = new LineSeries();
        bool isManual = false;
        public AutoPage()
        {
            InitializeComponent();
            oxyPlotViewModel = new OxyPlotViewModel();
            BindingContext = oxyPlotViewModel;

            MessagingCenter.Subscribe<Application, string>(this, "State", (sender, arg) =>
            {
                stateLabel.Text = "Status: " + arg;
                if (AutoSwitch.IsToggled && DependencyService.Get<IBluetooth>().IsConnected())
                {
                    DependencyService.Get<IBluetooth>().Write("A");
                }
            });
            MessagingCenter.Subscribe<Application, string>(this, "ManualON", (sender, arg) =>
            {
                isManual = true;
            });
            MessagingCenter.Subscribe<Application, string>(this, "ManualOFF", (sender, arg) =>
            {
                isManual = false;
            });
            MessagingCenter.Subscribe<Application, string>(this, "AutoDisable", (sender, arg) =>
            {
                AutoSwitch.IsToggled = false;
                ClearOxyPlot();
            });
            MessagingCenter.Subscribe<Application, string>(this, "Read", (sender, arg) =>
            {
                if(AutoSwitch.IsToggled)
                {
                    try
                    {
                        arg = arg.Replace('\r', '\0');
                        arg = arg.Replace('\n', '\0');
                        arg = arg.Replace("\0", string.Empty);
                        string[] coordinates = arg.Split(':');
                        oxyPlotViewModel.Model.Series.Clear();
                        //  ScatterSeries pointsSeries = new ScatterSeries();
                        pointsSeries.Points.Add(new ScatterPoint(int.Parse(coordinates[0]), int.Parse(coordinates[1]),2.5));
                        oxyPlotViewModel.Model.Series.Add(pointsSeries);
                        oxyPlotViewModel.Model.InvalidatePlot(true);
                    }
                    catch (Exception e)
                    {
                        //Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", e.Message);
                    }
                }
            });
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            oxyPlotViewModel.Model.Series.Clear();
            //  ScatterSeries pointsSeries = new ScatterSeries();
            pointsSeries.Points.Add(new ScatterPoint(random.Next(-50, 51), random.Next(-50, 51),2.5));
            oxyPlotViewModel.Model.Series.Add(pointsSeries);
            oxyPlotViewModel.Model.InvalidatePlot(true);
        }

        private void ClearOxyPlot()
        {
            oxyPlotViewModel.Model.Series.Clear();
            pointsSeries = new ScatterSeries();
            lineSeries = new LineSeries();
            oxyPlotViewModel.Model.InvalidatePlot(true);
        }

        private void PointsToLine()
        {
            oxyPlotViewModel.Model.Series.Clear();
            foreach (ScatterPoint scatter in pointsSeries.Points)
            {
                lineSeries.Points.Add(new DataPoint(scatter.X, scatter.Y));
            }
            oxyPlotViewModel.Model.Series.Add(lineSeries);
            oxyPlotViewModel.Model.InvalidatePlot(true);
        }

        private async void AutoSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected())
            {
                if (isManual && AutoSwitch.IsToggled)
                {
                    bool result = await DisplayAlert("Uwaga", "Uruchomiony jest tryb ręczny\n\rCzy chcesz go przerwać?", "Tak", "Nie");
                    if (result)
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "AutoON", "");
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ManualDisable", "");
                        isManual = false;
                        DependencyService.Get<IBluetooth>().Write("A");
                    }
                    else
                    {
                        AutoSwitch.IsToggled = false;
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "AutoOFF", "");
                    }
                }
                else if(AutoSwitch.IsToggled && !isManual)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "AutoON", "");
                    DependencyService.Get<IBluetooth>().Write("A");
                }
                else if(!isManual)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "AutoOFF", "");
                    DependencyService.Get<IBluetooth>().Write("S");
                    ClearOxyPlot();
                }
            }
            else
            {
                AutoSwitch.IsToggled = false;
                ClearOxyPlot();
            }
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Zapisz mape", "Jak chcesz nazwać mape?","OK","Anuluj");
            if(result != null)
            {
                MapItem map = new MapItem();
                map.Name = result;
                map.Points = pointsSeries.Points;
                await App.Database.SaveMapAsync(map);
                await DisplayAlert("", "Zapisano mape", "ok");
                ClearOxyPlot();
            }
        }
    }
}