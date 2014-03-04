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

[assembly: TypeVisualizer(typeof(MarkerTrackerVisualizer), Target = typeof(MarkerTracker))]

namespace Bonsai.Aruco.Design
{
    public class MarkerTrackerVisualizer : IplImageVisualizer
    {
        IplImage input;
        IDisposable inputObserver;

        public override void Show(object value)
        {
            if (input != null)
            {
                var markerFrame = (MarkerFrame)value;
                var image = input.Clone();
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
            var workflow = (ExpressionBuilderGraph)provider.GetService(typeof(ExpressionBuilderGraph));
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            if (context != null)
            {
                var workflowNode = workflow.First(node => node.Value == context.Source);
                var inputNode = workflow.Predecessors(workflowNode).Select(node => node.Value as InspectBuilder).FirstOrDefault();
                if (inputNode != null)
                {
                    inputObserver = inputNode.Output.Merge().Subscribe(output => input = output as IplImage);
                }
            }

            base.Load(provider);
        }

        public override void Unload()
        {
            if (inputObserver != null)
            {
                inputObserver.Dispose();
            }

            base.Unload();
        }
    }
}
