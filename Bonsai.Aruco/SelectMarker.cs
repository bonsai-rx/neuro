using Aruco.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Aruco
{
    [Description("Selects the specified marker from the set of detected image markers, or an invalid marker if the marker is not detected.")]
    public class SelectMarker : Transform<MarkerFrame, Marker>
    {
        [Description("The id of the marker.")]
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
