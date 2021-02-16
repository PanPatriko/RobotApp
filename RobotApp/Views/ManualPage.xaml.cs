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
                if(ManualSwitch.IsToggled && DependencyService.Get<IBluetooth>().IsConnected())
                {
                    DependencyService.Get<IBluetooth>().Write("M");
                    DependencyService.Get<IBluetooth>().Write(" " + PWMLabel.Text + " ");
                }
            });
            MessagingCenter.Subscribe<Application, string>(this, "AutoON", (sender, arg) =>
            {
                isAuto = true;
            });
            MessagingCenter.Subscribe<Application, string>(this, "AutoOFF", (sender, arg) =>
            {
                isAuto = false;
            });
            MessagingCenter.Subscribe<Application, string>(this, "ManualDisable", (sender, arg) =>
            {
                ManualSwitch.IsToggled = false;
            });
        }

        bool isAuto = false;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != WidthRequest || height != HeightRequest)
            {
                WidthRequest = width;
                HeightRequest = height;
                if (width > height)
                {
                    Stack1.Orientation = StackOrientation.Horizontal;
                    Stack1.VerticalOptions = LayoutOptions.Start;
                }
                else
                {
                    Stack1.Orientation = StackOrientation.Vertical;
                    Stack1.VerticalOptions = LayoutOptions.FillAndExpand;
                }
            }
        }

        private async void ManualSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected())
            {
                if(isAuto && ManualSwitch.IsToggled)
                {
                    bool result = await DisplayAlert("Uwaga", "Uruchomiony jest tryb automatyczny\n\rCzy chcesz go przerwać?", "Tak", "Nie");
                    if (result)
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ManualON", "");
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "AutoDisable", "");
                        isAuto = false;
                        DependencyService.Get<IBluetooth>().Write("M");
                        DependencyService.Get<IBluetooth>().Write(PWMLabel.Text);
                       // _ = Task.Run(ManualControl);
                    }
                    else
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ManualOFF", "");
                        ManualSwitch.IsToggled = false;
                    }
                }
                else if (ManualSwitch.IsToggled && !isAuto)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ManualON", "");
                    DependencyService.Get<IBluetooth>().Write("M");
                    DependencyService.Get<IBluetooth>().Write(PWMLabel.Text);
                   // _ = Task.Run(ManualControl);
                }
                else if(!isAuto)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ManualOFF", "");
                    DependencyService.Get<IBluetooth>().Write("S");
                }
            }
            else
            {
                ManualSwitch.IsToggled = false;
            }
        }

        private void ManualControl()
        {
            while (ManualSwitch.IsToggled)
            {
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
        }

        private void PWMSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PWMLabel.Text = Math.Round(PWMSlider.Value).ToString();
        //    if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
           // {
           //     DependencyService.Get<IBluetooth>().Write(PWMLabel.Text);
            //}
        }

        private void ForwardButton_Pressed(object sender, EventArgs e)
        {
            if(DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write("F");
            }
        }
        private void BackButton_Pressed(object sender, EventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write("B");
            }
        }

        private void LeftdButton_Pressed(object sender, EventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write("L");
            }
        }

        private void RightButton_Pressed(object sender, EventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write("R");
            }
        }

        private void Button_Released(object sender, EventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write("P");
            }
        }


        private void PWMSlider_DragCompleted(object sender, EventArgs e)
        {
            if (DependencyService.Get<IBluetooth>().IsConnected() && ManualSwitch.IsToggled)
            {
                DependencyService.Get<IBluetooth>().Write(" " + PWMLabel.Text + " ");
            }
        }
    }
}