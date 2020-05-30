using System.Collections.Generic;

using Xamarin.Forms.Maps;

namespace GongMask
{
    public class CustomMap : Map
    {
        public CustomMap()
        {

        }

        public CustomMap(MapSpan mapSpan) : base(mapSpan)
        {

        }

        public List<CustomPin> CustomPins { get; set; }
    }
}
