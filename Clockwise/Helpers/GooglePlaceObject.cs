using System;
namespace Clockwise
{
    public class GooglePlaceObject
    {
        public string Name { get; set; }
        public string FormattedAddress { get; set; }

        public GooglePlaceObject(){
            
        }

        public GooglePlaceObject(string name, string formattedAddress)
        {
            Name = name;
            FormattedAddress = formattedAddress;
        }
    }
}
