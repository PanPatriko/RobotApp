﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RobotApp.Droid;
using System;
using System.Linq;
using System.Text;
using Android.Bluetooth;
using RobotApp.Models;
using Java.Util;
using System.Threading.Tasks;
using System.Threading;
using Android.Locations;

[assembly: Xamarin.Forms.Dependency(typeof(Bluetooth_Android))]

namespace RobotApp.Droid
{

    class Bluetooth_Android : IBluetooth
    {
        BluetoothSocket socket;
        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        BluetoothDevice lastDevice;
        bool userDisconnect = false;
        public void BluetoothEnable()
        {
            if (bluetoothAdapter != null)
            {
                bluetoothAdapter.Enable();
            }
        }

        public void CancelScan()
        {
            if (bluetoothAdapter != null)
            {
                bluetoothAdapter.CancelDiscovery();
            }
        }
        public bool IsGpsEnable()
        {
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
        public void GpsEnable()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);

            try
            {
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: Gps Activity", Android.Widget.ToastLength.Short).Show();
            }

        }

        public bool IsBluetoothEnabled()
        {
            if (bluetoothAdapter != null)
            {
                return bluetoothAdapter.IsEnabled;
            }
            return false;
        }

        public bool IsConnected()
        {
            if (socket != null)
            {
                return socket.IsConnected;
            }
            return false;
        }
        public void CloseConnection()
        {
            if (socket != null)
            {
                try
                {
                    //Write("Disconnected with Android");
                    userDisconnect = true;
                    socket.Close();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                }
                catch (Java.IO.IOException)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Błąd podczas zamykania gniazda");
                }
            }
        }
        public void GetBondedDevices()
        {
            foreach (BluetoothDevice device in bluetoothAdapter.BondedDevices)
            {
                Models.DeviceInfo deviceInfo = new Models.DeviceInfo(device.Name, device.Address);
                if (!Views.BluetoothPage.bondedDevices.Contains(deviceInfo))
                {
                    Views.BluetoothPage.bondedDevices.Add(deviceInfo);
                    BluetoothDevicesList.devices.Add(device);
                }
            }
        }

        public void Scan()
        {
            if (bluetoothAdapter != null)
            {
                if (bluetoothAdapter.IsDiscovering)
                {
                    bluetoothAdapter.CancelDiscovery();
                }
                bluetoothAdapter.StartDiscovery();
            }
        }
        public async void ConnectAgain()
        {
            try
            {
                socket = lastDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                await socket.ConnectAsync();
                Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "połączono z " + socket.RemoteDevice.Name + " " + socket.RemoteDevice.Address);
                Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Połączono z " + socket.RemoteDevice.Name);
                //Write("Connected with Android");
                userDisconnect = false;
            }
            catch (Java.IO.IOException)
            {
                socket.Close();
                Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ConnectionLost", "Nie można połączyć się z urządzeniem");
            }
        }
        public async void Connect(DeviceInfo deviceInfo)
        {
            BluetoothDevice device = (from bd in BluetoothDevicesList.devices
                                      where bd.Name == deviceInfo.Name
                                      where bd.Address == deviceInfo.Addres
                                      select bd).FirstOrDefault();
            if (device != null)
            {
                try
                {
                    socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                }
                catch (Java.IO.IOException)
                {
                    socket.Close();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Nie można utworzyć gniazda");
                }
                try
                {
                    await socket.ConnectAsync();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "połączono z " + device.Name + " " + device.Address);
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Połączono z " + device.Name);
                    //Write("Connected with Android");
                    userDisconnect = false;
                    lastDevice = device;
                }
                catch (Java.IO.IOException)
                {
                    socket.Close();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Nie można połączyć się z urządzeniem");
                }
            }
        }

        public async void Listen()
        {
            byte[] buffer;
            string s = "";
            while (true)
            {
                try
                {
                    if(!socket.IsConnected)
                    {
                        break;
                    }
                    buffer = new byte[100];
                    int n = await socket.InputStream.ReadAsync(buffer, 0, buffer.Length);
                    s += Encoding.UTF8.GetString(buffer);
                    if (s.Contains("\r"))
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Read", s);
                        s = "";
                    }
                }
                catch (Java.IO.IOException)
                {
                    if (socket.IsConnected)
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                        socket.Close();
                        if (!userDisconnect)
                        {
                            Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "ConnectionLost", "Utracono połączenie");
                        }
                    }
                    break;
                }
            }
        }

        public void Write(string message)
        {
            if (socket != null)
            {
                if (socket.IsConnected)
                {
                    try
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(message + "\r\n");
                        socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch (Java.IO.IOException)
                    {
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Wystąpił błąd podczas wysyłania danych");
                        CloseConnection();
                    }
                }
            }
        }

        public string Read()
        {
            if (socket != null)
            {
                if (socket.IsConnected)
                {
                    byte[] buffer = new byte[100];
                    socket.InputStream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer);
                    return message;
                }
                else
                {
                    throw new Exception("Socket is not connected");
                }
            }
            else
            {
                throw new NullReferenceException("Socket is null");
            }
        }
    }
}