using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotApp.Droid
{
    public static class BluetoothDevicesList
    {
        public static List<BluetoothDevice> devices = new List<BluetoothDevice>();
    }
}