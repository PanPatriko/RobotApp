using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RobotApp.Droid;
using RobotApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Device_Android))]

namespace RobotApp.Droid
{
    class Device_Android : IDevice
    {
        public bool IsZebraDevice()
        {
            return Android.OS.Build.Manufacturer == "Zebra Technologies";
        }
    }
}