using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonsai.Design;
using Bonsai.Vision.Design;
using OpenCV.Net;
using Bonsai.Dag;
using Bonsai.Expressions;
using Aruco.Net;
using Bonsai;
using Bonsai.Aruco.Design;
using Bonsai.Aruco;
using System.Reactive.Linq;
using System.Windows.Forms;

[assembly: TypeVisualizer(typeof(DetectMarkersVisualizer), Target = typeof(DetectMarkers))]

namespace Bonsai.Aruco.Design
{
    public class DetectMarkersVisualizer : IplImageVisualizer
    {
        IplImage input;
        bool showThreshold;
        IDisposable inputObserver;
        MarkerDetector imageThreshold;
        DetectMarkers detectMarkers;

        public override void Show(object value)
        {
            if (input != null)
            {
                var markerFrame = (MarkerFrame)value;
                var image = new IplImage(input.Size, input.Depth, 3);
                if (showThreshold)
                {
                    var threshold = new IplImage(input.Size, input.Depth, 1);
                    var grayscale = input;
                    if (grayscale.Channels > 1)
                    {
                        CV.CvtColor(input, threshold, ColorConversion.Bgr2Gray);
                        grayscale = threshold;
                    }

                    imageThreshold.Threshold(detectMarkers.ThresholdMethod, grayscale, threshold, detectMarkers.Param1, detectMarkers.Param2);
                    CV.CvtColor(threshold, image, ColorConversion.Gray2Bgr);
                }
                else if (input.Channels == 1)
                {
                    CV.CvtColor(input, image, ColorConversion.Gray2Bgr);
                }
                else CV.Copy(input, image);

                foreach (var marker in markerFrame.DetectedMarkers)
                {
                    marker.Draw(image, Scalar.Rgb(0, 0, 255), 2, true);
                    if (markerFrame.CameraParameters != null)
                    {
                        DrawingUtils.Draw3dCube(image, marker, markerFrame.CameraParameters);
                    }
                }

                base.Show(image);
            }
        }

        public override void Load(IServiceProvider provider)
        {
            showThreshold = false;
            var workflow = (ExpressionBuilderGraph)provider.GetService(typeof(ExpressionBuilderGraph));
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            if (context != null)
            {
                var workflowNode = workflow.First(node => node.Value == context.Source);
                var inputNode = workflow.Predecessors(workflowNode).Select(node => node.Value as InspectBuilder).FirstOrDefault();
                var workflowElement = ExpressionBuilder.GetWorkflowElement(context.Source) as DetectMarkers;
                if (inputNode != null && workflowElement != null)
                {
                    detectMarkers = workflowElement;
                    imageThreshold = new MarkerDetector();
                    inputObserver = inputNode.Output.Merge().Subscribe(output => input = output as IplImage);
                }
            }

            base.Load(provider);
            StatusStripEnabled = false;
            VisualizerCanvas.Canvas.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    showThreshold = !showThreshold;
                }
            };
        }

        public override void Unload()
        {
            if (inputObserver != null)
            {
                inputObserver.Dispose();
                imageThreshold.Dispose();
                imageThreshold = null;
                inputObserver = null;
            }

            base.Unload();
        }
    }
}
