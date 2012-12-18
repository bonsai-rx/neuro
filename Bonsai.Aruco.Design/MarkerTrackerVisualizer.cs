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

[assembly: TypeVisualizer(typeof(MarkerTrackerVisualizer), Target = typeof(MarkerTracker))]

namespace Bonsai.Aruco.Design
{
    public class MarkerTrackerVisualizer : IplImageVisualizer
    {
        IplImage input;
        MarkerTracker tracker;
        IDisposable inputObserver;

        public override void Show(object value)
        {
            if (input != null)
            {
                var markerFrame = (MarkerFrame)value;
                var image = new IplImage(input.Size, 8, 3);
                Core.cvCopy(input, image);
                foreach (var marker in markerFrame.DetectedMarkers)
                {
                    marker.Draw(image.DangerousGetHandle(), 0, 0, 255, 2, true);
                    if (tracker != null && tracker.Parameters != null)
                    {
                        CvDrawingUtils.Draw3dCube(image.DangerousGetHandle(), marker, tracker.Parameters);
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
                var workflowNode = (from node in workflow
                                    where node.Value == context.Source
                                    select node).FirstOrDefault();
                var trackerNode = workflow.Predecessors(workflowNode).Single();
                var trackerBuilder = trackerNode.Value as SelectBuilder;
                if (trackerBuilder != null)
                {
                    tracker = (MarkerTracker)trackerBuilder.Transform;
                }

                var inputNode = workflow.Predecessors(trackerNode).Select(node => node.Value as InspectBuilder).SingleOrDefault();
                if (inputNode != null)
                {
                    inputObserver = inputNode.Output.Subscribe(output => input = output as IplImage);
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
