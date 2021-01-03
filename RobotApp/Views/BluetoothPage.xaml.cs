using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using RobotApp.Models;

namespace RobotApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {

        public BluetoothPage()
        {
            InitializeComponent();
            groupedDevices.Add(bondedDevices);
            groupedDevices.Add(newDevices);

            DevicesListView.ItemsSource = groupedDevices;

            MessagingCenter.Subscribe<Application, string>(this, "Hi", (sender, arg) =>
             {
                 readE.Text += arg;
             });

            MessagingCenter.Subscribe<Application, string>(this, "State", (sender, arg) =>
            {
                stateLabel.Text = "Status: " + arg;
                if (DependencyService.Get<IBluetooth>().IsConnected())
                {
                    DependencyService.Get<IBluetooth>().Listen();
                }
                else 
                {
                    ConnectButton.IsEnabled = true;
                }
            });
            //MessagingCenter.Subscribe<Application, string>(this, "Alert", (sender, arg) => CreateAlert(arg));

        }

        private ObservableCollection<GroupedModel> groupedDevices = new ObservableCollection<GroupedModel>();

        public static GroupedModel newDevices = new GroupedModel() { DeviceType = "Nowe urządzenia" };

        public static GroupedModel bondedDevices = new GroupedModel() { DeviceType = "Sparowane urządzenia" };


        private async void CreateAlert(string arg)
        {
            await DisplayAlert("", arg, "OK");
        }

        private async void Scan_Clicked(object sender, EventArgs e)
        {
            if (!DependencyService.Get<IBluetooth>().IsGpsEnable())
            {
                bool result = await DisplayAlert("GPS", "Aby wyszukać nowe urządzenia należy włączyć lokalizacje", "Tak", "Anuluj");
                if (result)
                {
                    DependencyService.Get<IBluetooth>().GpsEnable();
                }
            }
            if (!DependencyService.Get<IBluetooth>().IsBluetoothEnabled())
            {
                bool result = await DisplayAlert("Bluetooth", "Włączyć bluetooth?", "Tak", "Anuluj");
                if(result)
                {
                    DependencyService.Get<IBluetooth>().BluetoothEnable();
                }
            }
            else
            {
                DependencyService.Get<IBluetooth>().GetBondedDevices();
                DependencyService.Get<IBluetooth>().Scan();
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<IBluetooth>().Write(SendE.Text);
                readE.Text += SendE.Text + "\n\r";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void CloseConnection_Clicked(object sender, EventArgs e)
        {
            try
            {
                DependencyService.Get<IBluetooth>().CloseConnection();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DevicesListView.SelectedItem != null)
                {
                    if (!DependencyService.Get<IBluetooth>().IsConnected())
                    {
                        DependencyService.Get<IBluetooth>().CancelScan();
                        DependencyService.Get<IBluetooth>().Connect(DevicesListView.SelectedItem as Models.DeviceInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}