using RobotApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp
{
    public interface IBluetooth
    {
        bool IsBluetoothEnabled();

        bool IsConnected();

        void BluetoothEnable();

        void Scan();

        void CancelScan();

        void Write(string message);

        string Read();

        void Listen();

        void Connect(DeviceInfo deviceInfo);

        void CloseConnection();

        void GetBondedDevices();
    }
}
