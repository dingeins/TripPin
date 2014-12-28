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
}
