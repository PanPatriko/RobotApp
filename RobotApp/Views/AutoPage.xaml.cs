using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
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
        LineSeries line = new LineSeries();

        public AutoPage()
        {
            InitializeComponent();
            oxyPlotViewModel = new OxyPlotViewModel();
            BindingContext = oxyPlotViewModel;

            MessagingCenter.Subscribe<Application, string>(this, "State", (sender, arg) =>
            {
                stateLabel.Text = "Status: " + arg;
            });

            MessagingCenter.Subscribe<Application, string>(this, "Hi", (sender, arg) =>
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
                        pointsSeries.Points.Add(new ScatterPoint(int.Parse(coordinates[0]), int.Parse(coordinates[1])));
                        oxyPlotViewModel.Model.Series.Add(pointsSeries);
                        oxyPlotViewModel.Model.InvalidatePlot(true);
                    }
                    catch (Exception e)
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", e.Message);
                    }
                }
            });
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            oxyPlotViewModel.Model.Series.Clear();
            foreach (ScatterPoint scatter in pointsSeries.Points)
            {
                line.Points.Add(new DataPoint(scatter.X, scatter.Y));
            }
            oxyPlotViewModel.Model.Series.Add(line);
            oxyPlotViewModel.Model.InvalidatePlot(true);
        }

        private void AutoSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected())
            {
                if (AutoSwitch.IsToggled)
                {
                    DependencyService.Get<IBluetooth>().Write("A");

                }
                else
                {
                    DependencyService.Get<IBluetooth>().Write("Stop");
                }
            }
        }
    }
}