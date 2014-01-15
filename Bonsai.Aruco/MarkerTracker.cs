using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using Bonsai;
using Aruco.Net;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reactive.Linq;

namespace Bonsai.Aruco
{
    public class MarkerTracker : Transform<IplImage, MarkerFrame>
    {
        public MarkerTracker()
        {
            Param1 = 7.0;
            Param2 = 7.0;
            ThresholdType = ThresholdMethod.AdaptiveThreshold;
            CornerRefinement = CornerRefinementMethod.Harris;
        }

        [FileNameFilter("YML Files|*.yml;*.xml")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string CameraParameters { get; set; }

        public ThresholdMethod ThresholdType { get; set; }

        public double Param1 { get; set; }

        public double Param2 { get; set; }

        public CornerRefinementMethod CornerRefinement { get; set; }

        public float MarkerSize { get; set; }

        public override IObservable<MarkerFrame> Process(IObservable<IplImage> source)
        {
            return Observable.Defer(() =>
            {
                Mat cameraMatrix = null;
                Mat distortion = null;
                CameraParameters parameters = null;
                var detector = new MarkerDetector();
                if (!string.IsNullOrEmpty(CameraParameters))
                {
                    parameters = new CameraParameters();
                    parameters.ReadFromXmlFile(CameraParameters);

                    cameraMatrix = new Mat(3, 3, Depth.F32, 1);
                    distortion = new Mat(1, 4, Depth.F32, 1);
                    parameters.CopyParameters(cameraMatrix.DangerousGetHandle(), distortion.DangerousGetHandle());
                }

                return source.Select(input =>
                {
                    var hInput = input.DangerousGetHandle();
                    var hCamMatrix = cameraMatrix != null ? cameraMatrix.DangerousGetHandle() : IntPtr.Zero;
                    var hDistortion = distortion != null ? distortion.DangerousGetHandle() : IntPtr.Zero;
                    detector.ThresholdType = ThresholdType;
                    detector.Param1 = Param1;
                    detector.Param2 = Param2;
                    detector.CornerRefinement = CornerRefinement;

                    var detectedMarkers = detector.Detect(hInput, hCamMatrix, hDistortion, MarkerSize);
                    return new MarkerFrame(parameters, detectedMarkers);
                }).Finally(detector.Dispose);
            });
        }
    }
}
