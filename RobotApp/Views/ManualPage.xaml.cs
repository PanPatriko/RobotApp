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
    public partial class ManulaPage : ContentPage
    {
        public ManulaPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<Application, string>(this, "State", (sender, arg) =>
            {
                stateLabel.Text = "Status: " + arg;
            });
        }


        private void ManualSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected())
            {
                if (ManualSwitch.IsToggled)
                {
                    DependencyService.Get<IBluetooth>().Write("M");
                    DependencyService.Get<IBluetooth>().Write(PWMLabel.Text);
                    Task.Run(() =>
                    {
                        while (ManualSwitch.IsToggled)
                        {
                            //if(!DependencyService.Get<IBluetooth>().IsConnected())
                            //{
                            //     break;
                            // }
                            if (ForwardButton.IsPressed)
                            {
                                DependencyService.Get<IBluetooth>().Write("F");
                            }
                            if (BackButton.IsPressed)
                            {
                                DependencyService.Get<IBluetooth>().Write("B");
                            }
                            if (RightButton.IsPressed)
                            {
                                DependencyService.Get<IBluetooth>().Write("R");
                            }
                            if (LeftButton.IsPressed)
                            {
                                DependencyService.Get<IBluetooth>().Write("L");
                            }
                        }
                    });
                }
                else
                {
                    DependencyService.Get<IBluetooth>().Write("Stop");
                }
            }
        }

        private void PWMSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PWMLabel.Text = Math.Round(PWMSlider.Value).ToString();
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write(PWMLabel.Text);
            }
        }
    }
}