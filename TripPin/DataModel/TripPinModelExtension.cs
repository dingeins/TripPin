using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPin.DataModel
{
    public partial class Person
    {
        public string FullName
        {
            get
            {
                return this._FirstName + " " + this._LastName;
            }
        }
    }

    public partial class Airline
    {
        public string LogoUri
        {
            get
            {
                return "ms-appx:///Assets/AirlineLogos/" + this._AirlineCode + ".png";
            }
        }
    }
}
