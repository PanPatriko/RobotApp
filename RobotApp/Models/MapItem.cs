using OxyPlot.Series;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RobotApp.Models
{
    public class MapItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        [TextBlob("pointsBlobbed")]
        public List<ScatterPoint> Points { get; set; }
        public string pointsBlobbed { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
