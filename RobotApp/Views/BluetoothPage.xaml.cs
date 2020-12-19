using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Bluetooth;
using Base2Base.Abstractions.Connectivity.Bluetooth;
using Xamarin.Essentials;
namespace RobotApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {
        public BluetoothPage()
        {
            InitializeComponent();
        }

        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // if (!CrossBluetooth.Current.IsEnabled)
            //  {
            //     bool result = await DisplayAlert("Enable Bluetooth", "Enable Bluetooth?", "OK", "Cancel");
            //     if (result)
            //     {
            //          CrossBluetooth.Current.Enable();
            //    }
            //    }
            stateLabel.Text = await GetBluetoothDeviceAddress();
            
        }

    private async Task<bool> CheckBluetoothEnabled()
        {
            if (!CrossBluetooth.Current.IsEnabled)
            {
                bool result = await DisplayAlert("Enable Bluetooth", "Enable Bluetooth?", "OK", "Cancel");
                if (result)
                {
                    CrossBluetooth.Current.Enable();
                }
            }
            return CrossBluetooth.Current.IsEnabled;
        }
        public async Task<string> GetBluetoothDeviceAddress()
        {
            string deviceAddress = string.Empty;
            if (await this.CheckBluetoothEnabled())
            {
                string[] devices = (from qr in Plugin.Bluetooth.CrossBluetooth.Current.BondedDevices select qr.Text).ToArray();
                string deviceText = await DisplayActionSheet("Select device", "Cancel", null, devices);
                if (!string.IsNullOrEmpty(deviceText) && deviceText != "Cancel")
                {
                    BluetoothDevice bluetoothDevice = (from qr in Plugin.Bluetooth.CrossBluetooth.Current.BondedDevices
                                                       where qr.Text == deviceText
                                                       select qr).FirstOrDefault();
                    deviceAddress = bluetoothDevice.Address;
                }
            }
            return deviceAddress;
        }
    }
}