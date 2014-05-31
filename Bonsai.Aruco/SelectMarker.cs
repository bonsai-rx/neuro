using Aruco.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Aruco
{
    public class SelectMarker : Transform<MarkerFrame, MarkerTransform>
    {
        public int Id { get; set; }

        public override IObservable<MarkerTransform> Process(IObservable<MarkerFrame> source)
        {
            return source.Select(input =>
            {
                var markerTransform = new MarkerTransform();
                var selectedMarker = input.DetectedMarkers.FirstOrDefault(marker => marker.Id == Id);
                if (selectedMarker != null)
                {
                    var matrix = selectedMarker.GetGLModelViewMatrix();
                    markerTransform.M00 = matrix[0];
                    markerTransform.M01 = matrix[1];
                    markerTransform.M02 = matrix[2];
                    markerTransform.M03 = matrix[3];
                    markerTransform.M10 = matrix[4];
                    markerTransform.M11 = matrix[5];
                    markerTransform.M12 = matrix[6];
                    markerTransform.M13 = matrix[7];
                    markerTransform.M20 = matrix[8];
                    markerTransform.M21 = matrix[9];
                    markerTransform.M22 = matrix[10];
                    markerTransform.M23 = matrix[11];
                    markerTransform.M30 = matrix[12];
                    markerTransform.M31 = matrix[13];
                    markerTransform.M32 = matrix[14];
                    markerTransform.M33 = matrix[15];
                }

                return markerTransform;
            });
        }
    }
}
