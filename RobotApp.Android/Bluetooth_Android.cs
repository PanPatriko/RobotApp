using Android.App;
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

[assembly: Xamarin.Forms.Dependency(typeof(Bluetooth_Android))]

namespace RobotApp.Droid
{
    class Bluetooth_Android : IBluetooth
    {
        BluetoothSocket socket;
        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

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
                    Write("Disconnected with Android");
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                  //  Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Rozłączono z " + socket.RemoteDevice.Name);
                    socket.Close();
                }
                catch (Java.IO.IOException)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Cannot close socket");
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
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Cannot create socket");
                }
                try
                {
                    await socket.ConnectAsync();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "połączono z " + device.Name + " " + device.Address);
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Połączono z " + device.Name);
                    Write("Connected with Android");
                }
                catch (Java.IO.IOException)
                {
                    socket.Close();
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Cannot connect to device");
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
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Hi", s);
                        s = "";
                    }
                }
                catch (Java.IO.IOException)
                {
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Input stream was disconnected");
                    Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "State", "rozłączono");
                    socket.Close();
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
                        Xamarin.Forms.MessagingCenter.Send(Xamarin.Forms.Application.Current, "Alert", "Error occurred when sending data");
                        CloseConnection();
                    }
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