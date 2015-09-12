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
using System.IO;

namespace Bonsai.Aruco
{
    public class DetectMarkers : Transform<IplImage, MarkerFrame>
    {
        public DetectMarkers()
        {
            Param1 = 7.0;
            Param2 = 7.0;
            MinSize = 0.04f;
            MaxSize = 0.5f;
            ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
            CornerRefinement = CornerRefinementMethod.Lines;
            MarkerSize = 10;
        }

        [FileNameFilter("Camera Files|*.yml;*.xml|YAML Files|*.yml|XML Files|*.xml")]
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string CameraParameters { get; set; }

        public ThresholdMethod ThresholdMethod { get; set; }

        public double Param1 { get; set; }

        public double Param2 { get; set; }

        public float MinSize { get; set; }

        public float MaxSize { get; set; }

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

                Size size;
                cameraMatrix = new Mat(3, 3, Depth.F32, 1);
                distortion = new Mat(1, 4, Depth.F32, 1);
                var parametersFileName = CameraParameters;
                if (!string.IsNullOrEmpty(parametersFileName))
                {
                    parameters = new CameraParameters();
                    var extension = Path.GetExtension(parametersFileName);
                    if (extension == ".xml") parameters.ReadFromXmlFile(parametersFileName);
                    else parameters.ReadFromFile(parametersFileName);
                    parameters.CopyParameters(cameraMatrix, distortion, out size);
                }

                return source.Select(input =>
                {
                    detector.ThresholdMethod = ThresholdMethod;
                    detector.Param1 = Param1;
                    detector.Param2 = Param2;
                    detector.MinSize = MinSize;
                    detector.MaxSize = MaxSize;
                    detector.CornerRefinement = CornerRefinement;

                    var detectedMarkers = detector.Detect(input, cameraMatrix, distortion, MarkerSize);
                    return new MarkerFrame(parameters, detectedMarkers);
                }).Finally(detector.Dispose);
            });
        }
    }
}
