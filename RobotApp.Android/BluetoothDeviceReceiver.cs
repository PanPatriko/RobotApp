using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using System.Text;

namespace RobotApp.Droid
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action != Android.Bluetooth.BluetoothDevice.ActionFound)
            {
                return;
            }

            // Get the device
            var device = (Android.Bluetooth.BluetoothDevice)intent.GetParcelableExtra(Android.Bluetooth.BluetoothDevice.ExtraDevice);

            if (device.BondState != Bond.Bonded)
            {
                // Console.WriteLine($"Found device with name: {device.Name} and MAC address: {device.Address}");
                // Device_Android.deviceList.Add(device.Name + " " + device.Address);
                Models.DeviceInfo deviceInfo = new Models.DeviceInfo(device.Name, device.Address);
                if (!Views.BluetoothPage.newDevices.Contains(deviceInfo))
                {
                    Views.BluetoothPage.newDevices.Add(deviceInfo);
                    BluetoothDevicesList.devices.Add(device);
                }
            }
        }
    }
}
