using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotApp.ViewModels
{
    class OxyPlotViewModel
    {
        public PlotModel Model { get; set; }

        public OxyPlotViewModel()
        {
            Model = GetModel();
        }

        private PlotModel GetModel()
        {
            var plotModel1 = new PlotModel();
            plotModel1.Title = "Skan pomieszczenia";
            plotModel1.Background = OxyColors.LightGray;
            plotModel1.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Title = "Y",
                Position = AxisPosition.Left,
                // Magic Happens here we add the extra grid line on our Y Axis at zero
                ExtraGridlines = new Double[] { 0 }
            });
            plotModel1.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Title = "X",
                Position = AxisPosition.Bottom,

                // Magic Happens here we add the extra grid line on our Y Axis at zero
                ExtraGridlines = new Double[] { 0 }
            });

            return plotModel1;
        }
    }
}
