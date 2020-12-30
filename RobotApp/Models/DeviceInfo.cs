using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RobotApp.Models
{
    public class DeviceInfo
    {
        public string Name { get; set; }
        public string Addres { get; set; }

        public DeviceInfo(string name, string addres)
        {
            Name = name;
            Addres = addres;
        }
        public override string ToString()
        {
            return Name + " " + Addres;
        }

        public override bool Equals(object obj)
        {
            if ((obj as DeviceInfo).Name == Name && (obj as DeviceInfo).Addres == Addres)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class GroupedModel : ObservableCollection<DeviceInfo>
    {
        public string DeviceType { get; set; }
    }
}
