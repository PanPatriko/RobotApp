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
        
        public MapPage(string s)
        {
            InitializeComponent();
            oxyPlotViewModel = new OxyPlotViewModel();
            BindingContext = oxyPlotViewModel;
            var isZebraDevice = DependencyService.Get<IDevice>().IsZebraDevice();
            MapName.Text = s + " " + isZebraDevice.ToString();

        }
    }
}