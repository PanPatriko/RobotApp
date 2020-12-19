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
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            oxyPlotViewModel.Model.Series.Clear();
            //  ScatterSeries pointsSeries = new ScatterSeries();
            pointsSeries.Points.Add(new ScatterPoint(random.Next(-10, 10), random.Next(-10, 10)));
            oxyPlotViewModel.Model.Series.Add(pointsSeries);
            oxyPlotViewModel.Model.InvalidatePlot(true);
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
    }
}