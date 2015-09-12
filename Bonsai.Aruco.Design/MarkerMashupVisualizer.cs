using Aruco.Net;
using Bonsai;
using Bonsai.Aruco.Design;
using Bonsai.Design;
using Bonsai.Vision.Design;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: TypeVisualizer(typeof(MarkerMashupVisualizer), Target = typeof(VisualizerMashup<IplImageVisualizer, MarkerVisualizer>))]

namespace Bonsai.Aruco.Design
{
    public class MarkerMashupVisualizer : MashupTypeVisualizer
    {
        IplImageVisualizer visualizer;

        public override void Show(object value)
        {
            var marker = (Marker)value;
            var visualizerImage = visualizer.VisualizerImage;
            if (visualizerImage != null && marker != null && marker.IsValid)
            {
                marker.Draw(visualizerImage, Scalar.Rgb(0, 0, 255), 2, true);
            }
        }

        public override void Load(IServiceProvider provider)
        {
            visualizer = (IplImageVisualizer)provider.GetService(typeof(DialogMashupVisualizer));
        }

        public override void Unload()
        {
        }
    }
}
