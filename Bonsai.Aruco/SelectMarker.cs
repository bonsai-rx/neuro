using Aruco.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Aruco
{
    public class SelectMarker : Transform<MarkerFrame, Marker>
    {
        public int Id { get; set; }

        public override IObservable<Marker> Process(IObservable<MarkerFrame> source)
        {
            return source.Select(input =>
            {
                var selectedMarker = input.DetectedMarkers.FirstOrDefault(marker => marker.Id == Id);
                return selectedMarker ?? Marker.Empty;
            });
        }
    }
}
