using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentBase
{
    public class Number
    {
    }

    public class Country
    {
        public string Numeric { get; set; }
        public string Alpha2 { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public string Currency { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
    }

    public class Bank
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class BinResponse
    {
        public Number Number { get; set; }
        public string Scheme { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public Country Country { get; set; }
        public Bank Bank { get; set; }
    }
}
