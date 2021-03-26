using System;
using System.Collections.Generic;
using System.Text;

namespace Fare.Library.Models
{
    public class Line
    {
        public int Id { get; set; }
        public string Route { get; set; }
        public decimal BaseFare { get; set; }
        public decimal ExcessCharge { get; set; }
        public List<Station> Stations { get; set; }
    }

    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Distance { get; set; }
    }
}
